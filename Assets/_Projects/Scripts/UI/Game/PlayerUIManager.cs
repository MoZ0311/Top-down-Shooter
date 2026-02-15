using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] UIDocument playerUI;
    Label levelLabel;
    VisualElement fill;
    VisualElement damageEffect;
    const string LevelLabel ="LevelLabel";
    const string Fill = "Fill";
    const string DamageEffect = "DamageEffect";
    const string LevelText = "Lv.";

    public void Initialize()
    {
        // UI要素の検索/取得
        var root = playerUI.rootVisualElement;
        levelLabel = root.Q<Label>(LevelLabel);
        fill = root.Q<VisualElement>(Fill);
        damageEffect = root.Q<VisualElement>(DamageEffect);
    }

    public void UpdatePlayerUI(int level, float expRatio)
    {
        levelLabel.text = LevelText + level;
        fill.style.flexGrow = expRatio;
    }

    public void OnTookDamege()
    {
        damageEffect.style.opacity = 0.5f;
    }
}
