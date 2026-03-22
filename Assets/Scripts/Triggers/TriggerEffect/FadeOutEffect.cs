using System.Collections;
using UnityEngine;

public class FadeOutEffect : TriggerEffect
{
    [SerializeField] float duration;
    [SerializeField] bool fadeOut;
    [SerializeField] Color faderColor = Color.black;
    public override IEnumerator onTriggerEffect()
    {
        Fader.i.updateFaderColor(faderColor);
        
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