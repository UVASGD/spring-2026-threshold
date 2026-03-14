using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> playerHands;
    [SerializeField] int playerHandID = 0;
    public int PlayerHandID => playerHandID;

    [Header("Raise Animation")]
    [SerializeField] float raiseOffset = 0.3f;
    [SerializeField] float raiseDuration = 0.2f;

    List<Vector3> originalLocalPositions;
    List<Quaternion> originalLocalRotations;
    List<Vector3> currentRaiseOffsets;
    Coroutine raiseCoroutine;

    [Header("Camera Reference")]
    [SerializeField] Transform cameraReference;

    [Header("Arm Bob")]
    [SerializeField] bool bobEnabled = true;
    [SerializeField] float bobAmplitude = 0.02f;
    [SerializeField] float bobFrequency = 6f;
    [SerializeField] float bobHorizontalAmplitude = 0.01f;
    Vector3 lastCameraPos;

    void Start()
    {
        CacheOriginalPositions();
        if (cameraReference != null) lastCameraPos = cameraReference.position;
        UpdateHands();
    }

    void CacheOriginalPositions()
    {
        originalLocalPositions = new List<Vector3>();
        originalLocalRotations = new List<Quaternion>();
        currentRaiseOffsets = new List<Vector3>();
        if (playerHands == null) return;
        foreach (var go in playerHands)
        {
            if (go != null)
            {
                originalLocalPositions.Add(go.transform.localPosition);
                originalLocalRotations.Add(go.transform.localRotation);
                currentRaiseOffsets.Add(Vector3.zero);
            }
            else
            {
                originalLocalPositions.Add(Vector3.zero);
                originalLocalRotations.Add(Quaternion.identity);
                currentRaiseOffsets.Add(Vector3.zero);
            }
        }
    }

    void Update()
    {
        if (playerHands == null || playerHands.Count == 0) return;

        var mouse = Mouse.current;
        if (mouse != null)
        {
            var scroll = mouse.scroll.ReadValue().y;
            if (!Mathf.Approximately(scroll, 0f))
            {
                if (scroll > 0f)
                {
                    // Scroll up -> previous hand
                    playerHandID = (playerHandID - 1 + playerHands.Count) % playerHands.Count;
                }
                else
                {
                    // Scroll down -> next hand
                    playerHandID = (playerHandID + 1) % playerHands.Count;
                }

                UpdateHands();
            }
        }

    }

    void UpdateHands()
    {
        if (playerHands == null || playerHands.Count == 0) return;

        for (int i = 0; i < playerHands.Count; i++)
        {
            var go = playerHands[i];
            if (go == null) continue;

            if (i == playerHandID)
            {
                if (!go.activeSelf) go.SetActive(true);

                Vector3 original = (originalLocalPositions != null && i < originalLocalPositions.Count)
                    ? originalLocalPositions[i]
                    : go.transform.localPosition;

                Vector3 from = original - Vector3.up * raiseOffset;

                // compute offsets relative to original position
                Vector3 fromOffset = from - original; // typically -up*raiseOffset
                Vector3 toOffset = Vector3.zero;

                if (raiseCoroutine != null) StopCoroutine(raiseCoroutine);
                raiseCoroutine = StartCoroutine(RaiseRoutine(i, fromOffset, toOffset, raiseDuration));
                // Apply rotation immediately so the hand faces roughly like the camera while raising
                if (cameraReference != null && originalLocalRotations != null && i < originalLocalRotations.Count)
                    go.transform.localRotation = originalLocalRotations[i];
            }
            else
            {
                // Reset position and deactivate
                if (originalLocalPositions != null && i < originalLocalPositions.Count)
                    go.transform.localPosition = originalLocalPositions[i];
                if (currentRaiseOffsets != null && i < currentRaiseOffsets.Count)
                    currentRaiseOffsets[i] = Vector3.zero;
                if (originalLocalRotations != null && i < originalLocalRotations.Count)
                    go.transform.localRotation = originalLocalRotations[i];
                if (go.activeSelf) go.SetActive(false);
            }
        }
    }

    IEnumerator RaiseRoutine(int index, Vector3 fromOffset, Vector3 toOffset, float duration)
    {
        if (currentRaiseOffsets == null || index < 0 || index >= currentRaiseOffsets.Count)
        {
            yield break;
        }

        float elapsed = 0f;
        currentRaiseOffsets[index] = fromOffset;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / duration);
            currentRaiseOffsets[index] = Vector3.Lerp(fromOffset, toOffset, k);
            yield return null;
        }
        currentRaiseOffsets[index] = toOffset;
        raiseCoroutine = null;
    }

    void LateUpdate()
    {
        if (!bobEnabled) return;
        if (playerHands == null || playerHands.Count == 0) return;
        int i = playerHandID;
        if (i < 0 || i >= playerHands.Count) return;
        var go = playerHands[i];
        if (go == null) return;
        if (originalLocalPositions == null || i >= originalLocalPositions.Count) return;

        // Use camera movement (if available) to scale bob so arms follow player movement without needing FirstPersonController
        float t = Time.time * bobFrequency;
        float moveScale = 1f;
        bool moving = true;
        if (cameraReference != null)
        {
            float dt = Time.deltaTime > 0f ? Time.deltaTime : (1f / 60f);
            float speed = (cameraReference.position - lastCameraPos).magnitude / dt;
            lastCameraPos = cameraReference.position;
            // map speed to a 0..1 scale (tweak divisor if needed)
            moveScale = Mathf.Clamp01(speed / 2f);
            moving = moveScale > 0.01f;
            t = Time.time * bobFrequency * (1f + moveScale);
        }

        float phase = i * 0.5f;
        float y = Mathf.Sin(t + phase) * bobAmplitude * moveScale * (moving ? 1f : 0f);
        float x = Mathf.Cos(t * 0.5f + phase) * bobHorizontalAmplitude * moveScale * (moving ? 1f : 0f);

        // Use original stored position as base (avoid accumulating offsets)
        Vector3 basePos = (originalLocalPositions != null && i < originalLocalPositions.Count)
            ? originalLocalPositions[i]
            : go.transform.localPosition;

        // Apply raise offset for smooth hand switching animation
        Vector3 raiseOffsetVec = (currentRaiseOffsets != null && i < currentRaiseOffsets.Count)
            ? currentRaiseOffsets[i]
            : Vector3.zero;

        go.transform.localPosition = basePos + raiseOffsetVec + new Vector3(x, y, 0f);
    }

    public void SetPlayerHand(int id)
    {
        if (playerHands == null || playerHands.Count == 0) return;
        playerHandID = Mathf.Clamp(id, 0, playerHands.Count - 1);
        UpdateHands();
    }
}
