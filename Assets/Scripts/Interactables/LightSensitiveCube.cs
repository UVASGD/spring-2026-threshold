using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class LightSensitiveCube : MonoBehaviour
{
    [SerializeField] AudioClip sizzlingSFX;
    [SerializeField] AudioClip screamSFX;
    private AudioSource objectSource;
    void Awake()
    {
        objectSource = GetComponent<AudioSource>();
    }
    public void onLightEnter()
    {
        if(sizzlingSFX != null)
            objectSource.PlayOneShot(sizzlingSFX);
        
        if(screamSFX != null)
            objectSource.PlayOneShot(screamSFX);
    }
    public void onLightExit()
    {
        
    }
}