using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class LobbyUIManager : NetworkBehaviour
{
    [SerializeField] UIDocument lobbyUI;
    Label playerCountLabel;
    Button startButton;
    const int MinPlayerToStart = 2;
    const string GameScene = "GameScene";
    readonly NetworkVariable<int> playerCount = new(0);

    void Awake()
    {
        // UI要素の検索/取得
        var root = lobbyUI.rootVisualElement;
        playerCountLabel = root.Q<Label>();
        startButton = root.Q<Button>();
    }

    /// <summary>
    /// UI状態の更新処理
    /// </summary>
    /// <param name="playerCount">ロビーのプレイヤー数</param>
    void UpdateUI(int playerCount)
    {
        // プレイヤーの数を表示
        playerCountLabel.text = $"ロビー:{playerCount}/4";

        // 規定人数以上ならサーバー側でボタンを有効化
        if (IsServer)
        {
            startButton.enabledSelf = playerCount >= MinPlayerToStart;
        }
    }

    /// <summary>
    /// プレイヤー数の更新処理。サーバーからのみ呼び出す
    /// </summary>
    void UpdatePlayerCount(ulong clientID)
    {
        playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    /// <summary>
    /// ボタン押下時の処理
    /// </summary>
    void OnClickedStart()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(GameScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public override void OnNetworkSpawn()
    {
        // 接続人数が変動したとき、UIも更新する
        playerCount.OnValueChanged += (prevValue, newValue) => UpdateUI(newValue);

        if (IsServer)
        {
            // クライアントの接続時、離脱時にイベント登録
            NetworkManager.Singleton.OnClientConnectedCallback += UpdatePlayerCount;
            NetworkManager.Singleton.OnClientDisconnectCallback += UpdatePlayerCount;

            // ボタン表示
            startButton.style.display = DisplayStyle.Flex;

            // 押下時のイベント登録
            startButton.clicked += OnClickedStart;

            // サーバー接続後の状態でUIを更新
            UpdatePlayerCount(0);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= UpdatePlayerCount;
            NetworkManager.Singleton.OnClientDisconnectCallback -= UpdatePlayerCount;
            startButton.clicked -= OnClickedStart;
        }
    }
}
