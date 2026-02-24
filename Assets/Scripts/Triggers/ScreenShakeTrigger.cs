using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ScreenShakeTrigger : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] float intensity;
    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(CameraShake.i.cameraShake(duration, intensity));
    }
}