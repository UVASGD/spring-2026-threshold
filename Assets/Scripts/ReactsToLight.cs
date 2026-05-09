using UnityEngine;
using UnityEngine.Events;

public class ReactsToLight : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] UnityEvent onLightThresholdReached;
    [SerializeField] UnityEvent onLightThresholdLost;

    [Header("Light Detection")]
    [SerializeField] LayerMask occlusionMask = ~0;
    [SerializeField] float checkInterval = 0.1f;
    [SerializeField] float lightThreshold = 0.5f;
    [SerializeField] bool triggerOnlyOnce;

    private bool isAboveThreshold;
    private bool hasTriggered;
    private float nextCheckTime;
    private Collider cachedCollider;

    void Awake()
    {
        cachedCollider = GetComponentInChildren<Collider>();
    }

    void Update()
    {
        if (Time.time < nextCheckTime)
        {
            return;
        }

        nextCheckTime = Time.time + Mathf.Max(0.02f, checkInterval);

        float exposure = CalculateLightExposure();
        bool isCurrentlyAboveThreshold = exposure >= lightThreshold;

        if (isCurrentlyAboveThreshold == isAboveThreshold)
        {
            return;
        }

        isAboveThreshold = isCurrentlyAboveThreshold;

        if (isAboveThreshold)
        {
            if (!triggerOnlyOnce || !hasTriggered)
            {
                hasTriggered = true;
                onLightThresholdReached?.Invoke();
            }
        }
        else
        {
            onLightThresholdLost?.Invoke();
        }
    }

    private float CalculateLightExposure()
    {
        Bounds bounds = GetTargetBounds();
        Vector3 c = bounds.center;
        Vector3 e = bounds.extents;

        Vector3[] samplePoints = new Vector3[]
        {
            c,
            c + new Vector3( e.x,  e.y,  e.z),
            c + new Vector3( e.x,  e.y, -e.z),
            c + new Vector3( e.x, -e.y,  e.z),
            c + new Vector3( e.x, -e.y, -e.z),
            c + new Vector3(-e.x,  e.y,  e.z),
            c + new Vector3(-e.x,  e.y, -e.z),
            c + new Vector3(-e.x, -e.y,  e.z),
            c + new Vector3(-e.x, -e.y, -e.z)
        };

        float totalExposure = 0f;
        for (int i = 0; i < samplePoints.Length; i++)
        {
            totalExposure += CalculatePointExposure(samplePoints[i]);
        }

        return totalExposure / samplePoints.Length;
    }

    private float CalculatePointExposure(Vector3 targetPoint)
    {
        Light[] sceneLights = FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        float exposure = 0f;

        for (int i = 0; i < sceneLights.Length; i++)
        {
            Light lightSource = sceneLights[i];
            if (lightSource == null || !lightSource.isActiveAndEnabled || lightSource.intensity <= 0f)
            {
                continue;
            }

            exposure += CalculateContribution(lightSource, targetPoint);
        }

        return exposure;
    }

    private float CalculateContribution(Light lightSource, Vector3 targetPoint)
    {
        if (lightSource.type == LightType.Directional)
        {
            Vector3 lightDirection = lightSource.transform.forward.normalized;
            Vector3 rayOrigin = targetPoint - (lightDirection * 200f);

            if (IsOccluded(rayOrigin, targetPoint))
            {
                return 0f;
            }

            return lightSource.intensity;
        }

        Vector3 lightPos = lightSource.transform.position;
        Vector3 toPoint = targetPoint - lightPos;
        float distance = toPoint.magnitude;

        if (distance <= 0.0001f)
        {
            return lightSource.intensity;
        }

        if ((lightSource.type == LightType.Point || lightSource.type == LightType.Spot) && distance > lightSource.range)
        {
            return 0f;
        }

        if (lightSource.type == LightType.Spot)
        {
            float angleToPoint = Vector3.Angle(lightSource.transform.forward, toPoint);
            float halfAngle = lightSource.spotAngle * 0.5f;
            if (angleToPoint > halfAngle)
            {
                return 0f;
            }
        }

        if (IsOccluded(lightPos, targetPoint))
        {
            return 0f;
        }

        float attenuation = 1f;
        if (lightSource.range > 0f)
        {
            attenuation = Mathf.Clamp01(1f - (distance / lightSource.range));
        }

        if (lightSource.type == LightType.Spot)
        {
            float angleToPoint = Vector3.Angle(lightSource.transform.forward, toPoint);
            float halfAngle = lightSource.spotAngle * 0.5f;
            float spotFactor = Mathf.Clamp01(1f - (angleToPoint / halfAngle));
            attenuation *= spotFactor;
        }

        return lightSource.intensity * attenuation;
    }

    private bool IsOccluded(Vector3 origin, Vector3 targetPoint)
    {
        Vector3 toTarget = targetPoint - origin;
        float distance = toTarget.magnitude;

        if (distance <= 0.0001f)
        {
            return false;
        }

        Vector3 direction = toTarget / distance;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, occlusionMask, QueryTriggerInteraction.Ignore))
        {
            return hit.transform != transform && !hit.transform.IsChildOf(transform);
        }

        return false;
    }

    private Bounds GetTargetBounds()
    {
        if (cachedCollider != null)
        {
            return cachedCollider.bounds;
        }

        return new Bounds(transform.position, Vector3.zero);
    }
}
