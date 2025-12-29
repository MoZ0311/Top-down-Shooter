using UnityEngine;
using Unity.Netcode;

public class PlayerShot : NetworkBehaviour
{
    [Header("Prefab")]
    [SerializeField] NetworkObject bulletPrefab;

    [Header("Settings")]
    [SerializeField] Transform muzzlePosition;

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
            Debug.Log("server shot");
            NetworkObject bulletObject = Instantiate(bulletPrefab, muzzlePosition.position, transform.rotation);
            bulletObject.GetComponent<BulletMovement>().SetParameter(status.AttackPower, status.MoveSpeed);
            bulletObject.Spawn();
            fireRateTimer = 1.0f / status.FireRate;
            if (!status.CanRapidFire)
            {
                IsShooting = false;
            }
        }
    }
}
