using System.Collections.Generic;
using UnityEngine;

public class EnableDisableTrigger : MonoBehaviour
{
    [SerializeField] List<GameObject> enables;
    [SerializeField] List<GameObject> disables;
    public bool destroySelf;
    private BoxCollider trigger;

    void Awake()
    {
        trigger = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enable/disable trigger hit");

            foreach (var obj in enables)
                obj.SetActive(true);

            foreach (var obj in disables)
                obj.SetActive(false);
        }

        if(destroySelf)
            Destroy(this.gameObject);
    }
}