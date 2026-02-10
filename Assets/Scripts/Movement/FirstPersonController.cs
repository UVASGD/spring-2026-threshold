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

    public void RotateVelocity(Quaternion rotation)
    {
        velocity = rotation * velocity;
    }
}