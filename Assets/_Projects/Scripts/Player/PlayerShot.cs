using UnityEngine;
using Unity.Netcode;

public class PlayerShot : NetworkBehaviour
{
    [Header("Prefab")]
    [SerializeField] NetworkObject bulletPrefab;

    [Header("Settings")]
    [SerializeField] Transform muzzle;

    [Header("Ref Status")]
    [SerializeField] PlayerStatus status;

    public bool IsShooting { private get; set; }
    float fireRateTimer;

    public void HandleShot()
    {
        fireRateTimer = Mathf.Max(0, fireRateTimer - Time.deltaTime);

        if (IsShooting && fireRateTimer <= 0)
        {
            // 弾の生成処理
            BulletPoolManager.Instance.GetBullet(muzzle.position, transform.rotation).SetParameter(status.AttackPower, status.MoveSpeed);
            fireRateTimer = 1.0f / status.FireRate;
            if (!status.CanRapidFire)
            {
                IsShooting = false;
            }
        }
    }
}