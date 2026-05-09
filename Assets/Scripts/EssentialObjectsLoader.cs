using System;
using UnityEngine;
[DefaultExecutionOrder(100)] //should occur slightly later than essential objects
public class EssentialObjectsLoader : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectPrefab;
    [SerializeField] bool repositionExistingEssentialObjects = true;

    private void SnapToLoaderTransform(Transform target)
    {
        if (target == null) return;

        target.position = transform.position;
        target.rotation = transform.rotation;
    }

    void Awake()
    {
        if(EssentialObjects.i == null)
        {
            Debug.Log("Instantiating a new EssentialObjects instance");
            //if there is no essentialObjects in existence at the moment
            GameObject instance = Instantiate(essentialObjectPrefab);
            SnapToLoaderTransform(instance.transform);
        }
        else
        {
            if (repositionExistingEssentialObjects)
            {
                SnapToLoaderTransform(EssentialObjects.i.transform);
            }

            Destroy(this.gameObject);
        }
    }
}
