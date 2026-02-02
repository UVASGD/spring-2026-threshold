using UnityEngine;

public class BatteryItem : InventoryItem
{
    public override void OnItemUsed()
    {
        Debug.Log("Battery Used");
        Flashlight.i.TopOffBattery();
    }
}