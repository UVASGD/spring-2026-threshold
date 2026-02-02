using System;
using UnityEngine;

public class MessageTrigger : MonoBehaviour, IInteract
{
    [SerializeField] string message;
    [SerializeField] int messageTime;
    public void OnPlayerInteract()
    {
        MessageManager.i.DisplayText(message, messageTime);
    }
}
