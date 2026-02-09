using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class LobbyUIManager : NetworkBehaviour
{
    [SerializeField] UIDocument lobbyUI;
    Label playerCountLabel;
    Button startButton;
    const int MinPlayerToStart = 2;

    void Awake()
    {
        // UI要素の検索/取得
        var root = lobbyUI.rootVisualElement;
        playerCountLabel = root.Q<Label>();
        startButton = root.Q<Button>();

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
        startButton.enabledSelf = playerCount >= MinPlayerToStart;

        // プレイヤーの数を表示
        playerCountLabel.text = $"参加者{playerCount}/4";
    }

    void OnClickedStart()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
