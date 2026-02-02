using UnityEngine;

public class YarnBallPickups : MonoBehaviour, IInteract
{
    public void OnPlayerInteract()
    {
        Debug.Log("Yarn picked up");
        Destroy(this.gameObject);
    }
}