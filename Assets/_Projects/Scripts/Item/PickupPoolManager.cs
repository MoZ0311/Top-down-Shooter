using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PickupPoolManager : NetworkBehaviour
{
    // シングルトン用のインスタンス
    public static PickupPoolManager Instance { get; private set; } = null;

    [Header("Settings")]
    [SerializeField] PickupItemSO pickupItem;                               // アイテムのリストを持つSO
    readonly Dictionary<uint, Queue<NetworkObject>> pickupPools = new();    // NetworkObject用の疑似プール

    void Awake()
    {
        // シングルトン設計
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        // サーバー・クライアント両方で各プレハブのハンドラーを登録
        foreach (var pickup in pickupItem.PickupItemList)
        {
            GameObject prefab = pickup.gameObject;
            if (!prefab.TryGetComponent(out NetworkObject networkPrefab))
            {
                continue;
            }

            uint hash = networkPrefab.PrefabIdHash;
            if (!pickupPools.ContainsKey(hash))
            {
                pickupPools[hash] = new Queue<NetworkObject>();

                // NGOに生成、破棄のハンドリングを登録
                NetworkManager.Singleton.PrefabHandler.AddHandler(
                    networkPrefab,
                    new PickupInstanceHandler(prefab, this)
                );
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        // ハンドラーの解除
        foreach (var pickup in pickupItem.PickupItemList)
        {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(pickup.gameObject);
        }
    }

    /// <summary>
    /// プールの不足時に新しくオブジェクトを生成する処理
    /// </summary>
    /// <param name="prefab">生成するプレハブ</param>
    /// <returns>生成したNetworkObjectのインスタンス</returns>
    NetworkObject CreateNewInstance(GameObject prefab)
    {
        var obj = Instantiate(prefab);
        obj.SetActive(false);
        if (obj.TryGetComponent(out NetworkObject instance))
        {
            return instance;
        }
        return null;
    }

    /// <summary>
    /// オブジェクトプールから対象のオブジェクトを持ってくる処理
    /// </summary>
    /// <param name="prefab">生成したいプレハブ</param>
    /// <param name="position">生成位置</param>
    /// <param name="rotation">生成時の向き</param>
    /// <returns>持ってきたNetworkObjectのインスタンス</returns>
    public NetworkObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!prefab.TryGetComponent(out NetworkObject component))
        {
            return null;
        }

        uint hash = component.PrefabIdHash;
        NetworkObject netObj;

        if (pickupPools.TryGetValue(hash, out Queue<NetworkObject> pool) && pool.Count > 0)
        {
            netObj = pool.Dequeue();
        }
        else
        {
            // プールが空なら新しく生成
            netObj = CreateNewInstance(prefab);
        }

        netObj.transform.SetPositionAndRotation(position, rotation);
        netObj.gameObject.SetActive(true);
        return netObj;
    }

    /// <summary>
    /// 使い終えたオブジェクトをプールに返還する処理
    /// </summary>
    public void ReturnToPool(uint hash, NetworkObject networkObject)
    {
        networkObject.gameObject.SetActive(false);
        pickupPools[hash].Enqueue(networkObject);
    }
}

// NGOの生成・破棄要求を受け取るための内部クラス
public class PickupInstanceHandler : INetworkPrefabInstanceHandler
{
    readonly GameObject prefab;
    readonly PickupPoolManager poolManager;

    public PickupInstanceHandler(GameObject prefab, PickupPoolManager manager)
    {
        this.prefab = prefab;
        poolManager = manager;
    }

    // NetworkManager.Spawn が呼ばれた時に実行される
    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        return poolManager.GetFromPool(prefab, position, rotation);
    }

    // networkObject.Despawn() が呼ばれた時に実行される
    public void Destroy(NetworkObject networkObject)
    {
        uint hash = networkObject.PrefabIdHash;
        poolManager.ReturnToPool(hash, networkObject);
    }
}