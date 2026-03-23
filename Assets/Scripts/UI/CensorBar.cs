using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CensorBar : MonoBehaviour
{
    [Header("Prefabs & UI")]
    public GameObject censorUIPrefab;
    public Transform canvasTransform;
    public string targetTag = "Censored";

    [Header("Settings")]
    public float jitterIntensity = 8f;
    public float padding = 1.1f;

    private class CensorLink
    {
        public GameObject rootObject;
        public Renderer renderer;
        public RectTransform uiRect;
        public CanvasGroup group;
    }

    private List<CensorLink> _activeCensors = new List<CensorLink>();

    void Start()
    {
        foreach (GameObject t in GameObject.FindGameObjectsWithTag(targetTag))
        {
            Renderer r = t.GetComponentInChildren<Renderer>();
            if (r != null) CreateCensorUI(t, r);
        }
    }

    void CreateCensorUI(GameObject target, Renderer renderer)
    {
        GameObject newUI = Instantiate(censorUIPrefab, canvasTransform);
        CanvasGroup cg = newUI.GetComponent<CanvasGroup>() ?? newUI.AddComponent<CanvasGroup>();

        _activeCensors.Add(new CensorLink
        {
            rootObject = target,
            renderer = renderer,
            uiRect = newUI.GetComponent<RectTransform>(),
            group = cg
        });
    }

    void LateUpdate()
    {
        foreach (var link in _activeCensors)
        {
            if (link.rootObject == null || link.renderer == null) continue;

            Bounds bounds = link.renderer.bounds;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(bounds.center);

            // 1. Basic Frustum Check (Is it in front of camera?)
            bool isVisible = screenPos.z > 0;

            // 2. Line-of-Sight Check (Is it behind a wall?)
            if (isVisible)
            {
                // Check if the center is visible
                if (Physics.Linecast(Camera.main.transform.position, bounds.center, out RaycastHit hit))
                {
                    // If we hit something that isn't the target, it's occluded
                    if (hit.transform != link.rootObject.transform && !hit.transform.IsChildOf(link.rootObject.transform))
                    {
                        isVisible = false;
                    }
                }
            }

            // 3. Instant Snap Logic (No MoveTowards/Fading)
            if (isVisible)
            {
                link.uiRect.gameObject.SetActive(true);
                link.group.alpha = 1f;

                // Position and Scaling
                Rect screenRect = GetScreenRectFromBounds(bounds);
                Vector2 jitter = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * jitterIntensity;

                link.uiRect.position = screenRect.center + jitter;
                link.uiRect.sizeDelta = new Vector2(screenRect.width, screenRect.height) * padding;
                link.uiRect.localRotation = Quaternion.Euler(0, 0, Random.Range(-2f, 2f));
            }
            else
            {
                // Instant off
                link.group.alpha = 0f;
                link.uiRect.gameObject.SetActive(false);
            }
        }
    }

    private Rect GetScreenRectFromBounds(Bounds bounds)
    {
        Vector3 cen = bounds.center;
        Vector3 ext = bounds.extents;
        Vector3[] corners = new Vector3[] {
            new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z),
            new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z),
            new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z),
            new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z),
            new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z),
            new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z),
            new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z),
            new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z)
        };

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (Vector3 corner in corners)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(corner);
            minX = Mathf.Min(minX, screenPoint.x);
            minY = Mathf.Min(minY, screenPoint.y);
            maxX = Mathf.Max(maxX, screenPoint.x);
            maxY = Mathf.Max(maxY, screenPoint.y);
        }

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}