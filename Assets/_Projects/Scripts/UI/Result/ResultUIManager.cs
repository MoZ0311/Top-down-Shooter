using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class ResultUIManager : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] PanelRenderer resultUI;

    [Header("Ref Score")]
    [SerializeField] PlayerScoreSO playerScore;
    Label killCountLabel;
    Label deathCountLabel;
    Label maxLevelLabel;
    Label finishLevelLabel;
    Label rankLabel;
    Label waitingTextLabel;
    Button okButton;
    const string KillCountLabel = "KillCountLabel";
    const string DeathCountLabel = "DeathCountLabel";
    const string MaxLevelLabel = "MaxLevelLabel";
    const string FinishLevelLabel = "FinishLevelLabel";
    const string RankLabel = "RankLabel";
    const string WaitingTextLabel = "WaitingTextLabel";
    const string LobbyScene = "LobbyScene";
    const string Rank = "位";

    void Awake()
    {
        resultUI.RegisterUIReloadCallback(OnUIReload);
    }

    /// <summary>
    /// UIを再構成するコールバック
    /// </summary>
    void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
    {
        killCountLabel = root.Q<Label>(KillCountLabel);
        deathCountLabel = root.Q<Label>(DeathCountLabel);
        maxLevelLabel = root.Q<Label>(MaxLevelLabel);
        finishLevelLabel = root.Q<Label>(FinishLevelLabel);
        rankLabel = root.Q<Label>(RankLabel);
        waitingTextLabel = root.Q<Label>(WaitingTextLabel);
        okButton = root.Q<Button>();

        if (IsServer)
        {
            // 待ちテキストを非表示
            waitingTextLabel.style.display = DisplayStyle.None;

            // ボタン表示
            okButton.style.display = DisplayStyle.Flex;

            // 押下時のイベント登録
            okButton.clicked += OnClickedOK;
        }

        killCountLabel.text = playerScore.killCount.ToString();
        deathCountLabel.text = playerScore.deathCount.ToString();
        maxLevelLabel.text = playerScore.maxLevel.ToString();
        finishLevelLabel.text = playerScore.finishLevel.ToString();
        rankLabel.text = playerScore.rank.ToString() + Rank;

        // 歓声のSE再生
        AudioPlayer.Instance.PlaySE("cheers");
    }

    void OnEnable()
    {
        // カーソルを表示する
        UnityEngine.Cursor.visible = true;
    }

    public override void OnNetworkDespawn()
    {
        resultUI.UnregisterUIReloadCallback(OnUIReload);

        if (IsServer)
        {
            okButton.clicked -= OnClickedOK;
        }
    }

    void OnClickedOK()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(LobbyScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
