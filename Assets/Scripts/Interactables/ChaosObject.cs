using UnityEngine;

[RequireComponent(typeof(Rigidbody))] //only objects with a rigidbody can be a chaos object
public class ChaosObject : MonoBehaviour
{
    [Tooltip("How much the change in rotation impacts its room chaos contribution")]
    [SerializeField] float rotationalWeight;

    [Tooltip("How much the change in position impacts its room chaos contribution")]
    [SerializeField] float positionalWeight;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }
    
    private float chaosContribution;
    public float ChaosContribution => chaosContribution;
    public void updateChaosContribution()
    {
        //how far has it moved?
        float distanceMoved = Vector3.Distance(initialPosition, transform.position);

        //how much has it rotated?
        float amountRotated = Quaternion.Angle(initialRotation, transform.rotation);

        //return each one multiplied by its respective weight.
        //Rotation is multiplied by 0.05f for tuning.
        chaosContribution = (distanceMoved * positionalWeight) + (amountRotated * 0.05f * rotationalWeight);
        if(chaosContribution > 50)
        {
            Debug.Log($"Chaos contribution of {chaosContribution}");
        }
    }

    public bool isAwake()
    {
        return !rb.IsSleeping();
    }
}