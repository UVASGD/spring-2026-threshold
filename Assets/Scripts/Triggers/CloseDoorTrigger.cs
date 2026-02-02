using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class CloseDoorTrigger : MonoBehaviour
{
    [SerializeField] List<OpenableDoor> closeDoors;
    [SerializeField] bool destroyTrigger;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Door closed from triggers");
        foreach(var door in closeDoors)
        {
            door.ForceCloseDoor();
        }
        if(destroyTrigger)
            Destroy(this.gameObject); //destroy this close door trigger
    }
}