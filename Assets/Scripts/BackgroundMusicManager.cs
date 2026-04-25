using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [SerializeField] AudioListener bgmListener;
    private AudioClip currentBGM;
    [SerializeField] float fadeTime = 1f; //one second to fade between songs
    public void changeBPM(AudioClip newBGM, bool fade = false)
    {
        
    }
    public void stopMusic()
    {
        
    }
    public void resumeMusic()
    {
        
    }
}
