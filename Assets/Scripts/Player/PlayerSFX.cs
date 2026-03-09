using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class PlayerSFX : MonoBehaviour
{
    private AudioSource playerSource;
    public static PlayerSFX i;
    void Awake()
    {
        playerSource = GetComponent<AudioSource>();
        if(i == null)
        {
            i = this;
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        playerSource.PlayOneShot(clip);
    }
    public void PlaySFX(AudioClip clip, float volume)
    {
        float preVolume = playerSource.volume;
        playerSource.volume = volume;
        
        playerSource.PlayOneShot(clip);

        playerSource.volume = preVolume; //reset the volume
    }
}