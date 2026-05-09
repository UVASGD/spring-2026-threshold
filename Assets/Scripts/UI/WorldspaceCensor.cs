using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldspaceCensor : MonoBehaviour
{
    public string targetTag = "Censored";
    public Material censorMaterial; 
    
    [Header("Scaling & Position")]
    public float paddingMultiplier = 1.1f;
    public float surfaceOffset = 0.05f; 

    [Header("Style & Animation")]
    [Range(0f, 0.5f)] 
    public float jitterAmount = 0.05f;
    public float rotationJitter = 5.0f;

    private class CensorSticker {
        public Transform target;
        public Renderer targetRenderer;
        public GameObject stickerObject;
    }

    private List<CensorSticker> _stickers = new List<CensorSticker>();

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RebuildStickers();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ClearStickers();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebuildStickers();
    }

    private void RebuildStickers()
    {
        ClearStickers();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(targetTag))
        {
            Renderer r = obj.GetComponentInChildren<Renderer>();
            if (r == null) continue;

            GameObject sticker = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(sticker.GetComponent<MeshCollider>());

            Renderer stickerRenderer = sticker.GetComponent<Renderer>();
            if (stickerRenderer != null)
                stickerRenderer.material = censorMaterial;

            sticker.transform.SetParent(obj.transform);

            _stickers.Add(new CensorSticker {
                target = obj.transform,
                targetRenderer = r,
                stickerObject = sticker
            });
        }
    }

    private void ClearStickers()
    {
        foreach (var s in _stickers)
        {
            if (s != null && s.stickerObject != null)
                Destroy(s.stickerObject);
        }

        _stickers.Clear();
    }

    void LateUpdate()
    {
        if (Camera.main == null) return;

        Transform camTrans = Camera.main.transform;
        Vector3 camRight = camTrans.right;
        Vector3 camUp = camTrans.up;

        foreach (var s in _stickers)
        {
            if (s.target == null) continue;

            Bounds b = s.targetRenderer.bounds;
            
            // 1. Position: Push toward camera
            Vector3 dirToCamera = (camTrans.position - b.center).normalized;
            float pushDist = b.extents.magnitude + surfaceOffset;
            s.stickerObject.transform.position = b.center + (dirToCamera * pushDist);

            // 2. Dynamic Axis Scaling
            // We project the 8 corners of the bounds onto the camera's Right and Up axes
            Vector2 size = GetRequiredSizeOnCameraAxes(b, camRight, camUp);
            s.stickerObject.transform.localScale = new Vector3(size.x, size.y, 1f) * paddingMultiplier;

            // 3. Face Camera & Flip
            s.stickerObject.transform.LookAt(camTrans.position);
            s.stickerObject.transform.Rotate(0, 180, 0); 
            
            // 4. Jitter & Rotation
            if (jitterAmount > 0)
                s.stickerObject.transform.position += Random.insideUnitSphere * jitterAmount;

            if (rotationJitter > 0)
                s.stickerObject.transform.Rotate(0, 0, Random.Range(-rotationJitter, rotationJitter));
        }
    }

    private Vector2 GetRequiredSizeOnCameraAxes(Bounds b, Vector3 camRight, Vector3 camUp)
    {
        Vector3 cen = b.center;
        Vector3 ext = b.extents;
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

        float minRight = float.MaxValue, maxRight = float.MinValue;
        float minUp = float.MaxValue, maxUp = float.MinValue;

        foreach (Vector3 corner in corners)
        {
            // Project the world corner onto the camera's 2D axes
            float dotRight = Vector3.Dot(corner - cen, camRight);
            float dotUp = Vector3.Dot(corner - cen, camUp);

            minRight = Mathf.Min(minRight, dotRight);
            maxRight = Mathf.Max(maxRight, dotRight);
            minUp = Mathf.Min(minUp, dotUp);
            maxUp = Mathf.Max(maxUp, dotUp);
        }

        return new Vector2(maxRight - minRight, maxUp - minUp);
    }
}