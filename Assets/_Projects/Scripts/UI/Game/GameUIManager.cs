using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] PanelRenderer overviewUI;
    [SerializeField] PanelRenderer gameUI;

    [Header("Settings")]
    [SerializeField] int redThredhouldTime;             // タイマーを赤色にする閾値

    public VisualElement CrossHair { get; private set; }
    VisualElement rootElement;
    Label timerLabel;                                   // 残り時間を表示するラベル
    int uiVersion;
    const int UnitPerMinute = 60;                       // 一分の定義(秒)
    const string Crosshair = "Crosshair";
    const string RootElement = "RootElement";
    const string TimerLabelString = "TimerLabel";

    void OnEnable()
    {
        gameUI.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        gameUI.UnregisterUIReloadCallback(OnUIReload);
    }

    public void SwitchUI()
    {
        overviewUI.enabled = false;
        rootElement.style.display = DisplayStyle.Flex;
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

        CrossHair = root.Q<VisualElement>(Crosshair);
        rootElement = root.Q<VisualElement>(RootElement);
        timerLabel = root.Q<Label>(TimerLabelString);
    }

    /// <summary>
    /// タイマーUIの更新処理
    /// </summary>
    /// <param name="time">現在の残り時間(秒)</param>
    public void UpdateTimerText(int time)
    {
        // タイマー描画用のフォントの色を設定(閾値以下: 赤色 / それ以外: 白色)
        timerLabel.style.color = time <= redThredhouldTime ? Color.red : Color.white;

        // タイマーの分と秒を算出
        int min = time / UnitPerMinute;
        int sec = time % UnitPerMinute;

        // Labelに反映
        timerLabel.text = $"{min:00}:{sec:00}";
    }
}
