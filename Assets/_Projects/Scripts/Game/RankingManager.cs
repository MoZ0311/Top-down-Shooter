using UnityEngine;
using Unity.Netcode;
using System.Linq;

public struct PlayerRankData : INetworkSerializable
{
    public ulong clientId;
    public int iconIndex;
    public int level;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref iconIndex);
        serializer.SerializeValue(ref level);
    }
}

public class RankingManager : NetworkBehaviour
{
    // シングルトン用のインスタンス
    public static RankingManager Instance { get; private set; } = null;

    [Header("Ref ScoreSO")]
    [SerializeField] PlayerScoreSO playerScore;

    [Header("Scripts")]
    [SerializeField] RankingUIManager rankingUIManager;

    private void Awake()
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

    /// <summary>
    /// サーバーから全クライアントのスコアを参照し、順位を書き込む処理
    /// </summary>
    public void UpdateRankingServer()
    {
        if (!IsServer)
        {
            return;
        }

        // 全プレイヤーの (ClientId, Level) を取得
        var playersData = NetworkManager.Singleton.ConnectedClientsList
            .Where(c => c.PlayerObject != null)
            .Select(c => new PlayerRankData {
                clientId = c.ClientId,
                level = c.PlayerObject.GetComponent<PlayerLevel>().CurrentLevel.Value
            })
            .ToArray();

        // 各プレイヤーに送信
        UpdateRankingClientRpc(playersData);
    }

    [ClientRpc]
    private void UpdateRankingClientRpc(PlayerRankData[] newRankingData)
    {
        var sortedList = newRankingData.OrderByDescending(p => p.level).ToList();

        int rank = sortedList.FindIndex(p => p.clientId == NetworkManager.Singleton.LocalClientId) + 1;
        playerScore.rank = rank;

        rankingUIManager.UpdateRankingUI(newRankingData.ToList());
    }
}
