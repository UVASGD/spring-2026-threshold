using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController i; //only one should exist per season

    private bool movementActive = true;
    public bool MovementActive => movementActive;

    private CharacterController controller;
    private PlayerInputActions input;

    private Vector2 move;
    private Vector2 look;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private Vector3 velocity;
    private bool isSprinting;
    private bool jumpEnabled = true;

    [Header("Camera")]
    public Transform cameraTransform;
    public float lookSensitivity = 1f;
    public float cameraPitch = 0f;

    [Header("Head Bob")]
    public float bobFrequency = 1.5f;
    public float bobHeight = 0.05f;
    public float sprintMult = 1.5f; //how much extra bob happens when sprinting
    private float bobTimer = 0f;
    private Vector3 cameraStartPos;
    [Header("Interaction")]
    public float interactDistance = 3f;
    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.5f;   // Time between steps while walking
    public float sprintStepMultiplier = 0.75f; // Faster steps when sprinting

    [Header("Feet Push")]
    public float footPushForce = 4f;
    public float maxPushMass = 40f;
    [Range(0.1f, 1f)] public float footContactHeightRatio = 0.35f;
    public LayerMask pushableLayers = ~0;

    private float footstepTimer = 0f;

    void Awake()
    {
        if(i == null){
            i = this;
        }

        controller = GetComponent<CharacterController>();
        input = new PlayerInputActions();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraStartPos = cameraTransform.localPosition;
    }
    // Sets the camera rotation and updates cameraPitch to match
    public void SetCameraRotation(Quaternion rotation)
    {
        // Keep controller yaw and camera pitch in sync with scripted world rotation.
        Vector3 worldEuler = rotation.eulerAngles;

        transform.rotation = Quaternion.Euler(0f, worldEuler.y, 0f);

        // Unity's Euler angles wrap at 360, so convert pitch to -180..180.
        float pitch = worldEuler.x;
        if (pitch > 180f) pitch -= 360f;
        cameraPitch = Mathf.Clamp(pitch, -90f, 90f);

        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
       
    }
    private bool lookEnabled = true;
    public void SetLookEnabled(bool enabled)
    {
        lookEnabled = enabled;
    }
    public void SetJumpEnabled(bool enabled)
    {
        jumpEnabled = enabled;
    }
    public void toggleGravity(bool on)
    {
        if (on)
        {
            gravity = 0f;
        }
        else
        {
            gravity = -9.81f;
        }
    }
    void OnEnable()
    {
        input.Player.Enable();

        input.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => move = Vector2.zero;

        input.Player.Look.performed += ctx => look = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += ctx => look = Vector2.zero;

        input.Player.Jump.performed += ctx => Jump();
        input.Player.Sprint.performed += ctx => isSprinting = true;
        input.Player.Sprint.canceled += ctx => isSprinting = false;
        input.Player.Interact.performed += ctx => TryInteract();
    }
    void OnDisable()
    {
        input.Player.Disable();
    }
    public void changePlayerControlState(bool move){
        movementActive = move;
    }
    void Update(){
        if(movementActive)
        {
            HandleUpdate();
        }
    }

    void HandleUpdate()
    {
        HandleLook();
        HandleMovement();
        HandleHeadBob();
        HandleFootsteps();
    }
    private void HandleMovement()
    {
        Vector3 moveDir = transform.right * move.x + transform.forward * move.y;
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        controller.Move(moveDir * speed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    private void HandleLook()
    {
        if (!lookEnabled) return;

        Vector2 lookInput = look * lookSensitivity;

        // Vertical camera rotation
        cameraPitch -= lookInput.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);

        // Horizontal body rotation
        transform.Rotate(Vector3.up * lookInput.x);
    }
    private void Jump()
    {
        if (!jumpEnabled) return;

        if (controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
    private void HandleHeadBob()
    {
        if (controller.isGrounded && move.sqrMagnitude > 0.1f)
        {
            float speed = isSprinting ? sprintSpeed : walkSpeed;

            // How fast the bob cycles
            float frequency = bobFrequency * (isSprinting ? sprintMult : 1f);

            // Increase timer
            bobTimer += Time.deltaTime * frequency;

            // Sin wave bobbing (vertical)
            float bobOffset = Mathf.Sin(bobTimer) * bobHeight;

            // Apply offset to camera
            cameraTransform.localPosition = cameraStartPos + new Vector3(0f, bobOffset, 0f);
        }
        else
        {
            // Reset camera to rest position when not moving
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                cameraStartPos,
                Time.deltaTime * 10f
            );
        }
    }
    private void TryInteract()
    {
        if(movementActive){
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // Look for any script that implements IInteract
            IInteract interactable = hit.collider.GetComponent<IInteract>();

            if (interactable != null)
            {
                interactable.OnPlayerInteract();
            }
        }
        }
    }
    private void HandleFootsteps()
    {
        if (!controller.isGrounded || move.sqrMagnitude < 0.1f)
        {
            footstepTimer = 0f;
            return;
        }

        float interval = isSprinting ? footstepInterval * sprintStepMultiplier : footstepInterval;

        footstepTimer += Time.deltaTime;

        if (footstepTimer > interval)
        {
            PlayFootstep();
            footstepTimer = 0f;
        }
    }
    private void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepSource.PlayOneShot(clip);
    }
    private void ScalePlayerHeight(float newHeight)
    {
        //changes the y component of the player, making them appear shorter
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, newHeight, gameObject.transform.localScale.z);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;
        if (rb.mass > maxPushMass) return;
        if (((1 << rb.gameObject.layer) & pushableLayers) == 0) return;

        float contactLocalY = transform.InverseTransformPoint(hit.point).y;
        float bottomY = controller.center.y - (controller.height * 0.5f);
        float feetZoneTop = bottomY + (controller.height * footContactHeightRatio);
        if (contactLocalY > feetZoneTop) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        if (pushDir.sqrMagnitude < 0.0001f) return;

        rb.AddForce(pushDir.normalized * footPushForce, ForceMode.Impulse);
    }

    public void RotateVelocity(Quaternion rotation)
    {
        velocity = rotation * velocity;
    }
}