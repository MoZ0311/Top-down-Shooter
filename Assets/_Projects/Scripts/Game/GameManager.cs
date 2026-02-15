using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    // シングルトン用のインスタンス
    public static GameManager Instance { get; private set; } = null;

    [Header("Prefab")]
    [SerializeField] NetworkObject playerPrefab;                    // プレイヤーとして生成されるプレハブ

    [Header("Settings")]
    [SerializeField] Transform[] spawnPositions = new Transform[4]; // スポーン位置
    public Transform[] SpawnPositions => spawnPositions;

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
        // プレイヤーのスポーン処理は、サーバーでのみ行う
        if (IsServer)
        {
            // シーン遷移後のイベント追加
            NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// シーン遷移後のプレイヤー生成処理
    /// </summary>
    /// <param name="clientsCompleted">読み込みが終わったクライアントのリスト</param>
    void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
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
        Vector3 spawnPosition = spawnPositions[index].position;
        NetworkObject playerObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        playerObject.SpawnAsPlayerObject(clientID);
    }
}