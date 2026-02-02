using UnityEngine;
[RequireComponent(typeof(Collider))]
public class SFXTrigger : MonoBehaviour, ISFXGenerator
{
    [SerializeField] AudioClip clip;
    [SerializeField] bool destroy = true;

    public void PlayLocalSFX(AudioClip clip)
    {
        PlayerSFX.i.PlaySFX(clip);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayLocalSFX(clip);
        if(destroy) Destroy(this.gameObject);
    }
}
