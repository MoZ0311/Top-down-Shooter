using UnityEngine;

public class ItemPickup : Pickup
{
    protected override void OnPickedUp(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerLevel component))
        {
            component.PickedExp();
        }
    }
}
