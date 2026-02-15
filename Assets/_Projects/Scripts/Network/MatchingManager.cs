using UnityEngine;
using System.Threading.Tasks;

public class MatchingManager : MonoBehaviour
{
    [SerializeField] int maxConnections;        // 最大接続人数
    [SerializeField] RelayManager relayManager; // リレー管理用のスクリプト
    [SerializeField] LobbyManager lobbyManager; // ロビー管理用のスクリプト

    void Awake()
    {
        // シーンを跨いで存在できるように設定
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ホストとしてマッチを開始する処理
    /// </summary>
    /// <returns>接続できたかどうか</returns>
    public async Task<bool> StartHost()
    {
        string joinCode = await relayManager.CreateRelay(maxConnections);
        return !string.IsNullOrEmpty(joinCode) && await lobbyManager.CreateLobbyWithRelay(joinCode, maxConnections);
    }

    /// <summary>
    /// クライアントとしてマッチを開始する処理
    /// </summary>
    /// <returns>接続できたかどうか</returns>
    public async Task<bool> StartClient()
    {
        string joinCode = await lobbyManager.QuickJoinAndGetRelayCode();
        return !string.IsNullOrEmpty(joinCode) && await relayManager.JoinRelay(joinCode);
    }
}
