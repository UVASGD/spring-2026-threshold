using UnityEngine;

public class TitleScreenTheme : MonoBehaviour
{
    [SerializeField] AudioClip titleTheme;
    [SerializeField] AudioSource themeSource;

    void Start()
    {
        themeSource.clip = titleTheme;
        themeSource.volume = 1f;
        themeSource.Play();
    }
}
