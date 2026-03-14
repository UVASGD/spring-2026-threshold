using System.Collections;
using UnityEngine;

public class CloseDoorEffect : TriggerEffect
{
    [SerializeField] OpenableDoor targetDoor;
    public override IEnumerator onTriggerEffect()
    {
        targetDoor.OnPlayerInteract();

        yield return null;
    }
}