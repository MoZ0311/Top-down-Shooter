using UnityEngine;
using UnityEngine.Pool;

public class BulletMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float lifeTime = 3.0f;
    [SerializeField] LayerMask targetLayer;

    [Header("Components")]
    [SerializeField] TrailRenderer trailRenderer;
    IObjectPool<BulletMovement> objectPool;
    float damage;
    float moveSpeed;
    float timer;

    public void SetPool(IObjectPool<BulletMovement> pool)
    {
        objectPool = pool;
    }

    void Update()
    {
        // 1fあたりの移動距離の算出
        float moveDistance = moveSpeed * Time.deltaTime;

        // 対象とするレイヤーにぶつかった時
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, moveDistance, targetLayer, QueryTriggerInteraction.Ignore))
        {
            // 命中時の処理

            // 自身を消去
            objectPool.Release(this);
        }

        // ぶつからなかったので、データを更新
        transform.position += transform.forward * moveDistance;
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            // 時間切れで消滅
            objectPool.Release(this);
        }
    }

    public void SetParameter(float attackPower, float bulletSpeed)
    {
        timer = lifeTime;
        damage = attackPower;
        moveSpeed = bulletSpeed;
        trailRenderer.Clear();
    }
}