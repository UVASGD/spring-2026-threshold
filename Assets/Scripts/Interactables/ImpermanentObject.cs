using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Impermanent objects have different properties when they are not illuminated.
/// </summary>
public class ImpermanentObject : MonoBehaviour
{
    [SerializeField] UnityAction onUniluminated;
    [SerializeField] UnityAction onIlumination;
    public void togglePermanence(bool illuminated)
    {
        if (illuminated)
        {
            onIlumination?.Invoke();
        }
        else
        {
            onUniluminated?.Invoke();
        }
    }   
}