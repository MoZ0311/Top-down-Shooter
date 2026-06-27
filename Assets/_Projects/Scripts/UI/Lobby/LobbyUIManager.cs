using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class LobbyUIManager : NetworkBehaviour
{
    [SerializeField] PanelRenderer lobbyUI;

    [Header("Scripts")]
    [SerializeField] LobbyPlayerManager lobbyPlayerManager;
    Label playerCountLabel;
    Label waitingTextLabel;
    Button startButton;
    Button leaveButton;
    int uiVersion;
    const int MinPlayerToStart = 2;
    const int MaxPlayerCount = 4;
    const string PlayerCountLabel = "PlayerCountLabel";
    const string WaitingTextLabel = "WaitingTextLabel";
    const string GameScene = "GameScene";
    const string StartButton = "StartButton";
    const string LeaveButton = "LeaveButton";
    readonly NetworkVariable<int> playerCount = new(0);

    void OnEnable()
    {
        lobbyUI.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        lobbyUI.UnregisterUIReloadCallback(OnUIReload);
    }

    /// <summary>
    /// UIを再構成するコールバック
    /// </summary>
    void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
    {
        if (uiVersion == version)
        {
            return;
        }
        uiVersion = version;

        playerCountLabel = root.Q<Label>(PlayerCountLabel);
        waitingTextLabel = root.Q<Label>(WaitingTextLabel);
        startButton = root.Q<Button>(StartButton);
        leaveButton = root.Q<Button>(LeaveButton);

        startButton.RegisterCallback<ClickEvent>(OnClickedStartButton);
        leaveButton.RegisterCallback<ClickEvent>(OnClickedLeaveButton);

        if (IsServer)
        {
            // 待ちテキスト非表示
            waitingTextLabel.style.display = DisplayStyle.None;

            // ボタン表示
            startButton.style.display = DisplayStyle.Flex;
        }

        UpdatePlayerAndUI(playerCount.Value);
    }

    /// <summary>
    /// UI状態の更新処理
    /// </summary>
    /// <param name="playerCount">ロビーのプレイヤー数</param>
    void UpdatePlayerAndUI(int playerCount)
    {
        lobbyPlayerManager.UpdatePlayerVisuals(playerCount);

        // プレイヤーの数を表示
        if (playerCountLabel == null)
        {
            return;
        }
        playerCountLabel.text = $"ロビー:{playerCount}/{MaxPlayerCount}";

        // 規定人数以上ならサーバー側でボタンを有効化
        if (IsServer)
        {
            startButton.SetEnabled(playerCount >= MinPlayerToStart);
        }
    }

    /// <summary>
    /// プレイヤー数の更新処理。サーバーからのみ呼び出す
    /// </summary>
    void UpdatePlayerCount(ulong clientID)
    {
        if (IsServer)
        {
            playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
        }
    }

    /// <summary>
    /// ボタン押下時の処理
    /// </summary>
    void OnClickedStartButton(ClickEvent evt)
    {
        // ボタンクリック時のSE再生
        AudioPlayer.Instance.PlaySE("button");
        NetworkManager.Singleton.SceneManager.LoadScene(GameScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void OnClickedLeaveButton(ClickEvent evt)
    {
        // シーン内に残っているMatchingManagerを探して切断処理を呼ぶ
        if (MatchingManager.Instance != null)
        {
            MatchingManager.Instance.LeaveMatchAndReturnToTitle();
        }
        else
        {
            Debug.LogError("MatchingManagerが見つかりません。");
        }
    }

    public override void OnNetworkSpawn()
    {
        // 接続人数が変動したときのイベント登録
        playerCount.OnValueChanged += OnPlayerCountChanged;

        if (IsServer)
        {
            // クライアントの接続時、離脱時にイベント登録
            NetworkManager.Singleton.OnClientConnectedCallback += UpdatePlayerCount;
            NetworkManager.Singleton.OnClientDisconnectCallback += UpdatePlayerCount;

            // サーバー接続後の状態でUIを更新
            UpdatePlayerCount(0);
        }

        // 初期状態でプレイヤーのモデルとUI更新
        lobbyPlayerManager.UpdatePlayerVisuals(playerCount.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= UpdatePlayerCount;
            NetworkManager.Singleton.OnClientDisconnectCallback -= UpdatePlayerCount;
        }

        playerCount.OnValueChanged -= OnPlayerCountChanged;
    }

    void OnPlayerCountChanged(int prevValue, int newValue)
    {
        UpdatePlayerAndUI(newValue);
    }
}
