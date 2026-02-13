using System.Collections.Generic;
using UnityEngine;

public class KeyPad : MonoBehaviour
{
    int[] keyValues = {0,0,0,0}; 
    void Awake()
    {
        KeyCap.onKeycapPress += handleKeycapPress;
    }

    private void handleKeycapPress(int keypadID, int keyValue)
    {
        
    }
}