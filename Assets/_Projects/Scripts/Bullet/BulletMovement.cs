using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float lifeTime = 3.0f;
    [SerializeField] LayerMask targetLayer;

    [Header("Components")]
    [SerializeField] TrailRenderer trailRenderer;
    float moveSpeed;
    float timer;

    void Update()
    {
        // 1fあたりの移動距離の算出
        float moveDistance = moveSpeed * Time.deltaTime;

        // 対象とするレイヤーにぶつかった時
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, moveDistance, targetLayer, QueryTriggerInteraction.Ignore))
        {
            // 命中判定時、位置を補正
            transform.position = hit.point;

            // エフェクト再生
            BulletPoolManager.Instance.GetEffect(hit.point);

            // 自身を消去
            BulletPoolManager.Instance.BulletPool.Release(this);
        }

        // ぶつからなかったので、データを更新
        transform.position += transform.forward * moveDistance;
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            // 時間切れで消滅
            BulletPoolManager.Instance.BulletPool.Release(this);
        }
    }

    public void InitializeBullet(float bulletSpeed)
    {
        timer = lifeTime;
        moveSpeed = bulletSpeed;
        trailRenderer.Clear();
    }
}