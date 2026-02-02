using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class FlashlightPickup : MonoBehaviour, IInteract, ISFXGenerator
{
    [SerializeField] AudioClip pickupSFX;
    public void OnPlayerInteract()
    {
        //give the player access to the flashlight
    }

    public void PlayLocalSFX(AudioClip clip)
    {
        //play the local sfx
    }
}
