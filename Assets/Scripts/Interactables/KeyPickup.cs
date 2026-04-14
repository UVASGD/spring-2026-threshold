using System;
using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteract
{
    [SerializeField] DoorKey key;
    [SerializeField] string pickupMessage;
    private const int messageTime = 1; //the amount of time the message is shown
    public void OnPlayerInteract()
    {
        Debug.Log($"Key ID {key.keyID} picked up");
        
        if (pickupMessage != "")
        {
            StartCoroutine(MessageManager.i.DisplayText(pickupMessage, messageTime));
        }
        PlayerKeys.i.KeyPickup(key);

        Destroy(this.gameObject);
    }
}