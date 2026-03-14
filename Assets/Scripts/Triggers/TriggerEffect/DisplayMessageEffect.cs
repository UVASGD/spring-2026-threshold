using System.Collections;
using UnityEngine;

public class DisplayMessageEffect : TriggerEffect
{
    [SerializeField] int duration;
    [SerializeField] string message;
    public override IEnumerator onTriggerEffect()
    {
        yield return MessageManager.i.DisplayText(message, duration);
    }
}