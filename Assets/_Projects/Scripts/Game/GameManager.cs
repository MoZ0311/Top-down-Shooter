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
    [Min(0)][SerializeField] float openingDuration;                 // 開始時の演出の時間
    [SerializeField] Transform[] spawnPositions = new Transform[4]; // スポーン位置
    public Transform[] SpawnPositions => spawnPositions;

    [Header("Scripts")]
    [SerializeField] GameTimer gameTimer;
    [SerializeField] GameUIManager gameUIManager;

    public bool CanPlayingGame { get; private set; }                // プレイヤーが操作できるか

    const string ResultScene = "ResultScene";

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

        CanPlayingGame = false;
    }

    public override void OnNetworkSpawn()
    {
        // プレイヤーのスポーン処理は、サーバーでのみ行う
        if (IsServer)
        {
            // シーン遷移後のイベント追加
            NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        }

        Invoke(nameof(StartGame), openingDuration);
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
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity).SpawnAsPlayerObject(clientID);
    }

    /// <summary>
    /// ゲームの開始処理
    /// </summary>
    public void StartGame()
    {
        gameUIManager.SwitchUI();
        CameraManager.Instance.SwitchCamera(CameraMode.Player);
        gameTimer.StartCountdown();

        if (IsServer)
        {
            RankingManager.Instance.UpdateRankingServer();
        }

        // 操作可能フラグを立てる
        CanPlayingGame = true;
    }

    /// <summary>
    /// ゲームの終了処理
    /// </summary>
    public void FinishGame()
    {
        // 操作可能フラグを折る
        CanPlayingGame = false;

        // サーバー側からシーン遷移を行う
        if (IsServer)
        {
            RankingManager.Instance.UpdateRankingServer();
            NetworkManager.Singleton.SceneManager.LoadScene(ResultScene, LoadSceneMode.Single);
        }
    }
}