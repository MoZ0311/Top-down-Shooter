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
        playerScore.finishLevel = newValue;
        if (playerScore.maxLevel < newValue)
        {
            playerScore.maxLevel = newValue;
        }

        transform.localScale = Vector3.one * (1 + CurrentLevel.Value / 10.0f);
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
        if (IsOwner)
        {
            CurrentLevel.OnValueChanged += OnLevelChanged;
        }

        // 初期状態で一度更新
        OnLevelChanged(0, CurrentLevel.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            CurrentLevel.OnValueChanged -= OnLevelChanged;
        }
    }
}
