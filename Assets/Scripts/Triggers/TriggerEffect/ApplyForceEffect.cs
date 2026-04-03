using System.Collections;
using UnityEngine;

public class ApplyForceEffect : TriggerEffect
{
    [SerializeField] Vector3 appliedForce;
    [SerializeField] Rigidbody target;
    public override IEnumerator onTriggerEffect()
    {
        target.AddForce(appliedForce, ForceMode.Impulse);
        yield return null;
    }
}