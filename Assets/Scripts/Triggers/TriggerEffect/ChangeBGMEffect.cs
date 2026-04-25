using System;
using System.Collections;
using UnityEngine;

public class ChangeBGMEffect : TriggerEffect
{
    [SerializeField] AudioClip newBGM;
    [SerializeField] bool fade;
    public override IEnumerator onTriggerEffect()
    {
        BackgroundMusicManager.i.changeBPM(newBGM, fade);
        yield return null;
    }
}