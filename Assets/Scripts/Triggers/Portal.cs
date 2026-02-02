using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Portal : MonoBehaviour
{
    [SerializeField] Portal linkedPortal;
    [Header("Portal Frame Layer")]
    [SerializeField] string cullingLayerName = "PortalFrame";
    
    [Header("Clip Plane Offset")]
    [SerializeField] float clipPlaneOffset = -0.01f;
    private Shader portalShader;

    // Internal References
    private Camera portalCam;
    private Material portalMaterial;
    private RenderTexture viewTexture;
    private Renderer screenMesh;
    // Crossing detection to avoid one-frame wrong-side render
    private float prevCameraDot = 0f;
    private bool prevCameraDotInitialized = false;
    // When crossing occurs, skip a few frames to avoid flicker when rotating
    private int skipPortalRenderFrames = 0;

    private void Start()
    {
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

    private void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += OnBeginCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= OnBeginCameraRendering;
        
        //free up memeory to prevent a memory leak
        if (viewTexture != null) viewTexture.Release();
        if (portalCam != null) Destroy(portalCam.gameObject);
        if (portalMaterial != null) Destroy(portalMaterial);
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        // If we're currently skipping frames (cooldown), decrement and skip.
        if (skipPortalRenderFrames > 0)
        {
            skipPortalRenderFrames--;
            if (skipPortalRenderFrames == 0 && screenMesh != null) screenMesh.enabled = true;
            return;
        }

        if (linkedPortal == null || !screenMesh.isVisible) return;
        if (camera.cameraType == CameraType.Preview || camera.targetTexture != null) return;

        // Only perform crossing-detection for the main/player camera.
        if (camera == Camera.main)
        {
            float camDot = Vector3.Dot(transform.forward, camera.transform.position - transform.position);
            if (!prevCameraDotInitialized) prevCameraDot = camDot;

            // If sign flips between frames, the camera just crossed the portal plane.
            // Skip rendering the portal for this frame to avoid showing the wrong side.
            if (camDot * prevCameraDot < 0f)
            {
                prevCameraDot = camDot;
                // Start a small cooldown to avoid flicker from rotation/visibility toggles
                skipPortalRenderFrames = 3;
                if (screenMesh != null) screenMesh.enabled = false;
                return;
            }

            prevCameraDot = camDot;
        }

        UpdateCamera(camera);

        //supress obsolete warning in editor
        #pragma warning disable 0618 
        UniversalRenderPipeline.RenderSingleCamera(context, portalCam);
        #pragma warning restore 0618
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