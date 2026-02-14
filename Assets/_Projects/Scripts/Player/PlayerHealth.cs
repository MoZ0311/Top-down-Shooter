using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Ref Status")]
    [SerializeField] PlayerStatus status;

    [Header("Ref Score")]
    [SerializeField] PlayerScore score;

    public float MaxHealth => status.Health;
    public NetworkVariable<float> CurrentHealth { get; } = new();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            CurrentHealth.Value = status.Health;
        }
    }

    /// <summary>
    /// サーバーから弾が命中したと判定された時のダメージ処理
    /// </summary>
    /// <param name="damageAmount">ダメージ量</param>
    /// <param name="attackerID">弾の発射主のID</param>
    public void TakeDamage(float damageAmount, ulong attackerID)
    {
        CurrentHealth.Value -= damageAmount;
        if (CurrentHealth.Value <= 0)
        {
            // 自身のデス数を増やす
            score.deathCount.Value++;

            // 弾に渡されたIDを使って、攻撃者を検索
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(attackerID, out var client))
            {
                // 攻撃者のスコアにアクセス
                if (client.PlayerObject.TryGetComponent(out PlayerScore attackerScore))
                {
                    // 攻撃者のキル数を増やす
                    attackerScore.killCount.Value++;
                }
            }
            gameObject.SetActive(false);
        }
    }
}