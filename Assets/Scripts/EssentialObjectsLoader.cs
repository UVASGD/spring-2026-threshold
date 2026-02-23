using System;
using UnityEngine;

public class EssentialObjectsLoader : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectPrefab;
    void Start()
    {
        if(EssentialObjects.i == null)
        {   Debug.Log("Instantiating a new EssentialObjects instance");
            //if there is no essentialObjects in existence at the moment
            Instantiate(essentialObjectPrefab).transform.position = this.transform.position;
        }
    }
}
