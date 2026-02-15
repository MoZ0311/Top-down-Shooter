using UnityEngine;

public class ItemPickup : Pickup
{
    [SerializeField] int exp;
    protected override void OnPickedUp(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerLevel component))
        {
            component.PickedExp(exp);
        }
    }
}
