using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerLevel : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxLevel;      // 最大レベル
    [SerializeField] float baseExp;     // Lv1→2に必要な経験値
    [SerializeField] float multiplier;  // 増加倍率 (n倍)

    [Header("ScoreSO")]
    [SerializeField] PlayerScoreSO playerScore;

    [Header("Ref Status")]
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] PlayerHealth playerHealth;

    readonly List<int> nextLevelExpList = new();
    public NetworkVariable<int> CurrentLevel { get; } = new(1);
    int currentExp;

    void Awake()
    {
        GenerateExpTable();
    }

    /// <summary>
    /// 経験値テーブルを生成する
    /// </summary>
    void GenerateExpTable()
    {
        nextLevelExpList.Clear();

        // レベル0を挿入
        nextLevelExpList.Add(0);

        float currentRequiredExp = baseExp;

        for (int level = 1; level < maxLevel; ++level)
        {
            // 小数点以下を切り捨てて整数としてリストに追加
            nextLevelExpList.Add(Mathf.FloorToInt(currentRequiredExp));

            // 次のレベルの必要経験値を計算（n倍する）
            currentRequiredExp *= multiplier;
        }
    }

    /// <summary>
    /// 現在の経験値に基づき、レベルアップが必要か判定する
    /// </summary>
    void TryLevelUp()
    {
        // 現在のレベルが最大未満、かつ現在の経験値が必要量に達している間繰り返す
        while (CurrentLevel.Value < maxLevel && currentExp >= nextLevelExpList[CurrentLevel.Value])
        {
            // 必要分を消費してレベルアップ
            currentExp -= nextLevelExpList[CurrentLevel.Value];
            CurrentLevel.Value++;
        }
    }

    /// <summary>
    /// 現在のレベルが変動したときの処理
    /// </summary>
    void OnLevelChanged(int prevValue, int newValue)
    {
        // 直前の最大HPを取得
        float prevMaxHealth = playerStatus.Health;

        // ステータスの再計算は全てのクライアントで実行される
        playerStatus.UpdateStatus(newValue);

        // サーバー側でHP最大値の変更に伴う処理を行う
        if (IsServer)
        {
            // レベルアップの伴うHP増加分を計算
            float diff = playerStatus.Health - prevMaxHealth;

            // 増加分を負のダメージ(回復)として与える
            playerHealth.TakeDamage(-diff);
        }

        // スコア更新は、Ownerのみが行える
        if (IsOwner)
        {
            playerScore.finishLevel = newValue;
            if (playerScore.maxLevel < newValue)
            {
                playerScore.maxLevel = newValue;
            }
        }
    }

    public void PickedExp()
    {
        currentExp++;
        TryLevelUp();
    }

    public void LostExp()
    {
        currentExp--;
    }

    public override void OnNetworkSpawn()
    {
        CurrentLevel.OnValueChanged += OnLevelChanged;

        // 初期状態で一度更新
        OnLevelChanged(0, CurrentLevel.Value);
    }

    public override void OnNetworkDespawn()
    {
        CurrentLevel.OnValueChanged -= OnLevelChanged;
    }
}
