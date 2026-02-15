using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class GameTimer : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int timerLimit;                    // 制限時間(秒)
    [SerializeField] int redThredhouldTime;             // タイマーを赤色にする閾値

    [Header("Components")]
    [SerializeField] UIDocument gameUI;                 // 参照するゲーム画面のUIDocument

    Label timerLabel;                                   // 残り時間を表示するラベル
    readonly NetworkVariable<double> endTime = new(0);  // ゲームの終了時刻
    int displayTime;                                    // ラベルに表示する時間
    int prevDisplayTime;                                // ラベルに直前まで表示されていた時間
    bool hasFinishedGame;

    const int UnitPerMinute = 60;                       // 一分の定義(秒)
    const string TimerLabelString = "TimerLabel";
    const string ResultScene = "ResultScene";

    void OnEnable()
    {
        // UI要素の検索
        var root = gameUI.rootVisualElement;
        timerLabel = root.Q<Label>(TimerLabelString);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // サーバー側で、終了時刻を設定
            endTime.Value = NetworkManager.Singleton.ServerTime.Time + timerLimit;
        }
        hasFinishedGame = false;
    }

    void Update()
    {
        // endTimeが初期化されるまでは、何もしない。
        // NetworkManagerが消えた時も同様
        if (endTime.Value <= 0 || NetworkManager.Singleton == null || hasFinishedGame)
        {
            return;
        }

        // 残り時間を現在時刻と終了時刻から算出
        double remainingTime = endTime.Value - NetworkManager.Singleton.ServerTime.Time;

        if (remainingTime <= 0)
        {
            remainingTime = 0;

            // サーバー側からシーン遷移を行う
            if (IsServer)
            {
                RankingManager.Instance.UpdateRanksServer();
                NetworkManager.Singleton.SceneManager.LoadScene(ResultScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            hasFinishedGame = true;
        }

        // タイマー描画用のint型の値を作成
        displayTime = Mathf.CeilToInt((float)remainingTime);

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
