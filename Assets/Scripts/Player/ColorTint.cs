using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorTint : MonoBehaviour
{
    public static ColorTint i;
    [SerializeField] Volume volume;
    [SerializeField] Color tintColor = Color.white;

    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        if(i==null) i = this;
        CacheColorAdjustments();
    }

    public void enableColorTint(bool enable)
    {
        if (volume == null) return;
        volume.enabled = enable;
    }

    public void setTintColor()
    {
        setTintColor(tintColor);
    }

    public void setTintColor(Color newTintColor)
    {
        tintColor = newTintColor;

        if (!CacheColorAdjustments())
        {
            return;
        }

        colorAdjustments.active = true;
        colorAdjustments.colorFilter.value = tintColor;
    }

    private bool CacheColorAdjustments()
    {
        if (volume == null || volume.profile == null)
        {
            return false;
        }

        if (colorAdjustments != null)
        {
            return true;
        }

        if (!volume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments = volume.profile.Add<ColorAdjustments>(true);
        }

        return colorAdjustments != null;
    }
}