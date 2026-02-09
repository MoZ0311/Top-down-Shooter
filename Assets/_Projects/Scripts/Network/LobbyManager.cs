using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] float waitTime = 15;
    Lobby currentLobby;
    const string RelayKey = "RelayCode";

    async void Start()
    {
        // 初期化とログイン
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    /// <summary>
    /// リレー機能を用いたロビーの準備
    /// </summary>
    /// <param name="relayCode">リレー機能のID</param>
    /// <param name="maxPlayers">プレイヤーの最大接続数</param>
    /// <returns>ロビー開設に成功したか</returns>
    public async Task<bool> CreateLobbyWithRelay(string relayCode, int maxPlayers)
    {
        try
        {
            var options = new CreateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    { RelayKey, new DataObject(DataObject.VisibilityOptions.Public, relayCode) }
                }
            };
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("MyRoom", maxPlayers, options);
            StartCoroutine(HeartbeatLobby(currentLobby.Id, waitTime));
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    /// <summary>
    /// コードなしでロビーに参加する処理
    /// </summary>
    /// <returns>ロビーのID</returns>
    public async Task<string> QuickJoinAndGetRelayCode()
    {
        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            return currentLobby.Data[RelayKey].Value;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    /// <summary>
    /// ロビーが生きていることを伝える処理
    /// </summary>
    /// <param name="lobbyId">対象のロビー</param>
    /// <param name="waitTime">伝える間隔</param>
    private System.Collections.IEnumerator HeartbeatLobby(string lobbyId, float waitTime)
    {
        var delay = new WaitForSecondsRealtime(waitTime);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}