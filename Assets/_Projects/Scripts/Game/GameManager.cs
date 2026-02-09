using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;
using System.Collections.Generic;

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

        // シーン遷移後のイベント追加
        NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        // 読み込みが完了したクライアント全員に対してプレイヤーを生成
        foreach (var clientId in clientsCompleted)
        {
            SpawnPlayer(clientId);
        }
    }

    /// <summary>
    /// 接続しているクライアントのプレイヤーをスポーンさせる処理
    /// </summary>
    /// <param name="clientID">接続したクライアントのID</param>
    void SpawnPlayer(ulong clientID)
    {
        int index = (int)clientID % spawnPositions.Length;
        var spawnPosition = spawnPositions[index].position;
        NetworkObject playerObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        playerObject.SpawnAsPlayerObject(clientID);
    }
}