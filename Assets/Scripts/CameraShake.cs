using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake i;
    void Awake()
    {
        i=this;
    }
    public IEnumerator cameraShake(float duration, float intensity)
    {
        Debug.Log("Camera shake starting");

        Vector3 localPos = transform.localPosition;

        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            float x = Random.Range(-0.1f,0.1f) * intensity;
            float y = Random.Range(-0.1f,0.1f) * intensity;
            transform.localPosition += new Vector3(x,y,0);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        //return to original position at the end

        transform.localPosition = localPos;
    }
}
