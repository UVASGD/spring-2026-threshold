using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] Mesh inspectMesh;
    public Mesh InspectMesh => inspectMesh; //for use outside of this classs
    public virtual void OnItemUsed()
    {
        Debug.Log("Generic Item Used");
    }
    public virtual void OnItemInspected()
    {
        Debug.Log("Item inspected");
    }
}