using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-100)]
public class EssentialObjects : MonoBehaviour
{
    public static EssentialObjects i;

    void Awake()
    {
        DontDestroyOnLoad(this);
        i = this;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        StartCoroutine(RefreshWorldSpaceCanvasesRoutine());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RefreshWorldSpaceCanvasesRoutine());
    }

    private IEnumerator RefreshWorldSpaceCanvasesRoutine()
    {
        // Let scene objects initialize so Camera.main and canvases are valid.
        yield return null;

        Camera mainCamera = Camera.main;
        if (mainCamera == null) yield break;

        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            if (canvas == null) continue;
            if (canvas.renderMode != RenderMode.WorldSpace) continue;

            if (canvas.worldCamera == null || !canvas.worldCamera.isActiveAndEnabled)
            {
                canvas.worldCamera = mainCamera;
            }
        }
    }
}
