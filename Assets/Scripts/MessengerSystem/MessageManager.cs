using System.Collections;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager i;
    [SerializeField] TMP_Text centralText;
    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
    }

    public IEnumerator DisplayText(string shownText, int displayTime = 3)
    {
        centralText.text = shownText;
        centralText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        //now disable the object once the time is finished
        centralText.gameObject.SetActive(false);
        centralText.text = ""; //null out the text field

        yield break;
    }
}
