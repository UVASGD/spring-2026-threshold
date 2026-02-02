using UnityEngine;

public class LookTrigger : MonoBehaviour
{
    public float lookDistance = 5f;
    public LayerMask lookLayer;

    private GameObject currentLookTarget;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lookDistance, lookLayer))
        {
            if (hit.collider.gameObject != currentLookTarget)
            {
                currentLookTarget = hit.collider.gameObject;
                currentLookTarget.SendMessage("OnLookEnter", SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (currentLookTarget != null)
        {
            currentLookTarget.SendMessage("OnLookExit", SendMessageOptions.DontRequireReceiver);
            currentLookTarget = null;
        }
    }
}