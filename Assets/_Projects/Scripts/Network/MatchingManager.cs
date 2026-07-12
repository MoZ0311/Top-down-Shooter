using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class MatchingManager : MonoBehaviour
{
    [SerializeField] bool isOnline;
    [SerializeField] int maxConnections;        // 最大接続人数
    [SerializeField] LanConnectionManager lanConnectionManager;
    [SerializeField] RelayManager relayManager; // リレー管理用のスクリプト
    [SerializeField] LobbyManager lobbyManager; // ロビー管理用のスクリプト

    const string TitleSceneString = "TitleScene";

    public static MatchingManager Instance { get; private set; } = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            // 起動時の引数に「-profile プロファイル名」が含まれていたら、そのプロファイルに切り替える
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-profile" && i + 1 < args.Length)
                {
                    string profileName = args[i + 1];
                    AuthenticationService.Instance.SwitchProfile(profileName);
                    break;
                }
            }

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    /// <summary>
    /// ホストとしてマッチを開始する処理
    /// </summary>
    /// <returns>接続できたかどうか</returns>
    public async Task<bool> StartHost()
    {
        if (isOnline)
        {
            string joinCode = await relayManager.CreateRelay(maxConnections);
            return !string.IsNullOrEmpty(joinCode) && await lobbyManager.CreateLobbyWithRelay(joinCode, maxConnections);
        }

        return lanConnectionManager.StartHost();
    }

    /// <summary>
    /// クライアントとしてマッチを開始する処理
    /// </summary>
    /// <returns>接続できたかどうか</returns>
    public async Task<bool> StartClient()
    {
        if (isOnline)
        {
            string joinCode = await lobbyManager.QuickJoinAndGetRelayCode();
            return !string.IsNullOrEmpty(joinCode) && await relayManager.JoinRelay(joinCode);
        }

        return lanConnectionManager.StartClient();
    }

    /// <summary>
    /// ボタンから呼び出す、マッチ切断＆タイトル帰還処理
    /// </summary>
    public async void LeaveMatchAndReturnToTitle()
    {
        // オンライン時はLobbyの退出/削除を待つ
        if (isOnline)
        {
            await lobbyManager.LeaveLobby();
        }

        // Netcode for GameObjects の通信を終了
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // タイトルシーンへ遷移
        SceneManager.LoadScene(TitleSceneString);
    }
}
