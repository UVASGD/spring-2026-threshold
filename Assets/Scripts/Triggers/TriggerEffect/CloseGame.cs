using System.Collections;
using UnityEngine;

public class CloseGame : TriggerEffect
{
    public override IEnumerator onTriggerEffect()
    {
        Application.Quit(); //close the game when this trigger is called.
        yield return null;
    }
}