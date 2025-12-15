using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] int maxHealth = 100;

    readonly NetworkVariable<int> currentHealth = new();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth.Value -= damageAmount;
        if (currentHealth.Value <= 0)
        {
            NetworkObject.Despawn();
        }
        Debug.Log($"Player{OwnerClientId}: took {damageAmount} damage");
    }
}
