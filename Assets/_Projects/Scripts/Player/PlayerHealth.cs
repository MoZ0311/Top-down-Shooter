using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Ref Status")]
    [SerializeField] PlayerStatus status;

    readonly NetworkVariable<float> currentHitPoint = new();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHitPoint.Value = status.HitPoint;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHitPoint.Value -= damageAmount;
        if (currentHitPoint.Value <= 0)
        {
            // NetworkObject.Despawn();
            Debug.Log("死");
        }
    }
}