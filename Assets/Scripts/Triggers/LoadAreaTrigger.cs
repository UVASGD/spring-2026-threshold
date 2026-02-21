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
        //start the fadeout
        StartCoroutine(Fader.i.roomTransition()); //start the coroutine, as other actions occur during this

        FirstPersonController.i.gameObject.transform.position = teleportPosition;

        SceneManager.LoadScene(sceneAdditiveName);
        
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AreaTextPopup.i.showAreaText(areaText));

        yield return new WaitForSeconds(2f);

        Destroy(this.gameObject);
    }
}
