using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class LoadAreaTrigger : MonoBehaviour
{
    [SerializeField] string areaText;
    [SerializeField] string sceneAdditiveName;
    [SerializeField] Vector3 teleportPosition;
    private Collider triggerCollider;
    void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        DontDestroyOnLoad(this); //to survive scene transitions
    }

    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(enterTrigger());
    }

    public IEnumerator enterTrigger()
    {
        //set the fader color to black
        Fader.i.updateFaderColor(Color.black);

        //update the text string
        AreaTextPopup.i.updateText(areaText);

        //start the fadeout
        FirstPersonController.i.changePlayerControlState(false); //disable player control
        
        yield return Fader.i.fadeOut(0.5f);

        yield return AreaTextPopup.i.FadeIn(); //start the coroutine for room name display
        
        FirstPersonController.i.toggleGravity(false);
        yield return FirstPersonController.i.gameObject.transform.position = teleportPosition;

        yield return SceneManager.LoadSceneAsync(sceneAdditiveName, LoadSceneMode.Single);

        yield return new WaitForSeconds(0.25f);
        
        FirstPersonController.i.toggleGravity(true);
        FirstPersonController.i.changePlayerControlState(true); //return player control

        yield return Fader.i.fadeIn(0.25f);


        yield return new WaitForSeconds(0.5f);

        yield return AreaTextPopup.i.FadeOut();

        Destroy(this.gameObject);
    }
}
