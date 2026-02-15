using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[System.Serializable]
public struct SpawnArea
{
    [SerializeField] Vector3 center;            // エリアの中心
    [SerializeField] float radius;              // エリアの半径
    [Range(1, 10), SerializeField] int weight;  // エリアの重み(抽選率)

    public readonly Vector3 Center => center;
    public readonly float Radius => radius;
    public readonly int Weight => weight;
}

public class PickupSpawner : NetworkBehaviour
{
    // シングルトン用のインスタンス
    public static PickupSpawner Instance { get; private set; } = null;

    [Header("Settings")]
    [SerializeField] PickupItemSO pickupItem;               // アイテムのリストを持つSO
    [SerializeField] int maxItemCount;                      // 通常スポーン時の最大数
    [SerializeField] float spawnInterval;                   // スポーンの間隔
    [SerializeField] List<SpawnArea> spawnAreas = new();    // 生成範囲のリスト
    [SerializeField] Transform itemParent;                  // 生成したアイテムたちの親オブジェクト
    readonly List<NetworkObject> activeItems = new();       // アクティブ状態のアイテム
    float spawnTimer = 0;                                   // スポーン用のタイマー

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
        // サーバー以外ではスポーン処理を行わない
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        spawnTimer = spawnInterval;
    }

    void Update()
    {
        // サーバー外であったり、NetworkManagerが見つからなかったりすれば、早期return
        if (!IsServer || NetworkManager.Singleton == null)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0;
            SpawnItem();
        }
    }

    /// <summary>
    /// 最大数までアイテムを補充する処理
    /// </summary>
    void SpawnItem()
    {
        // リストから既に破壊された（拾われた）アイテムを除去
        activeItems.RemoveAll(item => item == null || !item.IsSpawned);

        // 足りない個数を計算する
        int itemsToSpawn = maxItemCount - activeItems.Count;

        // 足りない分だけループして生成
        for (int i = 0; i < itemsToSpawn; ++i)
        {
            // 重みに基づいてエリアを選択
            SpawnArea selectedArea = GetRandomArea();

            // エリア内のランダムな位置を計算
            Vector2 randomPoint = Random.insideUnitCircle * selectedArea.Radius;
            Vector3 spawnPosition = selectedArea.Center + new Vector3(randomPoint.x, 0, randomPoint.y);

            // スポーン処理
            NetworkObject networkItem = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                pickupItem.PickupItemList[0],
                OwnerClientId,
                true,
                position: spawnPosition,
                rotation: Quaternion.identity
            );

            // 親の設定
            if (!networkItem.TrySetParent(itemParent))
            {
                Debug.LogWarning("Could not set parent");
            }

            // 管理リストに追加
            activeItems.Add(networkItem);
        }
    }

    /// <summary>
    /// 指定した地点を中心に、アイテムの群れを即座に生成します。
    /// </summary>
    /// <param name="origin">中心座標</param>
    /// <param name="range">散らばる半径</param>
    /// <param name="count">生成する個数</param>
    /// <param name="prefab">生成したいアイテムのNetworkObjectプレハブ</param>
    public void SpawnBurst(Vector3 origin, float range, int count, NetworkObject prefab = null)
    {
        // サーバー以外では実行不可
        if (!IsServer)
        {
            return;
        }

        for (int i = 0; i < count; ++i)
        {
            // 中心座標から指定範囲内のランダムな位置を計算
            Vector2 randomCircle = Random.insideUnitCircle * range;
            Vector3 spawnPosition = origin + new Vector3(randomCircle.x, 0, randomCircle.y);

            // プレハブに指定がなければ、ノーマル経験値を発生させる
            NetworkObject itemPrefab = prefab == null ? pickupItem.PickupItemList[0] : prefab;

            // プールから取得してネットワークスポーン
            NetworkObject networkItem = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                itemPrefab,
                OwnerClientId,
                true,
                position: spawnPosition,
                rotation: Quaternion.identity
            );

            // 親の設定
            if (!networkItem.TrySetParent(itemParent))
            {
                Debug.LogWarning("Could not set parent");
            }
        }
    }

    /// <summary>
    /// ランダムなスポーンエリアを返す処理
    /// </summary>
    /// <returns>ランダムな球状の範囲</returns>
    SpawnArea GetRandomArea()
    {
        // リスト内のエリアの重みの合計値を算出
        int totalWeight = 0;
        foreach (var area in spawnAreas)
        {
            totalWeight += area.Weight;
        }

        // 乱数を作成
        int random = Random.Range(0, totalWeight);

        // エリアの重みを足していき、一致する場所を探す
        int currentWeight = 0;
        foreach (var area in spawnAreas)
        {
            currentWeight += area.Weight;

            // 乱数が重みの合計値の中に入れば、そのエリアを返す
            if (random < currentWeight)
            {
                return area;
            }
        }

        // 到達しなかった場合、リストの先頭を返す
        return spawnAreas[0];
    }

    private void OnDrawGizmos()
    {
        foreach (var zone in spawnAreas)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawSphere(zone.Center, zone.Radius);
        }
    }
}
