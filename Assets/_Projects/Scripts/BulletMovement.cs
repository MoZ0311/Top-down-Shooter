using UnityEngine;
using Unity.Netcode;

public class BulletMovement : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] float lifeTime = 3.0f;

    [Header("Components")]
    [SerializeField] Rigidbody bulletRigidbody;
    float damage;
    float moveSpeed;
    float spawnTime;

    public override void OnNetworkSpawn()
    {
        spawnTime = Time.time;
        if (IsServer)
        {
            bulletRigidbody.linearVelocity = transform.forward * moveSpeed;
        }
    }

    void Update()
    {
        if (IsServer)
        {
            if (Time.time - spawnTime >= lifeTime)
            {
                Despawn();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
            Despawn();
        }
    }

    void Despawn()
    {
        NetworkObject.Despawn();
    }

    public void SetParameter(float attackPower, float bulletSpeed)
    {
        damage = attackPower;
        moveSpeed = bulletSpeed;
    }
}
