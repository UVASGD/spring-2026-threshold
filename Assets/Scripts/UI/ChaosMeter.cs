using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ChaosMeter : MonoBehaviour
{
    public static ChaosMeter i;
    private Slider slider; //slider is configured to have a maximum value of 1000.
    [SerializeField] TMP_Text stateText;

    //slider threshold (heh) values are:
    // <250 (low, subtle hostility)
    // <750 (medium, objects begin to fall)
    // >750 (high, openly hostile, walls spout and warp)

    void Awake()
    {
        if(i==null) i = this;

        slider = GetComponent<Slider>();
    }

    public void updateChaosMeter(float newValue)
    {
        slider.value = newValue; //update the UI.

        //TODO: Make sure to update any text and color of text
        
    }    
}