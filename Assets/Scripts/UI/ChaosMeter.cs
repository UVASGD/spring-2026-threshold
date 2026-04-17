using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ChaosMeter : MonoBehaviour
{
    public static ChaosMeter i;
    private Slider slider; //slider is configured to have a maximum value of 1000.
    private bool hasBeenRevealed;
    [SerializeField] TMP_Text stateText;

    [SerializeField] List<Color> colors;

    //slider threshold (heh) values are:
    // <250 (low, subtle hostility)
    // <750 (medium, objects begin to fall)
    // >750 (high, openly hostile, walls spout and warp)

    void Awake()
    {
        if(i==null) i = this;

        slider = GetComponent<Slider>();
        gameObject.SetActive(false);
    }

    public void updateChaosMeter(float newValue)
    {
        if(!hasBeenRevealed && newValue > 10f)
        {
            hasBeenRevealed = true;
            gameObject.SetActive(true);
        }

        if(!hasBeenRevealed)
        {
            return;
        }

        newValue = Mathf.Min(newValue, 1000);
        slider.value = newValue; //update the UI.

        //TODO: Make sure to update any text and color of text
        switch (newValue)
        {
            case < 250f:
                stateText.text = "low chaos";
                updateColors(0);
                break;
            case < 750f:
                stateText.text = "medium chaos";
                updateColors(1);
                break;
            default: //otherwise it is higher than 750, and therefore at high level
                stateText.text = "high chaos";
                updateColors(2);
                break;
        }
    }    

    private void updateColors(int threshold)
    {
        stateText.color = colors[threshold];
    }
}