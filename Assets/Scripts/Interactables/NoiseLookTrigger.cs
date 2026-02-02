using System;
using UnityEngine;

public class NoiseLookTrigger : MonoBehaviour, ILookable
{
    [SerializeField] AudioClip sfx;
    public void OnLookEnter()
    {
       Debug.Log("Audio-look trigger hit");
       PlayerSFX.i.PlaySFX(sfx);
       Destroy(this.gameObject);
    }

    public void OnLookExit()
    {
        //do nothing
    }
}
