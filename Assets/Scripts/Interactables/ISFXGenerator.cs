using UnityEngine;

public interface ISFXGenerator //interface for world objects that play sound effects from their own audiosource
{
    public void PlayLocalSFX(AudioClip clip);
}