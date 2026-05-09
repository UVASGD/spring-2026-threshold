using System.Collections;
using UnityEngine;

public class InstantDeathEffect : TriggerEffect
{
    public override IEnumerator onTriggerEffect()
    {
        if(FirstPersonController.i != null)
        {
            PlayerHP playerHP = FirstPersonController.i.GetComponentInChildren<PlayerHP>(); //not optimal I know
            if(playerHP != null)
            {
                playerHP.forceKill();
            }
        }

        yield return null;
    }
}