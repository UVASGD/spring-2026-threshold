using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class Fader : MonoBehaviour
{
    public static Fader i;

    private Image lerpImage;

    void Awake()
    {
        if(i==null)
            i = this;
        
        lerpImage = GetComponent<Image>();
    }
    //used on the title screen when starting a new game
    public void fadeOutNonCoroutine()
    {
        StartCoroutine(fadeOut(2f));
    }
    //fadeout/fadein transitions
    public IEnumerator fadeOut(float lerpTime)
    {
        //lerp the alpha of the black image to 1
        float elapsedTime = 0;
        float lerpPercentage;
        while(elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            lerpPercentage = elapsedTime / lerpTime;
            lerpImage.color = new Color(lerpImage.color.r, lerpImage.color.g, lerpImage.color.b, lerpPercentage);

            yield return null;
        }

        yield return null;
    }
    public IEnumerator fadeIn(float lerpTime)
    {
        //lerp the alpha of the black image to zero
        //lerp the alpha of the black image to 1
        float elapsedTime = 0;
        float lerpPercentage;
        while(elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            lerpPercentage = elapsedTime / lerpTime;
            lerpImage.color = new Color(lerpImage.color.r, lerpImage.color.g, lerpImage.color.b, 1 - lerpPercentage);

            yield return null;
        }

        yield return null;
    }
    public void updateFaderColor(Color newColor)
    {
        lerpImage.color = newColor;
    }
    public void resetFaderColor()
    {
        lerpImage.color = Color.black;
    }
}