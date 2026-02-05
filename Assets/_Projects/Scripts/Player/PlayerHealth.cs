using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Ref Status")]
    [SerializeField] PlayerStatus status;

    readonly NetworkVariable<float> currentHealth = new();
    public float HealthRatio { get; private set; } = 1;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = status.Health;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth.Value -= damageAmount;
        HealthRatio = currentHealth.Value / status.Health;
        if (currentHealth.Value <= 0)
        {
            // NetworkObject.Despawn();
            Debug.Log("死");
        }
    }
}