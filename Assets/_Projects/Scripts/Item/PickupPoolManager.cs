using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PickupPoolManager : NetworkBehaviour
{
    public static PickupPoolManager Instance = null;

    [Header("Settings")]
    [SerializeField] Pickup[] pickupPrefabs;
    [SerializeField] int poolSizePerPrefab = 10; // プレハブごとの初期数
    [SerializeField] Transform itemParent;

    readonly Dictionary<uint, Queue<NetworkObject>> pickupPools = new();
    readonly Dictionary<uint, GameObject> hashToPrefab = new();

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
    }

    public override void OnNetworkSpawn()
    {
        // サーバー・クライアント両方でハンドラーを登録する必要がある
        foreach (var pickup in pickupPrefabs)
        {
            GameObject prefab = pickup.gameObject;
            NetworkObject networkObject = prefab.GetComponent<NetworkObject>();
            uint globalObjectIdHash = networkObject.PrefabIdHash;

            if (!pickupPools.ContainsKey(globalObjectIdHash))
            {
                pickupPools[globalObjectIdHash] = new Queue<NetworkObject>();
                hashToPrefab[globalObjectIdHash] = prefab;

                // NGOに「このプレハブの生成・破棄はこのクラス（内部ハンドラー）に任せて」と登録
                NetworkManager.Singleton.PrefabHandler.AddHandler(networkObject, new PickupInstanceHandler(prefab, this));

                // 事前生成 (Prewarm)
                for (int i = 0; i < poolSizePerPrefab; i++)
                {
                    var obj = CreateNewInstance(prefab);
                    ReturnToPool(globalObjectIdHash, obj);
                }
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        // ハンドラーの解除
        foreach (var pickup in pickupPrefabs)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.PrefabHandler != null)
            {
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(pickup.gameObject);
            }
        }
    }

    NetworkObject CreateNewInstance(GameObject prefab)
    {
        var obj = Instantiate(prefab, itemParent);
        obj.SetActive(false);
        return obj.GetComponent<NetworkObject>();
    }

    public NetworkObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        uint hash = prefab.GetComponent<NetworkObject>().PrefabIdHash;
        NetworkObject netObj;

        if (pickupPools[hash].Count > 0)
        {
            netObj = pickupPools[hash].Dequeue();
        }
        else
        {
            // プールが空なら新しく生成
            netObj = CreateNewInstance(prefab);
        }

        netObj.transform.SetPositionAndRotation(position, rotation);
        netObj.gameObject.SetActive(true); // 表示をオンにする
        return netObj;
    }

    public void ReturnToPool(uint hash, NetworkObject networkObject)
    {
        networkObject.gameObject.SetActive(false); // 非表示にする
        pickupPools[hash].Enqueue(networkObject);
    }
}

// NGOの生成・破棄要求を受け取るための内部クラス
// これにより「どのプレハブが呼ばれたか」を特定できる
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

    // networkObject.Despawn(false) が呼ばれた時に実行される
    public void Destroy(NetworkObject networkObject)
    {
        uint hash = networkObject.PrefabIdHash;
        poolManager.ReturnToPool(hash, networkObject);
    }
}