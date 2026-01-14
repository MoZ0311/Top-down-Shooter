using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance = null;

    [Header("Prefab")]
    [SerializeField] BulletMovement bulletPrefab;
    [SerializeField] BulletEffect effectPrefab;

    [Header("Settings")]
    [SerializeField] int defaultBulletCapacity;
    [SerializeField] int defaultEffectCapacity;
    [SerializeField] int maxSize;
    [SerializeField] Transform bulletParent;
    [SerializeField] Transform effectParent;

    public IObjectPool<BulletMovement> BulletPool { get; private set; }
    public IObjectPool<BulletEffect> EffectPool { get; private set; }

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

        BulletPool = new ObjectPool<BulletMovement>(
            createFunc: () => Instantiate(bulletPrefab, bulletParent),
            actionOnGet: (bullet) => bullet.gameObject.SetActive(true),
            actionOnRelease: (bullet) => bullet.gameObject.SetActive(false),
            defaultCapacity: defaultBulletCapacity,
            maxSize: maxSize
        );

        EffectPool = new ObjectPool<BulletEffect>(
            createFunc: () => Instantiate(effectPrefab, effectParent),
            actionOnGet: (effect) => effect.gameObject.SetActive(true),
            actionOnRelease: (effect) => effect.gameObject.SetActive(false),
            defaultCapacity: defaultEffectCapacity,
            maxSize: maxSize
        );
    }

    void Start()
    {
        Prewarm(BulletPool, defaultBulletCapacity);
        Prewarm(EffectPool, defaultEffectCapacity);
    }

    void Prewarm<T>(IObjectPool<T> objectPool, int count) where T : Component
    {
        // 要素数がcount個のインスタンス配列を定義
        T[] pooledObjects = new T[count];

        // 配列を埋めるまで生成
        for (int i = 0; i < count; ++i)
        {
            pooledObjects[i] = objectPool.Get();
        }

        // 生成したのち、まとめてプールに返す
        foreach (var item in pooledObjects)
        {
            objectPool.Release(item);
        }
    }

    public BulletMovement GetBullet(Vector3 position, Quaternion rotation, float bulletSpeed)
    {
        // プールから弾のインスタンスを取得
        BulletMovement bullet = BulletPool.Get();

        // 弾の初期設定
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.InitializeBullet(bulletSpeed);
        return bullet;
    }

    public BulletEffect GetEffect(Vector3 position)
    {
        BulletEffect effect = EffectPool.Get();
        effect.transform.SetPositionAndRotation(position, Quaternion.identity);
        return effect;
    }
}