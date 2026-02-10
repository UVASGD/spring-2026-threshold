using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class NotePopup : MonoBehaviour, ISFXGenerator
{
    void Awake(){
        InteractableNote.onNoteInteract += updateNotePopupText;
    }
    [SerializeField] AudioClip noteSFX;
    [SerializeField] TMP_Text noteText;
    [SerializeField] GameObject graphicsParent;
    private bool _currentlyActive = false;
    public void updateNotePopupText(string newText)
    {
        Debug.Log($"Note popup assigned new string {newText}");
        noteText.text = newText;
        toggleNotePopup(true);
    }
    public void toggleNotePopup(bool active){
        _currentlyActive = active;
        graphicsParent.gameObject.SetActive(_currentlyActive);

        if(_currentlyActive){
            FirstPersonController.i.changePlayerControlState(false); //take away player control when handling player input
            _timeNoteOpened = Time.time;
        }
        else{
            FirstPersonController.i.changePlayerControlState(true); //give them back control state
        }

        //PlayLocalSFX();
    }
    public void PlayLocalSFX(AudioClip clip){

    }

    private float _noteCloseDelay = 0.5f;
    private float _timeNoteOpened = 0f;

    void Update(){
        if (_currentlyActive && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if(Time.time - _timeNoteOpened > _noteCloseDelay)
            toggleNotePopup(false);
        }
    }
}