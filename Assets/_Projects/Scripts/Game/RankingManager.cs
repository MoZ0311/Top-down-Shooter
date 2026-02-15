using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class RankingManager : NetworkBehaviour
{
    // シングルトン用のインスタンス
    public static RankingManager Instance { get; private set; } = null;

    [Header("Ref ScoreSO")]
    [SerializeField] PlayerScoreSO playerScore;

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
    public void UpdateRanksServer()
    {
        if (!IsServer)
        {
            return;
        }

        // 1. 全プレイヤーの (ClientId, Level) を取得
        var players = NetworkManager.Singleton.ConnectedClientsList
            .Select(c => new {
                Id = c.ClientId,
                Level = c.PlayerObject.GetComponent<PlayerLevel>().CurrentLevel.Value
            })
            .OrderByDescending(p => p.Level) // レベル順にソート
            .ToList();

        // 2. 順位を各プレイヤーに送信
        for (int i = 0; i < players.Count; ++i)
        {
            int rank = i + 1;
            // ClientRpcで特定のプレイヤーに順位を通知
            NotifyRankClientRpc(rank, new ClientRpcParams {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { players[i].Id } }
            });
        }
    }

    [ClientRpc]
    private void NotifyRankClientRpc(int rank, ClientRpcParams rpcParams = default)
    {
        // 受け取った順位をSOに即座に反映
        playerScore.rank = rank;
    }
}
