using UnityEngine;
using UnityEngine.Pool;

public class BulletManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float lifeTime = 3.0f;

    [Header("Components")]
    [SerializeField] Rigidbody bulletRigidbody;
    [SerializeField] TrailRenderer trailRenderer;
    IObjectPool<BulletManager> objectPool;
    float damage;
    float moveSpeed;
    float timer;

    public void SetPool(IObjectPool<BulletManager> pool)
    {
        objectPool = pool;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            objectPool.Release(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            // playerHealth.TakeDamage(damage);
            Debug.Log("hit player");
        }
        objectPool.Release(this);
    }

    public void SetParameter(float attackPower, float bulletSpeed)
    {
        timer = 0;
        damage = attackPower;
        moveSpeed = bulletSpeed;
        bulletRigidbody.linearVelocity = transform.forward * moveSpeed;
        trailRenderer.Clear();
    }
}