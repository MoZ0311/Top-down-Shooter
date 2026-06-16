using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class RankingUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] UIDocument gameUI;
    [SerializeField] VisualTreeAsset rankingElement;

    [Header("Settings")]
    [SerializeField] float spacing;

    VisualElement rankingContainer; // ランキングの格納庫

    class RankingItem
    {
        public VisualElement item;
        public int level;
    }
    readonly List<RankingItem> rankingItemList = new();

    const string RankingContainer = "RankingContainer";
    const string RankingElement = "RankingElement";

    void OnEnable()
    {
        var root = gameUI.rootVisualElement;
        rankingContainer = root.Q<VisualElement>(RankingContainer);

        InitializeRankingUI();
    }

    void InitializeRankingUI()
    {
        int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        for (int i = 0; i < playerCount; ++i)
        {
            AddRankingElement();
        }

        UpdateRankingDisplay();
    }

    void AddRankingElement()
    {
        // ランキングの中身を生成
        TemplateContainer rankingElementInstance = rankingElement.Instantiate();
        VisualElement additionalElement = rankingElementInstance.Q<VisualElement>(RankingElement);
        rankingContainer.Add(additionalElement);

        // 管理用構造体を作成
        RankingItem item = new()
        {
            item = additionalElement,
            level = 0
        };
        rankingItemList.Add(item);
    }

    public void UpdateRankingUI(List<PlayerRankData> rankDataList)
    {
        for (int i = 0; i < rankDataList.Count; ++i)
        {
            // Labelコンポーネントのテキストを更新
            rankingItemList[i].item.Q<Label>().text = $"Lv.{rankDataList[i].level}";
            rankingItemList[i].level = rankDataList[i].level;
        }

        // 更新されたデータに基づいてソート＆アニメーション
        UpdateRankingDisplay();
    }

    void UpdateRankingDisplay()
    {
        var sortedList = rankingItemList.OrderByDescending(c => c.level).ToList();

        for (int i = 0; i < sortedList.Count; ++i)
        {
            float targetY = i * spacing;
            sortedList[i].item.style.top = targetY;
        }
    }
}
