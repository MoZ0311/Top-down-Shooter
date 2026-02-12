using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ResultUIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] UIDocument resultUI;

    [Header("Ref Score")]
    [SerializeField] PlayerScoreSO playerScore;
    Label killCountLabel;
    Label deathCountLabel;
    Label maxLevelLabel;
    Label finishLevelLabel;
    Button okButton;
    const string KillCountLabel = "KillCountLabel";
    const string DeathCountLabel = "DeathCountLabel";
    const string MaxLevelLabel = "MaxLevelLabel";
    const string FinishLevelLabel = "FinishLevelLabel";

    void Awake()
    {
        // カーソルを表示する
        UnityEngine.Cursor.visible = true;

        var root = resultUI.rootVisualElement;
        killCountLabel = root.Q<Label>(KillCountLabel);
        deathCountLabel = root.Q<Label>(DeathCountLabel);
        maxLevelLabel = root.Q<Label>(MaxLevelLabel);
        finishLevelLabel = root.Q<Label>(FinishLevelLabel);
        okButton = root.Q<Button>();

        killCountLabel.text = playerScore.killCount.ToString();
        deathCountLabel.text = playerScore.deathCount.ToString();
        maxLevelLabel.text = playerScore.maxLevel.ToString();
        finishLevelLabel.text = playerScore.finishLevel.ToString();
    }

    void OnClickedOK()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void OnEnable()
    {
        okButton.clicked += OnClickedOK;
    }

    void OnDisable()
    {
        okButton.clicked -= OnClickedOK;
    }
}
