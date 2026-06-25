using UnityEngine;
using Unity.Netcode;

public class GameTimer : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int timerLimit;                    // 制限時間(秒)

    [Header("Scripts")]
    [SerializeField] GameUIManager gameUIManager;       // ゲームUIの管理クラス

    readonly NetworkVariable<double> endTime = new(0);  // ゲームの終了時刻
    int displayTime;                                    // ラベルに表示する時間
    int prevDisplayTime;                                // ラベルに直前まで表示されていた時間

    void Update()
    {
        // endTimeが初期化されるまでは、何もしない。
        // NetworkManagerが消えた時も同様
        if (endTime.Value <= 0 || NetworkManager.Singleton == null || !GameManager.Instance.CanPlayingGame)
        {
            return;
        }

        // 残り時間を現在時刻と終了時刻から算出
        double remainingTime = endTime.Value - NetworkManager.Singleton.ServerTime.Time;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GameManager.Instance.FinishGame();
        }

        // タイマー描画用のint型の値を作成
        displayTime = Mathf.CeilToInt((float)remainingTime);

        // タイマー描画用ラベルのテキスト更新
        if (displayTime != prevDisplayTime)
        {
            prevDisplayTime = displayTime;
            gameUIManager.UpdateTimerText(displayTime);
        }
    }

    /// <summary>
    /// ゲーム終了に向けたカウントダウンの開始処理
    /// </summary>
    public void StartCountdown()
    {
        if (IsServer)
        {
            // サーバー側で、終了時刻を設定
            endTime.Value = NetworkManager.Singleton.ServerTime.Time + timerLimit;
        }
    }
}
