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
            lerpImage.color = new Color(0,0,0, lerpPercentage);

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
            lerpImage.color = new Color(0,0,0, 1 - lerpPercentage); //the 1 - lerpPercentage causes the alpha to decrease (alpha is a percentage / 1)

            yield return null;
        }

        yield return null;
    }
}