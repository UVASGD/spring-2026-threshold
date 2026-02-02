using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public InventoryItem heldItem;
    public void UseItem()
    {
        heldItem.OnItemUsed();
    }
    public void InspectItem()
    {
        heldItem.OnItemInspected();
    }
}