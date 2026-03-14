using System.Collections;
using UnityEngine;

public class TeleportPlayerEffect : TriggerEffect
{
    [SerializeField] Vector3 targetPosition;
    public override IEnumerator onTriggerEffect()
    {
        if(FirstPersonController.i != null)
        {
            FirstPersonController.i.gameObject.transform.position = targetPosition;
        }

        yield return null;
    }
}