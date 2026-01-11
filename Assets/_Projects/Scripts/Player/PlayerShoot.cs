using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] Transform muzzle;

    [Header("Ref Status")]
    [SerializeField] PlayerStatus status;

    public bool IsShooting { private get; set; }
    float fireRateTimer;

    public void HandleShoot()
    {
        fireRateTimer = Mathf.Max(0, fireRateTimer - Time.deltaTime);

        if (IsShooting && fireRateTimer <= 0)
        {
            ShootBullet(muzzle.position, transform.rotation, status.BulletSpeed);
            ShootServerRpc();
            fireRateTimer = 1.0f / status.FireRate;
            if (!status.CanRapidFire)
            {
                IsShooting = false;
            }
        }
    }

    // 弾の発射処理
    void ShootBullet(Vector3 muzzlePosition, Quaternion rotation, float bulletSpeed)
    {
        BulletPoolManager.Instance.GetBullet(muzzlePosition, rotation).InitializeBullet(bulletSpeed);
    }

    [ServerRpc]
    void ShootServerRpc()
    {
        // サーバー側で当たり判定計算
        BulletCollisionManager.Instance.AddBullet(muzzle.position, transform.forward, status.BulletSpeed, status.AttackPower);

        // 自分以外のクライアントを対象として、自分のステータスを参照する弾を生成
        ShootClientRpc(muzzle.position, transform.rotation, status.BulletSpeed);
    }

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