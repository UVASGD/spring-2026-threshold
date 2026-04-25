using System.Collections;
using UnityEngine;

public class ColorTintEffect : TriggerEffect
{
    [SerializeField] Color tintColor;
    [SerializeField] bool enable;
    public override IEnumerator onTriggerEffect()
    {
       ColorTint.i.enableColorTint(enable);
       ColorTint.i.setTintColor(tintColor);

       yield return null;
    }
}