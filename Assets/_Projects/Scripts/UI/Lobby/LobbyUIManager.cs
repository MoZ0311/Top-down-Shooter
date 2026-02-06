using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class LobbyUIManager : NetworkBehaviour
{
    [SerializeField] int minPlayersToStart = 2;
    [SerializeField] UIDocument lobbyUI;
    Label joinCodeLabel;
    Button startButton;
    const string LobbyIDString = "ロビーID:";

    void Awake()
    {
        var root = lobbyUI.rootVisualElement;
        joinCodeLabel = root.Q<Label>();
        startButton = root.Q<Button>();

        // ロビーIDを表示
        joinCodeLabel.text = LobbyIDString + RelayManager.JoinCode;

        // ホストでない場合は、早期return
        if (!NetworkManager.Singleton.IsHost)
        {
            return;
        }

        // クライアントの接続時、離脱時にイベント登録
        NetworkManager.Singleton.OnClientConnectedCallback += UpdateUI;
        NetworkManager.Singleton.OnClientDisconnectCallback += UpdateUI;

        // 初期状態の確認
        UpdateUI(0);

        // ボタン表示
        startButton.style.display = DisplayStyle.Flex;

        startButton.clicked += OnClickedStart;
    }

    void UpdateUI(ulong clientId)
    {
        // 現在の接続人数を取得
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        // 規定人数以上ならボタンを有効化
        startButton.enabledSelf = playerCount >= minPlayersToStart;
    }

    void OnClickedStart()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
