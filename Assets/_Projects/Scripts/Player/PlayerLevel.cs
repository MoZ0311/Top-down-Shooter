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

    [Header("Scripts")]
    [SerializeField] PlayerUIManager playerUIManager;

    readonly List<int> nextLevelExpList = new();

    readonly NetworkVariable<int> currentLevel = new(1);
    public NetworkVariable<int> CurrentLevel
    {
        get => currentLevel;
        set => currentLevel.Value = Mathf.Clamp(value.Value, 1, maxLevel);
    }

    readonly NetworkVariable<int> currentExp = new(0);

    void Awake()
    {
        GenerateExpTable();
    }

    public override void OnNetworkSpawn()
    {
        // オーナーの時のみ、UI関連にアクセス
        if (IsOwner)
        {
            playerUIManager.gameObject.SetActive(true);
            playerUIManager.Initialize();
        }

        currentExp.OnValueChanged += OnExpChanged;
        CurrentLevel.OnValueChanged += OnLevelChanged;

        // 初期状態で一度更新
        OnExpChanged(0, currentExp.Value);
        OnLevelChanged(0, CurrentLevel.Value);
    }

    public override void OnNetworkDespawn()
    {
        currentExp.OnValueChanged -= OnExpChanged;
        CurrentLevel.OnValueChanged -= OnLevelChanged;
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
    /// 現在の経験値の進捗率を算出する
    /// </summary>
    float CalculateExpRatio()
    {
        // 最大レベルに達している場合はゲージを満タンにする
        if (CurrentLevel.Value >= maxLevel)
        {
            return 1.0f;
        }

        // 現在のレベルで必要な経験値
        float requiredExp = nextLevelExpList[CurrentLevel.Value];

        // 0除算を防ぎつつ割合を計算
        return requiredExp > 0 ? currentExp.Value / requiredExp : 0f;
    }

    /// <summary>
    /// 現在の経験値に基づき、レベルアップが必要か判定する
    /// </summary>
    void TryLevelUp()
    {
        // 現在のレベルが最大未満、かつ現在の経験値が必要量に達している間繰り返す
        while (CurrentLevel.Value < maxLevel && currentExp.Value >= nextLevelExpList[CurrentLevel.Value])
        {
            // 必要分を消費してレベルアップ
            currentExp.Value -= nextLevelExpList[CurrentLevel.Value];
            CurrentLevel.Value++;
        }
    }

    void UpdatePlayerUI()
    {
        // オーナーのみUIを更新する
        if (IsOwner)
        {
            playerUIManager.UpdatePlayerUI(CurrentLevel.Value, CalculateExpRatio());
        }
    }

    /// <summary>
    /// 経験値を更新し、UIに反映する
    /// </summary>
    void OnExpChanged(int prevValue, int newValue)
    {
        UpdatePlayerUI();
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

        if (IsOwner)
        {
            // UI更新
            UpdatePlayerUI();

            // スコア更新は、Ownerのみが行える
            playerScore.finishLevel = newValue;
            if (playerScore.maxLevel < newValue)
            {
                playerScore.maxLevel = newValue;
            }
        }
    }

    public void PickedExp(int value)
    {
        currentExp.Value += value;
        TryLevelUp();
    }
}
