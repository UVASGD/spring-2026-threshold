using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This class is a wrapper for the PlayerHP script, and renders at a different color depending
/// on how depleted the playerHP is.
/// </summary>
public class HealthBar : MonoBehaviour
{
    public static HealthBar i;
    private Slider barSlider;
    private float maxHealth;
    private float currentHealth;
    void Awake()
    {
        if(i==null) i = this;
        barSlider = GetComponent<Slider>();
    }

    public void setMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        barSlider.maxValue = maxHealth;
    }

    public void updateCurrentHealth(float newHealth)
    {
        if(newHealth > maxHealth)
        {
            Debug.Log("New health value is at max. Setting to max value.");
            newHealth = maxHealth;
        }

        this.currentHealth = newHealth;
        barSlider.value = newHealth;

        updateHealthColor();
    }

    private void updateHealthColor()
    {
        //lerp between a dark red and dark green color for the player health, so it progressively gets more and more red as damage is accrued.
        float healthPercentage = currentHealth / maxHealth;
        barSlider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, healthPercentage);
        //more health = more green bar fill color
    }
}
