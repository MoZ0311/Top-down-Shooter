using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float lifeTime;
    [SerializeField] LayerMask targetLayer;         // 衝突判定を取るレイヤー

    [Header("Components")]
    [SerializeField] TrailRenderer trailRenderer;   // 弾の軌跡を描くTrail
    float moveSpeed;                                // 弾速
    float remainingTime;                            // 残り時間

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
            PoolManager.Instance.GetEffect(hit.point);

            // 自身を消去
            PoolManager.Instance.BulletPool.Release(this);
        }

        // ぶつからなかったので、データを更新
        transform.position += transform.forward * moveDistance;
        remainingTime -= Time.deltaTime;

        if (remainingTime < 0)
        {
            // 時間切れで消滅
            PoolManager.Instance.BulletPool.Release(this);
        }
    }

    /// <summary>
    /// 弾の初期化処理。残り時間と弾速の設定
    /// </summary>
    /// <param name="bulletSpeed">弾速</param>
    public void InitializeBullet(float bulletSpeed)
    {
        remainingTime = lifeTime;
        moveSpeed = bulletSpeed;
        trailRenderer.Clear();
    }
}