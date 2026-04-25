using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager i;
    [SerializeField] AudioSource bgmSource;
    private AudioClip currentBGM;
    [SerializeField] float fadeTime = 1f; //one second to fade between songs
    private Coroutine fadeRoutine;
    void Awake()
    {
        if(i==null) i = this;
    }
    public void changeBPM(AudioClip newBGM, bool fade = false)
    {
        if (bgmSource == null || newBGM == null)
        {
            return;
        }
        if (currentBGM == newBGM)
        {
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        if (!fade || fadeTime <= 0f)
        {
            currentBGM = newBGM;
            bgmSource.clip = newBGM;
            bgmSource.volume = 1f;
            bgmSource.Play();
            return;
        }

        fadeRoutine = StartCoroutine(FadeToClip(newBGM));
    }

    private IEnumerator FadeToClip(AudioClip newBGM)
    {
        float startVolume = bgmSource.volume;
        float timer = 0f;

        // fade out current track
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTime);
            yield return null;
        }

        bgmSource.volume = 0f;
        currentBGM = newBGM;
        bgmSource.clip = newBGM;
        bgmSource.Play();

        // fade in new track
        timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, startVolume <= 0f ? 1f : startVolume, timer / fadeTime);
            yield return null;
        }

        bgmSource.volume = startVolume <= 0f ? 1f : startVolume;
        fadeRoutine = null;
    }

    public void pauseMusic()
    {
        bgmSource.Pause();
    }
    public void resumeMusic()
    {
        bgmSource.Play();
    }
}
