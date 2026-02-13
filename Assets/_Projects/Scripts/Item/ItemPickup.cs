using UnityEngine;

public class ItemPickup : Pickup
{
    protected override void OnPickedUp()
    {
        Debug.Log("ItemPickup");
    }
}
