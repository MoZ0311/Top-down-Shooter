using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class BulletCollisionManager : NetworkBehaviour
{
    public static BulletCollisionManager Instance = null;

    [Header("Settings")]
    [SerializeField] LayerMask targetLayer;
    struct Bullet
    {
        public Vector3 position;
        public Vector3 direction;
        public float moveSpeed;
        public float damageAmount;
        public float lifeTime;
    }
    List<Bullet> bulletList = new();

    void Awake()
    {
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
