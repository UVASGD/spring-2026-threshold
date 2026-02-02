using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteract
{
    [SerializeField] DoorKey key;
    public void OnPlayerInteract()
    {
        Debug.Log($"Key ID {key.keyID} picked up");
        PlayerKeys.i.KeyPickup(key);

        Destroy(this.gameObject);
    }
}