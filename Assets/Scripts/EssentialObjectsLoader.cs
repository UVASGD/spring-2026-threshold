using System;
using UnityEngine;
[DefaultExecutionOrder(100)] //should occur slightly later than essential objects
public class EssentialObjectsLoader : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectPrefab;
    void Awake()
    {
        if(EssentialObjects.i == null)
        {   Debug.Log("Instantiating a new EssentialObjects instance");
            //if there is no essentialObjects in existence at the moment
            Instantiate(essentialObjectPrefab).transform.position = this.transform.position;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
