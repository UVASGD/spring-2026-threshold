using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class serves to move + rotate the camera as needed for the title screen
/// </summary>
public class Move_RotateCamera : MonoBehaviour
{
    [SerializeField] Camera movedCamera;
    [SerializeField] AnimationCurve lerpCurve; //for smoother animated linear interpolation
    Coroutine activeMove;

    [Header("Credits Position")]
    [SerializeField] Transform creditsPosition;
    [SerializeField] Transform lookAtCredits;
    [SerializeField] float lerpTime;

    [Header("Default Position")]
    [SerializeField] Transform homePosition;
    [SerializeField] Transform lookAtHome;
    public void MoveCameraCredits()
    {
        Debug.Log("Move to Credits button clicked");

        StartMove(new CameraMovementData(lookAtCredits, creditsPosition, lerpTime));
    }

    public void MoveCameraHome()
    {
        Debug.Log("Move to Home button clicked");

        StartMove(new CameraMovementData(lookAtHome, homePosition, lerpTime));
    }

    void StartMove(CameraMovementData movementData)
    {
        if (activeMove != null)
        {
            StopCoroutine(activeMove);
        }

        activeMove = StartCoroutine(MoveCamera(movementData));
    }

    public IEnumerator MoveCamera(CameraMovementData movementData)
    {
        if (movedCamera == null || movementData == null || movementData.targetPosition == null || movementData.toFace == null)
        {
            yield break;
        }

        if (movementData.lerpTime <= 0f)
        {
            movedCamera.transform.position = movementData.targetPosition.position;
            Vector3 instantForward = movementData.toFace.position - movedCamera.transform.position;
            if (instantForward.sqrMagnitude > 0.0001f)
            {
                movedCamera.transform.rotation = Quaternion.LookRotation(instantForward.normalized);
            }
            activeMove = null;
            yield break;
        }

        //get the differences between the current and target position/rotation and lerp both simultaneously
        float elapsedTime = 0f;
        Vector3 startPos = movedCamera.transform.position;
        Quaternion startRot = movedCamera.transform.rotation;
        while (elapsedTime < movementData.lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpPercent = elapsedTime / movementData.lerpTime;

            float curveValue = lerpCurve != null ? lerpCurve.Evaluate(lerpPercent) : lerpPercent;
            Vector3 currentPos = Vector3.Lerp(startPos, movementData.targetPosition.position, curveValue);
            movedCamera.transform.position = currentPos;

            Vector3 lookDirection = movementData.toFace.position - currentPos;
            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion currentTargetRot = Quaternion.LookRotation(lookDirection.normalized);
                movedCamera.transform.rotation = Quaternion.Slerp(startRot, currentTargetRot, curveValue);
            }

            yield return null;
        }

        //snap to final position and ensure final look direction is exact
        movedCamera.transform.position = movementData.targetPosition.position;
        Vector3 finalForward = movementData.toFace.position - movedCamera.transform.position;
        if (finalForward.sqrMagnitude > 0.0001f)
        {
            movedCamera.transform.rotation = Quaternion.LookRotation(finalForward.normalized);
        }

        activeMove = null;

        //final yield return statement
        yield return null;
    }

    [SerializeField] string introSceneName;
    public void LoadIntroScene()
    {
        StartCoroutine(startNewGame());
    }
    private IEnumerator startNewGame()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(introSceneName);
    }
}
/// <summary>
/// This class summarizes what's necessary to lerp a camera both by movement and looking at another object.
/// </summary>
[System.Serializable]
public class CameraMovementData
{
    public Transform toFace;
    public Transform targetPosition;
    public float lerpTime;

    public CameraMovementData(Transform toFace, Transform targetPosition, float lerpTime)
    {
        this.toFace = toFace;
        this.targetPosition = targetPosition;
        this.lerpTime = lerpTime;
    }
}