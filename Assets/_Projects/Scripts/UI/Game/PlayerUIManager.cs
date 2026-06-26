using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] PanelRenderer playerUI;
    [SerializeField] float displayInterval;
    VisualElement rootElement;
    VisualElement fill;
    Label levelLabel;
    Label operationLabel;
    const string RootElement = "RootElement";
    const string Fill = "Fill";
    const string LevelLabel = "LevelLabel";
    const string OperationLabel = "OperationLabel";
    const string LevelText = "Lv.";
    const string Hidden = "hidden";
    float timer;
    int uiVersion;

    void OnEnable()
    {
        playerUI.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        playerUI.UnregisterUIReloadCallback(OnUIReload);
    }

    /// <summary>
    /// UIを再構成するコールバック
    /// </summary>
    void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
    {
        if (uiVersion == version)
        {
            return;
        }
        uiVersion = version;

        rootElement = root.Q<VisualElement>(RootElement);
        fill = root.Q<VisualElement>(Fill);
        levelLabel = root.Q<Label>(LevelLabel);
        operationLabel = root.Q<Label>(OperationLabel);
    }

    public void DisplayPlayerUI()
    {
        if (rootElement != null)
        {
            rootElement.style.display = DisplayStyle.Flex;
        }
    }

    public void UpdateOperationLabel(bool isMoving)
    {
        if (isMoving)
        {
            timer = 0;
            operationLabel.AddToClassList(Hidden);
        }
        else if (timer > displayInterval)
        {
            operationLabel.RemoveFromClassList(Hidden);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public void UpdatePlayerUI(int level, float expRatio)
    {
        if (levelLabel == null || fill == null) return;

        levelLabel.text = LevelText + level;
        fill.style.flexGrow = expRatio;
    }
}
