using UnityEngine;
using UnityEngine.Pool;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance = null;
    [SerializeField] BulletManager bulletPrefab;
    IObjectPool<BulletManager> objectPool;

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

        objectPool = new ObjectPool<BulletManager>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: (bullet) => bullet.gameObject.SetActive(true),
            actionOnRelease: (bullet) => bullet.gameObject.SetActive(false),
            defaultCapacity: 100,
            maxSize: 1000
        );
    }

    public BulletManager GetBullet(Vector3 position, Quaternion rotation)
    {
        BulletManager bullet = objectPool.Get();
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.SetPool(objectPool);
        return bullet;
    }
}