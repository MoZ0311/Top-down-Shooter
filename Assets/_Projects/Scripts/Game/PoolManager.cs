using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    // シングルトン用のインスタンス
    public static PoolManager Instance { get; private set; } = null;

    [Header("Prefab")]
    [SerializeField] BulletMovement bulletPrefab;   // 弾のプレハブ
    [SerializeField] BulletEffect effectPrefab;     // エフェクト(ParticleSystem)のプレハブ

    [Header("Settings")]
    [SerializeField] int defaultBulletCapacity;     // 事前に生成する弾の数
    [SerializeField] int defaultEffectCapacity;     // 事前に生成するエフェクトの数
    [SerializeField] int maxSize;                   // 確保できる数
    [SerializeField] Transform bulletParent;        // 弾の親
    [SerializeField] Transform effectParent;        // エフェクトの親

    public IObjectPool<BulletMovement> BulletPool { get; private set; } // 弾用のオブジェクトプール
    public IObjectPool<BulletEffect> EffectPool { get; private set; }   // エフェクト用のオブジェクトプール

    void Awake()
    {
        // シングルトン用
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 各プールの初期化
        InitializePool();
    }

    void Start()
    {
        Prewarm(BulletPool, defaultBulletCapacity);
        Prewarm(EffectPool, defaultEffectCapacity);
    }

    /// <summary>
    /// プールの初期化処理
    /// </summary>
    void InitializePool()
    {
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

    /// <summary>
    /// プールをあらかじめ埋める処理
    /// </summary>
    /// <param name="objectPool">対象のオブジェクトプール</param>
    /// <param name="count">オブジェクトの生成数</param>
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

    /// <summary>
    /// 弾をプールから取り出して、初期化する処理
    /// </summary>
    /// <param name="scale">大きさ</param>
    /// <param name="position">位置</param>
    /// <param name="rotation">向き</param>
    /// <param name="bulletSpeed">速度</param>
    /// <returns>初期化された弾のインスタンス</returns>
    public BulletMovement GetBullet(Vector3 scale, Vector3 position, Quaternion rotation, float bulletSpeed)
    {
        // プールから弾のインスタンスを取得
        BulletMovement bullet = BulletPool.Get();

        // 弾の初期設定
        bullet.transform.SetPositionAndRotation(position, rotation);        // 位置と向きを設定

        Vector3 originScale = bulletPrefab.transform.localScale;
        bullet.transform.localScale = Vector3.Scale(originScale, scale);    // 大きさを設定

        bullet.InitializeBullet(bulletSpeed);
        return bullet;
    }

    /// <summary>
    /// エフェクトをプールから呼び出して、初期化する処理
    /// </summary>
    /// <param name="position">位置</param>
    /// <returns>初期化されたエフェクトのインスタンス</returns>
    public BulletEffect GetEffect(Vector3 position)
    {
        // プールからエフェクトのインスタンスを取得
        BulletEffect effect = EffectPool.Get();

        // 渡された座標にエフェクトを出す
        effect.transform.SetPositionAndRotation(position, Quaternion.identity);
        return effect;
    }
}