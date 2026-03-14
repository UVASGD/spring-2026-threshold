using System;
using System.Collections;
using UnityEngine;

public class ShakeCameraEffect : TriggerEffect
{
    [SerializeField] float duration;
    [Header("Make sure intensity values are 0-0.2")]
    [SerializeField] float intensity;
    public override IEnumerator onTriggerEffect()
    {
        yield return CameraShake.i.cameraShake(duration, intensity);
    }
}