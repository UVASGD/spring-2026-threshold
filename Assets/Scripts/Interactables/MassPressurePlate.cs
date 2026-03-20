using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MassPressurePlate : MonoBehaviour
{
    [SerializeField] private float requiredMass;
    [SerializeField] private bool impactfulDeactivation; //whether or not the pressure releasing deactivates anything
    [Header("Resulting events")]
    [SerializeField] UnityEvent weighedEvent;
    [SerializeField] UnityEvent unweighedEvent;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private MeshRenderer mRenderer;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private BoxCollider triggerCollider;
    [SerializeField] private float triggerTopPadding = 0.02f;
    private List<Rigidbody> objectsOnPlate = new List<Rigidbody>();
    private float currentMass = 0f;
    private bool isActivated = false;
    private Vector3 initialTriggerCenter;
    private Vector3 initialTriggerSize;
    private float initialTriggerBottomLocalY;
    private float initialTriggerBottomWorldY;

    private bool triggered = false;

    private void Awake()
    {
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<BoxCollider>();
        }

        if (triggerCollider != null)
        {
            initialTriggerCenter = triggerCollider.center;
            initialTriggerSize = triggerCollider.size;
            initialTriggerBottomLocalY = initialTriggerCenter.y - (initialTriggerSize.y * 0.5f);
            initialTriggerBottomWorldY = triggerCollider.bounds.min.y;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && 
           (detectionLayer & (1 << other.gameObject.layer)) != 0)
        {
            if (!objectsOnPlate.Contains(other.attachedRigidbody))
            {
                objectsOnPlate.Add(other.attachedRigidbody);
                CalculateMass();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            objectsOnPlate.Remove(other.attachedRigidbody);
            CalculateMass();
        }
    }

    private void CalculateMass()
    {
        currentMass = 0f;
        for (int i = objectsOnPlate.Count - 1; i >= 0; i--)
        {
            if (objectsOnPlate[i] == null)
            {
                objectsOnPlate.RemoveAt(i);
            }
        }

        foreach (var rb in objectsOnPlate)
        {
            currentMass += rb.mass;
        }

        UpdateTriggerHeight();
        CheckActivation();
    }

    private void UpdateTriggerHeight()
    {
        if (triggerCollider == null)
        {
            return;
        }

        float targetTop = initialTriggerBottomWorldY + (initialTriggerSize.y * Mathf.Abs(transform.lossyScale.y));

        foreach (var rb in objectsOnPlate)
        {
            Collider[] colliders = rb.GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                if (col == null || col.isTrigger)
                {
                    continue;
                }

                targetTop = Mathf.Max(targetTop, col.bounds.max.y + triggerTopPadding);
            }
        }

        float worldScaleY = Mathf.Max(0.0001f, Mathf.Abs(transform.lossyScale.y));
        float desiredHeightLocal = Mathf.Max(initialTriggerSize.y, (targetTop - initialTriggerBottomWorldY) / worldScaleY);

        Vector3 updatedSize = triggerCollider.size;
        Vector3 updatedCenter = triggerCollider.center;

        updatedSize.y = desiredHeightLocal;
        updatedCenter.y = initialTriggerBottomLocalY + (desiredHeightLocal * 0.5f);

        triggerCollider.size = updatedSize;
        triggerCollider.center = updatedCenter;
    }

    private void CheckActivation()
    {
        if (currentMass >= requiredMass && !isActivated)
        {
            isActivated = true;
            OnPlateActivated();
        }
        else if (currentMass < requiredMass && isActivated)
        {
            isActivated = false;
            OnPlateDeactivated();
        }
    }

    void OnPlateActivated()
    {
        if(!triggered) triggered = true; else if (!impactfulDeactivation) return; //if there is no impactful deactivation and already triggered, do not trigger again.
        if(!triggered) return; //return to prevent the event from invoking
        triggered = true;
        Debug.Log("Pressure plate activated");
        mRenderer.material = onMaterial;

        //invoke the event
        weighedEvent?.Invoke();
    }
    void OnPlateDeactivated()
    {
        if(!impactfulDeactivation) return;

        if(triggered) triggered = false;
        if(triggered) return; //return to prevent the event from invoking
        Debug.Log("Pressure plate deactivated");
        mRenderer.material = offMaterial;

        //invoke the cancellation event
        weighedEvent?.Invoke();
    }
}