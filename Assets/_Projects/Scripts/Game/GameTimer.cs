using UnityEngine;
using UnityEngine.UIElements;

public class GameTimer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int timerLimit;        // 制限時間
    [SerializeField] int redThredhouldTime; // タイマーを赤色にする閾値

    [Header("Components")]
    [SerializeField] UIDocument gameUI;

    Label timerLabel;
    float remainingTime;                    // 残り時間
    int displayTime;                        // ラベルに表示する時間
    int prevDisplayTime;                    // ラベルに直前まで表示されていた時間

    const int UnitPerMinute = 60;           // 一分の定義(秒)
    const string TimerLabelString = "TimerLabel";

    void Awake()
    {
        var root = gameUI.rootVisualElement;
        timerLabel = root.Q<Label>(TimerLabelString);

        // タイマーの初期値を設定
        remainingTime = timerLimit;
    }

    void Update()
    {
        // タイマー0以下で早期return
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            return;
        }

        // float型のタイマーをカウントダウン
        remainingTime -= Time.deltaTime;

        // タイマー描画用のint型の値を作成
        displayTime = Mathf.CeilToInt(remainingTime);

        // タイマー描画用ラベルのテキスト更新
        if (displayTime != prevDisplayTime)
        {
            prevDisplayTime = displayTime;
            UpdateTimerText(displayTime);
        }
    }

    /// <summary>
    /// タイマーUIの更新処理
    /// </summary>
    /// <param name="time">現在の残り時間(秒)</param>
    void UpdateTimerText(int time)
    {
        // タイマー描画用のフォントの色を設定(閾値以下: 赤色 / それ以外: 白色)
        timerLabel.style.color = displayTime <= redThredhouldTime ? Color.red : Color.white;

        // タイマーの分と秒を算出
        int min = time / UnitPerMinute;
        int sec = time % UnitPerMinute;

        // Labelに反映
        timerLabel.text = $"{min:00}:{sec:00}";
    }
}
