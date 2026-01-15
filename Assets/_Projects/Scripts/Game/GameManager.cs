using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [Header("Prefab")]
    [SerializeField] NetworkObject playerPrefab;    // プレイヤーとして生成されるプレハブ

    [Header("Settings")]
    [SerializeField] Transform[] spawnPositions;    // スポーン位置
    public override void OnNetworkSpawn()
    {
        // プレイヤーのスポーン処理は、サーバーでのみ行う
        if (!IsServer)
        {
            return;
        }

        // クライアント接続時のイベント追加
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            // サーバー接続前に接続済みのクライアントに処理を適用
            OnClientConnected(client.ClientId);
        }
    }

    /// <summary>
    /// 接続しているクライアントのプレイヤーをスポーンさせる処理
    /// </summary>
    /// <param name="clientID">接続したクライアントのID</param>
    void OnClientConnected(ulong clientID)
    {
        var spawnPosition = spawnPositions[clientID].position;
        NetworkObject playerObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        playerObject.SpawnAsPlayerObject(clientID);
    }
}