using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Ref Status")]
    [SerializeField] PlayerStatus status;
    public float MaxHealth => status.Health;
    public NetworkVariable<float> CurrentHealth { get; } = new();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            CurrentHealth.Value = status.Health;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        CurrentHealth.Value -= damageAmount;
        if (CurrentHealth.Value <= 0)
        {
            NetworkObject.Despawn();
            Debug.Log("死");
        }
    }
}