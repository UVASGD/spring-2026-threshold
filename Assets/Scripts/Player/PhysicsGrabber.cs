using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PhysicsGrabber : MonoBehaviour
{
    //this script handles functionality with player grabbing physics objects
    public Transform holdPoint;
    private Camera playerCamera;
    public LayerMask grabbableLayer;

    [Header("Physics stuff")]
    [SerializeField] float grabRange = 5f;
    [SerializeField] float attractionSpeed = 15f;
    [SerializeField] float maxHoldDistance = 2.5f; //drop an object if it gets stuck

    private Rigidbody heldObject;
    private float originalLinearDamping;
    private float originalAngularDamping;
    private bool jumpLockedByGrab;
    private Collider[] playerColliders;

    public Rigidbody HeldObject => heldObject;

    void OnEnable()
    {
        playerCamera = FirstPersonController.i.cameraTransform.GetComponent<Camera>();
        playerColliders = FirstPersonController.i.GetComponentsInChildren<Collider>();
    }

    void OnDisable()
    {
        if(heldObject != null)
        {
            Drop();
        }
    }

    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if(heldObject == null)
            {
                tryGrab();
            }
            else
            {
                Drop();
            }
        }
    }

    //fixed update prevents exponnentially more expensive physics calculations due to high frame rates
    void FixedUpdate()
    {
        if(heldObject != null)
        {
            //maintain the player hold on the object by applying a force towards the hold point
            maintainHold();
        }
    }
    
    private void tryGrab()
    {
        //raycast to find an object to pick up
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, grabRange, grabbableLayer))
        {
            if(hit.rigidbody != null)
            {
                heldObject = hit.rigidbody;

                //disable gravity for the grabbed object
                heldObject.useGravity = false;

                originalLinearDamping = heldObject.linearDamping;
                originalAngularDamping = heldObject.angularDamping;

                heldObject.linearDamping = 10f;
                heldObject.angularDamping = 10f;

                setHeldCollisionWithPlayer(true);
            }
        }
    }

    private void maintainHold()
    {
        Vector3 directionToPoint = holdPoint.position - heldObject.position;
        float distanceToPoint = directionToPoint.magnitude;

        if(distanceToPoint > maxHoldDistance) //drop the item if it is too far from the holdPoint at any time
        {
            Drop();    
            return;
        }

        //move the object towards the hold point
        heldObject.linearVelocity = (directionToPoint * attractionSpeed) / heldObject.mass; //inertia slows down this velocity
    }

    private void Drop()
    {
        if(heldObject == null) return; //null check for held object

        //restore original physics of the object
        heldObject.useGravity = true;
        heldObject.linearDamping = originalLinearDamping;
        heldObject.angularDamping = originalAngularDamping;

        setHeldCollisionWithPlayer(false);

        heldObject = null;
    }

    private void setHeldCollisionWithPlayer(bool shouldIgnore)
    {
        if(heldObject == null || playerColliders == null) return;

        Collider[] heldColliders = heldObject.GetComponentsInChildren<Collider>();
        foreach(Collider heldCollider in heldColliders)
        {
            if(heldCollider == null) continue;

            foreach(Collider playerCollider in playerColliders)
            {
                if(playerCollider == null) continue;
                Physics.IgnoreCollision(playerCollider, heldCollider, shouldIgnore);
            }
        }
    }
}