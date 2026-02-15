using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] Transform muzzle;  // 銃口の位置

    [Header("Ref Status")]
    [SerializeField] PlayerStatus status;

    [Header("Components")]
    [SerializeField] ParticleSystem muzzleFlash;

    public bool IsShooting { private get; set; }
    float fireRateTimer;

    public void HandleShoot()
    {
        // 念のため0以上の範囲になるようにタイマーを減算
        fireRateTimer = Mathf.Max(0, fireRateTimer - Time.deltaTime);

        if (IsShooting && fireRateTimer <= 0)
        {
            // マズルフラッシュのパーティクル再生
            muzzleFlash.Play();

            // ローカル側で即座に生成
            ShootBullet(muzzle.position, transform.rotation, status.BulletSpeed);

            // サーバー側にも生成依頼
            ShootServerRpc();

            // 秒間FireRate発になるよう計算
            fireRateTimer = 1.0f / status.FireRate;

            // オート連射可能でなければ、都度射撃フラグを折る
            if (!status.CanRapidFire)
            {
                IsShooting = false;
            }
        }
    }

    /// <summary>
    /// 弾の発射処理
    /// </summary>
    /// <param name="muzzlePosition">銃口の位置</param>
    /// <param name="rotation">回転</param>
    /// <param name="bulletSpeed">弾速</param>
    void ShootBullet(Vector3 muzzlePosition, Quaternion rotation, float bulletSpeed)
    {
        PoolManager.Instance.GetBullet(
            transform.localScale,
            muzzlePosition,
            rotation,
            bulletSpeed
        );
    }

    /// <summary>
    /// サーバー側で弾道を計算する処理
    /// </summary>
    [ServerRpc]
    void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        // 発信元のIDを取得
        ulong attackerID = rpcParams.Receive.SenderClientId;

        // サーバー側で当たり判定計算
        BulletCollisionManager.Instance.AddBullet(
            attackerID,
            muzzle.position,
            transform.forward,
            status.BulletSpeed,
            status.AttackPower
        );

        // 自分以外のクライアントを対象として、自分のステータスを参照する弾を生成
        ShootClientRpc(muzzle.position, transform.rotation, status.BulletSpeed);
    }

    /// <summary>
    /// クライアント側で弾を生成する処理
    /// </summary>
    /// <param name="muzzlePosition">発射位置</param>
    /// <param name="rotation">向き</param>
    /// <param name="bulletSpeed">弾速</param>
    [ClientRpc]
    void ShootClientRpc(Vector3 muzzlePosition, Quaternion rotation, float bulletSpeed)
    {
        // Owner(弾の発射主)であれば、早期return
        if (IsOwner)
        {
            return;
        }

        // 自分以外のクライアントに、弾を生成させる
        ShootBullet(muzzlePosition, rotation, bulletSpeed);
    }
}