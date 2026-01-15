using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class BulletCollisionManager : NetworkBehaviour
{
    // シングルトン用のインスタンス
    public static BulletCollisionManager Instance = null;

    [Header("Settings")]
    [SerializeField] LayerMask targetLayer; // 弾が接触できるレイヤー

    // サーバー側で計算する、仮想の弾丸構造体
    struct Bullet
    {
        public Vector3 position;    // 位置
        public Vector3 direction;   // 方向
        public float moveSpeed;     // 弾速
        public float damageAmount;  // ダメージ量
        public float lifeTime;      // 残り時間
    }
    readonly List<Bullet> bulletList = new();   // 計算する仮想弾丸のリスト

    void Awake()
    {
        // シングルトン設計
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // ダメージを与える弾の当たり判定は、サーバーでのみ計算する
        if (!IsServer)
        {
            return;
        }

        // すべての弾の軌道を計算
        for (int i = 0; i < bulletList.Count; ++i)
        {
            var bullet = bulletList[i];

            // 1fあたりの移動距離の算出
            float moveDistance = bullet.moveSpeed * Time.deltaTime;

            // 対象とするレイヤーにぶつかった時
            if (Physics.Raycast(bullet.position, bullet.direction, out RaycastHit hit, moveDistance, targetLayer, QueryTriggerInteraction.Ignore))
            {
                // TryGetComponentでHP管理スクリプト取得/アクセスを試みる
                if (hit.collider.TryGetComponent<PlayerHealth>(out var playerHealth))
                {
                    // プレイヤーのダメージ処理呼び出し
                    playerHealth.TakeDamage(bullet.damageAmount);
                }

                // 自身をリストから消去してcontinue
                bulletList.RemoveAt(i);
                continue;
            }

            // ぶつからなかったので、データを更新
            bullet.position += bullet.direction * moveDistance;
            bullet.lifeTime -= Time.deltaTime;

            if (bullet.lifeTime < 0)
            {
                // 時間切れで消滅
                bulletList.RemoveAt(i);
            }
            else
            {
                // リストに変更を反映
                bulletList[i] = bullet;
            }
        }
    }

    /// <summary>
    /// サーバー側で計算する弾を追加する
    /// </summary>
    /// <param name="pos">発射した座標</param>
    /// <param name="dir">発射した方向</param>
    /// <param name="speed">弾速</param>
    /// <param name="damage">ダメージ量</param>
    public void AddBullet(Vector3 pos, Vector3 dir, float speed, float damage)
    {
        bulletList.Add(new Bullet {
            position = pos,
            direction = dir,
            moveSpeed = speed,
            damageAmount = damage,
            lifeTime = 3.0f
        });
    }
}
