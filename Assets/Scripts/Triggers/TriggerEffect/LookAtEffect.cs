using System;
using System.Collections;
using UnityEngine;
public class LookAtEffect : TriggerEffect
{
    [Header("How long the lookAt takes")]
    [SerializeField] float lerpDuration;

    [Header("Lerp Curve changes the abruptness of the player's scripted view'")]
    [SerializeField] AnimationCurve lerpCurve;
    [Header("Gameobject the player is looking at")]
    [SerializeField] Transform lookAt;
    public override IEnumerator onTriggerEffect()
    {
        //lerp the camera towards the lookAt transform
        if (FirstPersonController.i != null && lookAt != null)
        {
            Transform cam = FirstPersonController.i.cameraTransform;
            Quaternion startRot = cam.rotation;
            Quaternion targetRot = Quaternion.LookRotation(lookAt.position - cam.position);

            float elapsed = 0f;
            while (elapsed < lerpDuration)
            {
                elapsed += Time.deltaTime;
                cam.rotation = Quaternion.Slerp(startRot, targetRot, lerpCurve.Evaluate(elapsed / lerpDuration));
                yield return null;
            }

            cam.rotation = targetRot; // Snap to final rotation
            FirstPersonController.i.SetCameraRotation(targetRot);
        }

        yield return null;
    }
}