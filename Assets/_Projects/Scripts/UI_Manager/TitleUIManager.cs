using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] UIDocument titleUI;
    VisualElement titleElements;
    Button hostButton;
    Button clientButton;
    Label connectingMessageLabel;
    const string TitleElementsString = "TitleElements";
    const string HostButtonString = "HostButton";
    const string ClientButtonString = "ClientButton";
    const string GameSceneString = "GameScene";
    const string ConnectingMessageLabelString = "ConnectingMessageLabel";
    const string LobbyString = "lobby";
    void Awake()
    {
        var root = titleUI.rootVisualElement;
        titleElements = root.Q<VisualElement>(TitleElementsString);
        hostButton = root.Q<Button>(HostButtonString);
        clientButton = root.Q<Button>(ClientButtonString);
        connectingMessageLabel = root.Q<Label>(ConnectingMessageLabelString);

        hostButton.clicked += OnClickedHostButton;
        clientButton.clicked += OnClickedClientButton;
    }

    void OnClickedHostButton()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("ホストの開始に成功");
            hostButton.SetEnabled(false);
            clientButton.SetEnabled(false);
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneString, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("ホストの開始に失敗");
        }
    }

    void OnClickedClientButton()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("クライアント接続に成功");
            hostButton.SetEnabled(false);
            clientButton.SetEnabled(false);
        }
    }

    void SwithLobby()
    {
        connectingMessageLabel.style.display = DisplayStyle.Flex;
        titleElements.AddToClassList(LobbyString);
    }
}
