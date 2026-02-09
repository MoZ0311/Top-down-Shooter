using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MatchingManager : MonoBehaviour
{
    [SerializeField] int maxConnections = 4;
    [SerializeField] RelayManager relayManager;
    [SerializeField] LobbyManager lobbyManager;

    void Awake()
    {
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
