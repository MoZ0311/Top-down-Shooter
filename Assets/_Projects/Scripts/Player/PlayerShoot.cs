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
            ShootBullet(muzzle.position, transform.rotation, status);
            ShootServerRpc();
            fireRateTimer = 1.0f / status.FireRate;
            if (!status.CanRapidFire)
            {
                IsShooting = false;
            }
        }
    }

    // 弾の発射処理
    void ShootBullet(Vector3 muzzlePosition, Quaternion rotation, PlayerStatus status)
    {
        BulletPoolManager.Instance.GetBullet(muzzlePosition, rotation).SetParameter(status.AttackPower, status.BulletSpeed);
    }

    [ServerRpc]
    void ShootServerRpc()
    {
        // 当たり判定の実装

        // 自分以外のクライアントを対象として、弾を生成
        ShootClientRpc(muzzle.position, transform.rotation);
    }

    [ClientRpc]
    void ShootClientRpc(Vector3 muzzlePosition, Quaternion rotation)
    {
        // Owner以外で弾を生成したいので、早期return
        if (IsOwner)
        {
            return;
        }
        Debug.Log("player shot");
        ShootBullet(muzzlePosition, rotation, status);
    }
}