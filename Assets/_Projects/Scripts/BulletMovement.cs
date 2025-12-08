using UnityEngine;
using Unity.Netcode;

public class BulletMovement : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float lifeTime = 3.0f;
    [SerializeField] Rigidbody bulletRigidbody;
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
            Despawn();
        }
    }

    void Despawn()
    {
        NetworkObject.Despawn();
    }
}
