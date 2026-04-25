using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Impermanent objects have different properties when they are not illuminated.
/// </summary>
public class ImpermanentObject : MonoBehaviour
{
    public UnityEvent onUniluminated;
    public UnityEvent onIlumination;
    [SerializeField] LayerMask occlusionMask = ~0;
    [SerializeField] float lightCheckInterval = 0.1f;
    [SerializeField] [Range(0,3f)] private float maxLitRatioForDark = 0.25f;

    private bool isIlluminated;
    private float nextLightCheckTime;
    private Collider cachedCollider;

    private void Awake()
    {
        cachedCollider = GetComponentInChildren<Collider>();
    }

    private void OnEnable()
    {
        nextLightCheckTime = 0f;
    }

    private void Update()
    {
        if (Time.time < nextLightCheckTime)
        {
            return;
        }

        nextLightCheckTime = Time.time + Mathf.Max(0.02f, lightCheckInterval);

        float litRatio = CalculateLitRatio();
        bool illuminatedNow = litRatio > maxLitRatioForDark;
        if (illuminatedNow != isIlluminated)
        {
            isIlluminated = illuminatedNow;
            togglePermanence(isIlluminated);
        }
    }

    public void togglePermanence(bool illuminated)
    {
        if (illuminated)
        {
            onIlumination?.Invoke();
        }
        else
        {
            onUniluminated?.Invoke();
        }
    }

    private float CalculateLitRatio()
    {
        Bounds bounds = GetTargetBounds();
        Vector3 c = bounds.center;
        Vector3 e = bounds.extents;

        if (e.sqrMagnitude <= 0.0001f)
        {
            return IsPointLitByAnyLight(c) ? 1f : 0f;
        }

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

        int litSamples = 0;
        for (int i = 0; i < samplePoints.Length; i++)
        {
            if (IsPointLitByAnyLight(samplePoints[i]))
            {
                litSamples++;
            }
        }

        return litSamples / (float)samplePoints.Length;
    }

    private bool IsPointLitByAnyLight(Vector3 targetPoint)
    {
        Light[] sceneLights = FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        for (int i = 0; i < sceneLights.Length; i++)
        {
            Light lightSource = sceneLights[i];
            if (lightSource == null || !lightSource.isActiveAndEnabled)
            {
                continue;
            }

            if (lightSource.intensity <= 0f)
            {
                continue;
            }

            if (IsPointLitByLight(lightSource, targetPoint))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointLitByLight(Light lightSource, Vector3 targetPoint)
    {
        Vector3 origin;
        Vector3 toTarget;
        float maxDistance;

        if (lightSource.type == LightType.Directional)
        {
            Vector3 direction = lightSource.transform.forward;
            maxDistance = 200f;
            origin = targetPoint - (direction.normalized * maxDistance);
            toTarget = targetPoint - origin;
        }
        else
        {
            origin = lightSource.transform.position;
            toTarget = targetPoint - origin;
            maxDistance = toTarget.magnitude;

            if (lightSource.type == LightType.Point || lightSource.type == LightType.Spot)
            {
                if (maxDistance > lightSource.range)
                {
                    return false;
                }
            }

            if (lightSource.type == LightType.Spot)
            {
                float spotAngleToPoint = Vector3.Angle(lightSource.transform.forward, toTarget);
                if (spotAngleToPoint > lightSource.spotAngle * 0.5f)
                {
                    return false;
                }
            }
        }

        if (toTarget.sqrMagnitude <= 0.0001f)
        {
            return true;
        }

        Vector3 directionToTarget = toTarget.normalized;
        if (Physics.Raycast(origin, directionToTarget, out RaycastHit hit, maxDistance, occlusionMask, QueryTriggerInteraction.Ignore))
        {
            return hit.transform == transform || hit.transform.IsChildOf(transform);
        }

        return true;
    }

    private Vector3 GetTargetPoint()
    {
        if (cachedCollider != null)
        {
            return cachedCollider.bounds.center;
        }

        return transform.position;
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