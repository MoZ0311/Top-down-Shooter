using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] PanelRenderer playerUI;
    [SerializeField] float displayInterval;
    Label levelLabel;
    Label operationLabel;
    VisualElement fill;
    const string LevelLabel = "LevelLabel";
    const string OperationLabel = "OperationLabel";
    const string Fill = "Fill";
    const string LevelText = "Lv.";
    const string Hidden = "hidden";
    float timer;
    int uiVersion;

    void Awake()
    {
        playerUI.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDestroy()
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

        levelLabel = root.Q<Label>(LevelLabel);
        operationLabel = root.Q<Label>(OperationLabel);
        fill = root.Q<VisualElement>(Fill);

        playerUI.enabled = false;
    }

    public void UpdateOperationLabel(bool isMoving)
    {
        playerUI.enabled = true;
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
