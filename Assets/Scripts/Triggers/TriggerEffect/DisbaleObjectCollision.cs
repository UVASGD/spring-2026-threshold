using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisbaleObjectCollision : TriggerEffect
{
    [SerializeField] List<Collider> objectColliders;
    [SerializeField] bool enable; //whether or not to enable the colliders
    public override IEnumerator onTriggerEffect()
    {
        foreach(var collider in objectColliders)
        {
            collider.enabled = this.enable;
        }

        yield return null;
    }
}