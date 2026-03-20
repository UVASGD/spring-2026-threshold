using System.Collections;
using UnityEngine;

public class DropPhysicsEffect : TriggerEffect
{
    //this trigger type causes the physics grabber to drop its current object to the floor.
    public override IEnumerator onTriggerEffect()
    {
        PhysicsGrabber.i.externalDrop();

        yield return null;
    }
}