using System.Collections;
using UnityEngine;

public class EnableDisableGravityEffect : TriggerEffect
{
    [Header("Enable/disable gravity as needed from triggers")]
    [SerializeField] bool enable;
    public override IEnumerator onTriggerEffect()
    {
        if(FirstPersonController.i != null)
            FirstPersonController.i.toggleGravity(enable);

        yield return null;
    }
}
