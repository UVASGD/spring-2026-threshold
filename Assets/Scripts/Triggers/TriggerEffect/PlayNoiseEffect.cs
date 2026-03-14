using System;
using System.Collections;
using UnityEngine;

public class PlayNoiseEffect : TriggerEffect
{
    [SerializeField] AudioClip clip;

    [Header("Volume: Leave blank for default value")]
    [SerializeField] float volume;
    public override IEnumerator onTriggerEffect()
    {
        if(volume != 0)
            PlayerSFX.i.PlaySFX(clip);
        else
            PlayerSFX.i.PlaySFX(clip, volume); //call the overload with a volume value

        
        yield return null;
    }
}