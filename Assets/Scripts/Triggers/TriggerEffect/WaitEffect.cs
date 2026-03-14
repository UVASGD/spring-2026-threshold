using System.Collections;
using UnityEngine;

public class WaitEffect : TriggerEffect
{
    [SerializeField] float duration;
    public override IEnumerator onTriggerEffect()
    {
        yield return new WaitForSeconds(duration); //wait for the desired duration
    }
}