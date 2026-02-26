using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LerpableCamera : MonoBehaviour
{
    //lerpable camera allows for the camera to lerp outside of the player's body for certain scriptable scenarios
    private Vector3 originalLocalPosition;
    private Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    public void setOriginalCamPosition()
    {
        originalLocalPosition = gameObject.transform.position;
    }
    public void restoreOriginalCamPosition()
    {
        gameObject.transform.position = originalLocalPosition;
    }
    public IEnumerator lerpCamToPosition(Vector3 targetPosition, Transform lookAt, float lerpTime, float pauseTime, float returnTime)
    {
        //lerps a camera to a desired position, and looks at the desired object
        //probably made really easy by cinemachine, but too late at this point to implement

        setOriginalCamPosition(); //store the original positions used
        
        float elapsedTime = 0f;
        while(elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            transform.LookAt(lookAt);
            transform.position = Vector3.Lerp(originalLocalPosition, targetPosition, elapsedTime / lerpTime);

            yield return null;
        }

        yield return new WaitForSeconds(pauseTime);

        //now lerp back
        elapsedTime = 0f;
        while(elapsedTime < returnTime)
        {
            elapsedTime += Time.deltaTime;
            transform.LookAt(lookAt);
            transform.position = Vector3.Lerp(targetPosition, originalLocalPosition, elapsedTime / returnTime);
        }

        //now restore original camera position
        restoreOriginalCamPosition();
    }
}