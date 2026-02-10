using UnityEngine;
using System;
public class InteractableNote : MonoBehaviour, IInteract
{
    public static event Action<string> onNoteInteract;
    [SerializeField] string noteText;
    public void OnPlayerInteract(){
        onNoteInteract.Invoke(noteText);
        Debug.Log("Note has been interacted with by player");
    }
}