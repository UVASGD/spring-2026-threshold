using System;
using System.Collections;
using UnityEngine;

public class RemovePlayerMovementEffect : TriggerEffect
{
    [SerializeField] bool enable;

    public override IEnumerator onTriggerEffect()
    {
        if(FirstPersonController.i != null)
        {
            FirstPersonController.i.changePlayerControlState(enable);
        }

        yield return null;
    }
}