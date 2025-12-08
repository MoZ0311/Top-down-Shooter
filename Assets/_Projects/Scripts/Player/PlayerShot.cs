using UnityEngine;
using Unity.Netcode;

public class PlayerShot : NetworkBehaviour
{
    [SerializeField] NetworkObject bulletPrefab;
    [SerializeField] Transform muzzlePosition;
    [SerializeField] float fireRate = 5.0f;
    [SerializeField] bool canRapidFire;
    public bool IsShooting { private get; set; }
    public bool CanCreateLocalBullet { get; private set; }
    float fireRateTimer;

    public void HandleShot()
    {
        fireRateTimer = Mathf.Max(0, fireRateTimer - Time.deltaTime);

        if (IsShooting && fireRateTimer <= 0)
        {
            // 弾の生成処理
            Debug.Log("server shot");
            NetworkObject bulletObject = Instantiate(bulletPrefab, muzzlePosition.position, transform.rotation);
            bulletObject.Spawn();
            fireRateTimer = 1.0f / fireRate;
            if (!canRapidFire)
            {
                IsShooting = false;
            }
        }
    }
}
