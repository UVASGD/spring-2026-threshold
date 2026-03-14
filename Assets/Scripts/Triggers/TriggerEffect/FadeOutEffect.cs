using System.Collections;
using UnityEngine;

public class FadeOutEffect : TriggerEffect
{
    [SerializeField] float duration;
    [SerializeField] bool fadeOut;
    public override IEnumerator onTriggerEffect()
    {
        if (fadeOut)
        {
            yield return Fader.i.fadeOut(duration);
        }
        else
        {
            yield return Fader.i.fadeIn(duration);
        }
    }
}