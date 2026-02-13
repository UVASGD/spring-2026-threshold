using System;
using UnityEngine;

public class KeyCap : MonoBehaviour, IInteract
{
    [SerializeField] int keypadID;
    [SerializeField] int keyPressValue;
    public static event Action<int, int> onKeycapPress;

    public void OnPlayerInteract()
    {
       onKeycapPress.Invoke(keypadID, keyPressValue);
    }
}