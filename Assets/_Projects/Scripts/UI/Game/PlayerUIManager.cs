using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] float fadeDuration;
    [SerializeField] UIDocument playerUI;
    Label levelLabel;
    VisualElement fill;
    const string LevelLabel ="LevelLabel";
    const string Fill = "Fill";
    const string LevelText = "Lv.";

    public void Initialize()
    {
        // UI要素の検索/取得
        var root = playerUI.rootVisualElement;
        levelLabel = root.Q<Label>(LevelLabel);
        fill = root.Q<VisualElement>(Fill);
    }

    public void UpdatePlayerUI(int level, float expRatio)
    {
        levelLabel.text = LevelText + level;
        fill.style.flexGrow = expRatio;
    }
}
