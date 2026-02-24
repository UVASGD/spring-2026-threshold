using System.Collections;
using TMPro;
using UnityEngine;

public class AreaTextPopup : MonoBehaviour
{
    [SerializeField] AnimationCurve lerpCurve;
    [SerializeField] float fadeDuration;
    public static AreaTextPopup i;
    
    private TMP_Text tmpText;
    private RectTransform thisRect;

    void Awake()
    {
        if (i == null)
            i = this;

        tmpText = GetComponent<TMP_Text>();
        thisRect = GetComponent<RectTransform>();
    }
    public IEnumerator showAreaText(string areaName)
    {
        yield return FadeOut();

        //then wait for a second
        yield return new WaitForSeconds(0.5f);

        yield return FadeIn();
        
        yield return null;
    }

    public void updateText(string text)
    {
        tmpText.text = text;
    }

    public IEnumerator FadeIn()
    {
        // Ensure the text starts fully transparent (alpha 0)
        tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 0f);
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Calculate new alpha value using Mathf.Lerp or directly
            float newAlpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, newAlpha);
            yield return null; // Wait until the next frame
        }
        // Ensure the text is fully opaque at the end
        tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 1f);
    }

    public IEnumerator FadeOut()
    {
        // Ensure the text starts fully opaque (alpha 1)
        tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 1f);
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Calculate new alpha value
            float newAlpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, newAlpha);
            yield return null; // Wait until the next frame
        }
        // Ensure the text is fully transparent at the end
        tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 0f);
        // Optional: Deactivate the GameObject after fading out
        // gameObject.SetActive(false);
    }
}
