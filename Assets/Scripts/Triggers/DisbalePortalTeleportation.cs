using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class DisbalePortalTeleportation : MonoBehaviour
{
    //trigger disables the teleportation component of a portal, preventing reentry.
    [SerializeField] List<PortalTeleporter> teleporters;
    [SerializeField] bool destroySelf;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Portal teleportation disabling trigger hit");
        foreach(var portal in teleporters)
        {
            portal.enabled = false; //disable that script while keeping the visual functionality of the portal
        }

        if (destroySelf)
        {
            Debug.Log("Teleportation disable trigger self-deleted");
            Destroy(this.gameObject);
        }
    }
}
