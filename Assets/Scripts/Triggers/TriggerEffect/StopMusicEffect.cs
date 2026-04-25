using System.Collections;
using UnityEngine;

public class StopMusicEffect : TriggerEffect
{
    [SerializeField] bool pause; //false if resuming, true if pausing

    public override IEnumerator onTriggerEffect()
    {
        if (pause)
        {
            BackgroundMusicManager.i.pauseMusic();
        }
        else
        {
            BackgroundMusicManager.i.resumeMusic();
        }

        yield return null;
    }
}