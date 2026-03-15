using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpObjectEffect : TriggerEffect
{
    [SerializeField] List<GameObject> toLerp;
    [SerializeField] Vector3 relativePosition; //not absolute to prevent objects from meshing into one another
    [SerializeField] float duration;
    [SerializeField] AnimationCurve lerpCurve;
    public override IEnumerator onTriggerEffect()
    {
        List<Vector3> originalPositions = new List<Vector3>();
        List<Vector3> targetPositions = new List<Vector3>();
        
        foreach(var obj in toLerp)
        {
            originalPositions.Add(obj.transform.position); //store each initial position
            targetPositions.Add(obj.transform.position + relativePosition); //determine each transform's final position
        }

        float elapsed = 0f;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;

            //lerp each object to its final position according to the lerpCurve
            for(int i = 0; i < toLerp.Count; i++)
            {
                toLerp[i].transform.position = Vector3.Lerp(originalPositions[i], targetPositions[i], lerpCurve.Evaluate(elapsed / duration));
            }

            yield return null;
        }
    }
}