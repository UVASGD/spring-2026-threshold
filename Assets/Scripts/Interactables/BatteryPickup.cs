using System;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class BatteryPickup : MonoBehaviour, IInteract, ISFXGenerator
{
    public static event Action onBatteryPickup;
    [SerializeField] AudioClip pickupClip;
    public void OnPlayerInteract()
    {
        if(Flashlight.i == null) return;
        Debug.Log("Player flashlight battery topped off");
        Flashlight.i?.TopOffBattery();
        PlayLocalSFX(pickupClip);
        onBatteryPickup?.Invoke(); //invoke the batterypickup event if there are listeners
        
        Destroy(this.gameObject);
    }

    public void PlayLocalSFX(AudioClip clip)
    {
        PlayerSFX.i.PlaySFX(clip);
    }
}