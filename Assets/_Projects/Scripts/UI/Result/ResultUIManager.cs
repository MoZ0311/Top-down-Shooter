using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

public class ResultUIManager : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] UIDocument resultUI;

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

    void OnEnable()
    {
        // カーソルを表示する
        UnityEngine.Cursor.visible = true;

        // 各UI要素の取得
        var root = resultUI.rootVisualElement;
        killCountLabel = root.Q<Label>(KillCountLabel);
        deathCountLabel = root.Q<Label>(DeathCountLabel);
        maxLevelLabel = root.Q<Label>(MaxLevelLabel);
        finishLevelLabel = root.Q<Label>(FinishLevelLabel);
        rankLabel = root.Q<Label>(RankLabel);
        waitingTextLabel = root.Q<Label>(WaitingTextLabel);
        okButton = root.Q<Button>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            waitingTextLabel.style.display = DisplayStyle.None;
            okButton.style.display = DisplayStyle.Flex;
            okButton.clicked += OnClickedOK;
        }

        killCountLabel.text = playerScore.killCount.ToString();
        deathCountLabel.text = playerScore.deathCount.ToString();
        maxLevelLabel.text = playerScore.maxLevel.ToString();
        finishLevelLabel.text = playerScore.finishLevel.ToString();
        rankLabel.text = playerScore.rank.ToString() + "位";
    }

    public override void OnNetworkDespawn()
    {
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
