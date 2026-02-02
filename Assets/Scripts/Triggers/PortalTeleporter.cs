using UnityEngine;
using System.Collections;

public class PortalTeleporter : MonoBehaviour
{
    [Header("Settings")]
    public Transform reciever; 
    public Transform player;

    [Header("Debug")]
    public bool playerIsOverlapping = false;
    
    // NEW: A flag to prevent the destination from firing immediately
    private bool isJustArrived = false;

    void Start()
    {
        if (player == null)
        {
             GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
             if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        // Safety check: Only run if player is here AND we aren't in cooldown
        if (playerIsOverlapping && !isJustArrived && player != null)
        {
            Vector3 portalToPlayer = player.position - transform.position;
            
            // Your setup requires > 0f because your Blue Arrow points IN
            float dotProduct = Vector3.Dot(transform.forward, portalToPlayer);

            if (dotProduct > 0f) 
            {
                Teleport();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOverlapping = true;
            // Note: We do NOT reset isJustArrived here. 
            // We wait for OnTriggerExit to do that.
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOverlapping = false;
            
            // NEW: Once they leave the trigger, they are allowed to use this portal again
            isJustArrived = false;
        }
    }
    
    // NEW: Helper function called by the OTHER portal
    public void OnPlayerArrived()
    {
        isJustArrived = true;
        playerIsOverlapping = true; // We know they are here now
    }

    void Teleport()
    {
        // 1. Get the script on the OTHER portal
        PortalTeleporter destScript = reciever.GetComponent<PortalTeleporter>();
        if (destScript != null)
        {
            // Tell the destination: "Expect a visitor, don't teleport them back!"
            destScript.OnPlayerArrived();
        }

        // 2. Calculate Position (Relative to THIS portal)
        Vector3 localPos = transform.InverseTransformPoint(player.position);
        localPos = Quaternion.Euler(0f, 180f, 0f) * localPos;
        Vector3 targetPosition = reciever.TransformPoint(localPos);

        // 3. Rotation
        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * player.rotation;
        relativeRot = Quaternion.Euler(0, 180, 0) * relativeRot;
        Quaternion targetRotation = reciever.rotation * relativeRot;

        // 4. Move the Player (CharacterController Safe Mode)
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.position = targetPosition;
            player.rotation = targetRotation;
            cc.enabled = true;
        }
        else
        {
            player.position = targetPosition;
            player.rotation = targetRotation;
        }

        // 5. Cleanup THIS portal's state
        // We just sent them away, so they are no longer overlapping US.
        playerIsOverlapping = false;
    }
}