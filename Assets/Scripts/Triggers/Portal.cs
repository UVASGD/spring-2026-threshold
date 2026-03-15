using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Portal : MonoBehaviour
{
    [Header("Link")]
    [SerializeField] private Portal linkedPortal;

    [Header("Teleport")]
    [SerializeField] private bool teleportEnabled = true;
    private Transform player;
    [SerializeField] private bool playerIsOverlapping = false;
    private bool isJustArrived = false;

    [Header("Teleport Exit")]
    [SerializeField] private bool exitOnLinkedForwardSide = true;
    [SerializeField, Min(0f)] private float exitPlaneOffset = 0.05f;

    [Header("Object Teleport")]
    [SerializeField] private string grabbableObjectsLayerName = "grabbableObjects";
    private int grabbableObjectsLayer = -1;

    private readonly HashSet<Rigidbody> overlappingTeleportBodies = new HashSet<Rigidbody>();
    private readonly HashSet<Rigidbody> justArrivedTeleportBodies = new HashSet<Rigidbody>();
    private readonly List<Rigidbody> teleportBodyBuffer = new List<Rigidbody>();
    private readonly List<Rigidbody> staleBodyBuffer = new List<Rigidbody>();

    [Header("Portal Frame Layer")]
    [SerializeField] private string cullingLayerName = "PortalFrame";
    
    [Header("Clip Plane Offset")]
    [SerializeField] private float clipPlaneOffset = -0.01f;
    private Shader portalShader;

    // Internal References
    private Camera portalCam;
    private Material portalMaterial;
    private RenderTexture viewTexture;
    private Renderer screenMesh;
    // Crossing detection to avoid one-frame wrong-side render
    private float prevCameraDot = 0f;
    private bool prevCameraDotInitialized = false;

    [Header("Render Stability")]
    [SerializeField, Min(1)] private int renderCooldownFrames = 3;
    private int renderCooldownUntilFrame = -1;

    public bool TeleportEnabled
    {
        get => teleportEnabled;
        set
        {
            teleportEnabled = value;
            if (!teleportEnabled)
            {
                playerIsOverlapping = false;
                isJustArrived = false;
            }
        }
    }

    private void Start()
    {
        grabbableObjectsLayer = LayerMask.NameToLayer(grabbableObjectsLayerName);
        if (grabbableObjectsLayer == -1)
        {
            Debug.LogWarning($"Portal {name}: Layer '{grabbableObjectsLayerName}' was not found. Grabbable object teleport is disabled.", this);
        }

        TryResolvePlayer();

        //grab renderer
        screenMesh = GetComponentInChildren<Renderer>();
        if (screenMesh == null)
        {
            Debug.LogError($"Portal {name}: No MeshRenderer found in children!", this);
            return;
        }

        //create render tex
        viewTexture = new RenderTexture(Screen.width, Screen.height, 24);
        viewTexture.name = $"RT_{gameObject.name}";

        //setup material
        if (portalShader == null) portalShader = Shader.Find("Custom/PortalShader");
        portalMaterial = new Material(portalShader);
        portalMaterial.mainTexture = viewTexture;
        screenMesh.material = portalMaterial;

        CreatePortalCamera(); //init camera

        // initialize crossing-detection dot using main camera
        if (Camera.main != null)
        {
            prevCameraDot = Vector3.Dot(transform.forward, Camera.main.transform.position - transform.position);
            prevCameraDotInitialized = true;
        }
    }

    private void Update()
    {
        TryResolvePlayer();

        if (!teleportEnabled || linkedPortal == null || player == null) return;
        if (!playerIsOverlapping || isJustArrived) return;

        Vector3 portalToPlayer = player.position - transform.position;
        float dotProduct = Vector3.Dot(transform.forward, portalToPlayer);

        // This setup teleports when the player crosses the front-facing plane.
        if (dotProduct > 0f)
        {
            Teleport();
        }
    }

    private void FixedUpdate()
    {
        if (!teleportEnabled || linkedPortal == null) return;
        if (overlappingTeleportBodies.Count == 0) return;

        teleportBodyBuffer.Clear();
        staleBodyBuffer.Clear();

        foreach (Rigidbody body in overlappingTeleportBodies)
        {
            if (body == null)
            {
                staleBodyBuffer.Add(body);
                continue;
            }

            if (justArrivedTeleportBodies.Contains(body))
            {
                continue;
            }

            float dotProduct = Vector3.Dot(transform.forward, body.worldCenterOfMass - transform.position);
            if (dotProduct > 0f)
            {
                teleportBodyBuffer.Add(body);
            }
        }

        foreach (Rigidbody staleBody in staleBodyBuffer)
        {
            overlappingTeleportBodies.Remove(staleBody);
            justArrivedTeleportBodies.Remove(staleBody);
        }

        foreach (Rigidbody body in teleportBodyBuffer)
        {
            TeleportRigidbody(body);
        }
    }

    private void CreatePortalCamera()
    {
        GameObject camObj = new GameObject($"{gameObject.name}_Cam");
        portalCam = camObj.AddComponent<Camera>();
        portalCam.enabled = false; //pipeline handles differently
        portalCam.targetTexture = viewTexture;
        portalCam.nearClipPlane = 0.01f; //tweak near clip plane to not clip door frame

        //dupe post processing settings from Main Camera
        var mainCamData = Camera.main.GetComponent<UniversalAdditionalCameraData>();
        var portalCamData = camObj.AddComponent<UniversalAdditionalCameraData>();
        if (mainCamData != null) 
        {
            portalCamData.renderShadows = mainCamData.renderShadows;
        }

        int mainMask = Camera.main.cullingMask;
        int ignoreLayer = LayerMask.NameToLayer(cullingLayerName);
        
        if (ignoreLayer != -1)
        {
            portalCam.cullingMask = mainMask & ~(1 << ignoreLayer); //keep everything besides the ignored layer
        }
    }

    private void TryResolvePlayer()
    {
        if (player != null) return;

        if (FirstPersonController.i != null)
        {
            player = FirstPersonController.i.transform;
            return;
        }

        GameObject taggedPlayer = GameObject.FindGameObjectWithTag("Player");
        if (taggedPlayer != null)
        {
            player = taggedPlayer.transform;
        }
    }

    private void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += OnBeginCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= OnBeginCameraRendering;

        overlappingTeleportBodies.Clear();
        justArrivedTeleportBodies.Clear();
        teleportBodyBuffer.Clear();
        staleBodyBuffer.Clear();
        
        //free up memeory to prevent a memory leak
        if (viewTexture != null) viewTexture.Release();
        if (portalCam != null) Destroy(portalCam.gameObject);
        if (portalMaterial != null) Destroy(portalMaterial);
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (linkedPortal == null) return;
        if (camera.cameraType == CameraType.Preview || camera.targetTexture != null) return;

        // Cooldown is tracked per rendered frame (not per camera callback),
        // so Scene/Game cameras cannot consume it faster than intended.
        if (Time.frameCount > renderCooldownUntilFrame && screenMesh != null && !screenMesh.enabled)
        {
            screenMesh.enabled = true;
        }

        if (Time.frameCount <= renderCooldownUntilFrame)
        {
            if (camera == Camera.main)
            {
                SyncCameraCrossingDot(camera);
            }
            return;
        }

        // Run crossing detection before visibility checks so backwards crossings are not missed.
        if (camera == Camera.main)
        {
            float camDot = Vector3.Dot(transform.forward, camera.transform.position - transform.position);
            if (!prevCameraDotInitialized)
            {
                prevCameraDot = camDot;
                prevCameraDotInitialized = true;
            }

            const float crossingEpsilon = 0.0001f;
            bool crossedPortalPlane =
                (camDot > crossingEpsilon && prevCameraDot < -crossingEpsilon) ||
                (camDot < -crossingEpsilon && prevCameraDot > crossingEpsilon);

            // If sign flips between frames, the camera just crossed the portal plane.
            // Skip rendering the portal for this frame to avoid showing the wrong side.
            if (crossedPortalPlane)
            {
                prevCameraDot = camDot;
                BeginRenderCooldown();
                return;
            }

            prevCameraDot = camDot;
        }

        if (screenMesh == null || !screenMesh.isVisible) return;

        UpdateCamera(camera);

        //supress obsolete warning in editor
        #pragma warning disable 0618 
        UniversalRenderPipeline.RenderSingleCamera(context, portalCam);
        #pragma warning restore 0618
    }

    private void BeginRenderCooldown()
    {
        int frames = Mathf.Max(1, renderCooldownFrames);
        int cooldownEndFrame = Time.frameCount + frames - 1;
        if (cooldownEndFrame > renderCooldownUntilFrame)
        {
            renderCooldownUntilFrame = cooldownEndFrame;
        }

        if (screenMesh != null)
        {
            screenMesh.enabled = false;
        }
    }

    private void SyncCameraCrossingDot(Camera cam)
    {
        if (cam == null) return;

        prevCameraDot = Vector3.Dot(transform.forward, cam.transform.position - transform.position);
        prevCameraDotInitialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOverlapping = true;
            return;
        }

        Rigidbody body = other.attachedRigidbody;
        int otherLayer = body != null ? body.gameObject.layer : other.gameObject.layer;
        if (grabbableObjectsLayer == -1 || otherLayer != grabbableObjectsLayer) return;

        if (body != null)
        {
            overlappingTeleportBodies.Add(body);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOverlapping = false;
            isJustArrived = false;
            return;
        }

        Rigidbody body = other.attachedRigidbody;
        if (body != null)
        {
            overlappingTeleportBodies.Remove(body);
            justArrivedTeleportBodies.Remove(body);
        }
    }

    private void OnPlayerArrived()
    {
        isJustArrived = true;
        playerIsOverlapping = true;
    }

    private void OnRigidbodyArrived(Rigidbody body)
    {
        if (body == null) return;

        overlappingTeleportBodies.Add(body);
        justArrivedTeleportBodies.Add(body);
    }

    private void Teleport()
    {
        linkedPortal.OnPlayerArrived();

        // Force both portal surfaces into cooldown during teleport to prevent
        // a stale texture flash on the crossing frame.
        BeginRenderCooldown();
        linkedPortal.BeginRenderCooldown();

        Vector3 localPos = transform.InverseTransformPoint(player.position);
        localPos = Quaternion.Euler(0f, 180f, 0f) * localPos;

        // Keep x/y alignment but force a predictable side of the destination plane.
        float minExitDepth = Mathf.Max(exitPlaneOffset, Mathf.Abs(localPos.z));
        localPos.z = exitOnLinkedForwardSide ? minExitDepth : -minExitDepth;

        Vector3 targetPosition = linkedPortal.transform.TransformPoint(localPos);

        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * player.rotation;
        relativeRot = Quaternion.Euler(0f, 180f, 0f) * relativeRot;
        Quaternion targetRotation = linkedPortal.transform.rotation * relativeRot;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.SetPositionAndRotation(targetPosition, targetRotation);
            cc.enabled = true;
        }
        else
        {
            player.SetPositionAndRotation(targetPosition, targetRotation);
        }

        // Keep grabbed objects attached through portal traversal by teleporting
        // the held rigidbody immediately with the player.
        PhysicsGrabber grabber = player.GetComponentInChildren<PhysicsGrabber>();
        if (grabber != null && grabber.HeldObject != null)
        {
            TeleportRigidbody(grabber.HeldObject);
        }

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            SyncCameraCrossingDot(mainCam);
            linkedPortal.SyncCameraCrossingDot(mainCam);
        }

        playerIsOverlapping = false;
    }

    private void TeleportRigidbody(Rigidbody body)
    {
        if (body == null || linkedPortal == null) return;

        linkedPortal.OnRigidbodyArrived(body);

        Vector3 localPos = transform.InverseTransformPoint(body.position);
        localPos = Quaternion.Euler(0f, 180f, 0f) * localPos;

        float minExitDepth = Mathf.Max(exitPlaneOffset, Mathf.Abs(localPos.z));
        localPos.z = exitOnLinkedForwardSide ? minExitDepth : -minExitDepth;

        Vector3 targetPosition = linkedPortal.transform.TransformPoint(localPos);

        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * body.rotation;
        relativeRot = Quaternion.Euler(0f, 180f, 0f) * relativeRot;
        Quaternion targetRotation = linkedPortal.transform.rotation * relativeRot;

        Vector3 localVelocity = transform.InverseTransformDirection(body.linearVelocity);
        localVelocity = Quaternion.Euler(0f, 180f, 0f) * localVelocity;
        Vector3 targetVelocity = linkedPortal.transform.TransformDirection(localVelocity);

        Vector3 localAngularVelocity = transform.InverseTransformDirection(body.angularVelocity);
        localAngularVelocity = Quaternion.Euler(0f, 180f, 0f) * localAngularVelocity;
        Vector3 targetAngularVelocity = linkedPortal.transform.TransformDirection(localAngularVelocity);

        body.position = targetPosition;
        body.rotation = targetRotation;
        body.linearVelocity = targetVelocity;
        body.angularVelocity = targetAngularVelocity;
        body.WakeUp();

        overlappingTeleportBodies.Remove(body);
    }

    void UpdateCamera(Camera playerCam) //AI generated function since I don't know matrix multiplications or quaternions quite yet
    {
        // 1. Calculate Position & Rotation
        // Move the portal camera to the relative position of the player
        Matrix4x4 destFlipRotation = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(180.0f, Vector3.up), Vector3.one);
        Matrix4x4 sourceInvMat = destFlipRotation * transform.worldToLocalMatrix;

        Vector3 camPosInSourceSpace = sourceInvMat.MultiplyPoint(playerCam.transform.position);
        Quaternion camRotInSourceSpace = MathUtil_QuaternionFromMatrix(sourceInvMat) * playerCam.transform.rotation;

        portalCam.transform.position = linkedPortal.transform.TransformPoint(camPosInSourceSpace);
        portalCam.transform.rotation = linkedPortal.transform.rotation * camRotInSourceSpace;

        // 2. Calculate Oblique Projection Matrix (Clipping)
        Vector3 planePos = linkedPortal.transform.position + (linkedPortal.transform.forward * clipPlaneOffset);
        
        // Note: We use the positive forward vector (User Option 3 Fix)
        Plane p = new Plane(linkedPortal.transform.forward, planePos);

        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCam.worldToCameraMatrix)) * clipPlaneWorldSpace;

        portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }

    //helper method
    private static Quaternion MathUtil_QuaternionFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }
}