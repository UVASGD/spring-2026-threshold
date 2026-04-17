using UnityEngine;

public class LookTrigger : MonoBehaviour
{
    public float lookDistance = 5f;
    public LayerMask lookLayer; //the layer of an object that can be activated by a look trigger
    [SerializeField] private DynamicTrigger cutsceneTrigger;
    [SerializeField] private Collider lookTargetCollider;

    private bool hasActivated;

    private void Awake()
    {
        if (cutsceneTrigger == null)
        {
            cutsceneTrigger = GetComponentInChildren<DynamicTrigger>(true);
        }

        if (lookTargetCollider == null)
        {
            lookTargetCollider = GetComponent<Collider>();
        }
    }

    void Update()
    {
        Transform lookOrigin = null;
        if (FirstPersonController.i != null && FirstPersonController.i.cameraTransform != null)
        {
            lookOrigin = FirstPersonController.i.cameraTransform;
        }
        else if (Camera.main != null)
        {
            lookOrigin = Camera.main.transform;
        }

        if (lookOrigin == null)
        {
            return;
        }

        Ray ray = new Ray(lookOrigin.position, lookOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, lookDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide)
            && IsColliderInLookLayer(hit.collider)
            && IsLookHitThisTrigger(hit.collider)) //ensuring no walls are in the way
        {
            TriggerCutsceneExternally();
        }
    }

    public void TriggerCutsceneExternally()
    {
        if (cutsceneTrigger == null || hasActivated)
        {
            return;
        }

        cutsceneTrigger.externalTriggerActivated();
        hasActivated = true;
        enabled = false;

        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }

    private bool IsLookHitThisTrigger(Collider hitCollider)
    {
        if (hitCollider == null)
        {
            return false;
        }

        if (lookTargetCollider != null)
        {
            return hitCollider == lookTargetCollider;
        }

        return hitCollider.gameObject == gameObject || hitCollider.transform.IsChildOf(transform);
    }

    private bool IsColliderInLookLayer(Collider hitCollider)
    {
        if (hitCollider == null)
        {
            return false;
        }

        return (lookLayer.value & (1 << hitCollider.gameObject.layer)) != 0;
    }
}