using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] float fadeDuration;
    [SerializeField] UIDocument playerUI;
    [SerializeField] float displayInterval;
    Label levelLabel;
    Label operationLabel;
    VisualElement fill;
    const string LevelLabel = "LevelLabel";
    const string OperationLabel = "OperationLabel";
    const string Fill = "Fill";
    const string LevelText = "Lv.";
    const string Move = "move";
    float timer;

    public void Initialize()
    {
        // UI要素の検索/取得
        var root = playerUI.rootVisualElement;
        levelLabel = root.Q<Label>(LevelLabel);
        operationLabel = root.Q<Label>(OperationLabel);
        fill = root.Q<VisualElement>(Fill);
    }

    public void UpdateOperationLabel(bool isMoving)
    {
        if (isMoving)
        {
            timer = 0;
            operationLabel.AddToClassList(Move);
        }
        else if (timer > displayInterval)
        {
            operationLabel.RemoveFromClassList(Move);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public void UpdatePlayerUI(int level, float expRatio)
    {
        levelLabel.text = LevelText + level;
        fill.style.flexGrow = expRatio;
    }
}
