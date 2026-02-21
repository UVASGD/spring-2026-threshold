using System.Collections;
using TMPro;
using UnityEngine;

public class AreaTextPopup : MonoBehaviour
{
    public static AreaTextPopup i;
    [SerializeField] TMP_Text areaText;

    void Awake()
    {
        if(i==null)
            i=this;

    }
    public IEnumerator showAreaText(string areaName)
    {
        areaText.gameObject.SetActive(true);
        areaText.text = areaName;

        yield return new WaitForSeconds(1f);

        areaText.gameObject.SetActive(false);

        yield break;
    }
}
