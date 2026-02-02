using System.Collections.Generic;
using UnityEngine;

public class LookEnableDisable : MonoBehaviour, ILookable
{
    [SerializeField] List<GameObject> enables;
    [SerializeField] List<GameObject> disables;
    public bool destroySelf;

    //look trigger version of enable/disable trigger

    public void OnLookEnter()
    {
        Debug.Log("Enable/disable trigger hit");

        foreach (var obj in enables)
            obj.SetActive(true);

        foreach (var obj in disables)
            obj.SetActive(false);

        if (destroySelf)
            Destroy(this.gameObject);
    }

    public void OnLookExit()
    {
        //N/A
    }
}
