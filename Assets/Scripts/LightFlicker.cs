using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    //randomly flicker a light x times after an also set time

    [SerializeField] int flickerTimes = 5;
    [SerializeField] float timeBetweenFlickers = 15f;

    private Light associatedLight;
    void Awake()
    {
        associatedLight = GetComponent<Light>();
    }
    void Start()
    {
        StartCoroutine(flickerTimer());
    }
    const float flickerTime = 0.1f;
    public IEnumerator flickerLights()
    {
        if (associatedLight == null) yield break; //null check for the light component

        float originalIntensity = associatedLight.intensity;

        for (int i = 0; i < flickerTime; i++)
        {
            associatedLight.intensity = 0f;
            yield return new WaitForSeconds(flickerTime);
            associatedLight.intensity = originalIntensity;
            yield return new WaitForSeconds(flickerTime);
        }

        yield break; //break at the end of the coroutine
    }

    public IEnumerator flickerTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(flickerTimes);
            flickerLights();
        }
    }
}