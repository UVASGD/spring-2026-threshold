using UnityEngine;
using System;
public class InteractableNote : MonoBehaviour, IInteract
{
    public static event Action<string> onNoteInteract;
    [TextArea(15,20)]
    [SerializeField] string noteText;
    public void OnPlayerInteract(){
        onNoteInteract.Invoke(noteText);
        Debug.Log("Note has been interacted with by player");
    }
}