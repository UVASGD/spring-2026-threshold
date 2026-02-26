using UnityEngine;
[RequireComponent(typeof(Collider))]
public class LookAtObjectTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //FirstPersonController.i.MovementActive = false;
    }
}