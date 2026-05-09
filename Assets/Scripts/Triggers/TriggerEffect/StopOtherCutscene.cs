using System.Collections;
using UnityEngine;

public class StopOtherCutscene : TriggerEffect
{
    [SerializeField] DynamicTrigger cutsceneToStop;

    public override IEnumerator onTriggerEffect()
    {
        if(cutsceneToStop != null)
        {
            cutsceneToStop.StopEffects();
        }

        Debug.Log("Cutscene stopped");
        yield return null;
    }
}