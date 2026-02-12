using UnityEngine;
using UnityEngine.UIElements;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] UIDocument titleUI;
    [SerializeField] MatchingManager matchingManager;
    Button hostButton;
    Button clientButton;
    Label connectingMessageLabel;
    const string HostButton = "HostButton";
    const string ClientButton = "ClientButton";
    const string ConnectingMessageLabel = "ConnectingMessageLabel";
    const string ConnectingText = "接続中…";
    const string FailedText = "接続に失敗しました";
    void Awake()
    {
        var root = titleUI.rootVisualElement;
        hostButton = root.Q<Button>(HostButton);
        clientButton = root.Q<Button>(ClientButton);
        connectingMessageLabel = root.Q<Label>(ConnectingMessageLabel);
    }

    async void OnClickedHostButton()
    {
        OnConnected();
        if (!await matchingManager.StartHost())
        {
            OnFailedConnection();
        }
    }

    async void OnClickedClientButton()
    {
        OnConnected();
        if (!await matchingManager.StartClient())
        {
            OnFailedConnection();
        }
    }

    void OnConnected()
    {
        connectingMessageLabel.style.display = DisplayStyle.Flex;
        connectingMessageLabel.text = ConnectingText;
        hostButton.SetEnabled(false);
        clientButton.SetEnabled(false);
    }

    void OnFailedConnection()
    {
        connectingMessageLabel.text = FailedText;
        hostButton.SetEnabled(true);
        clientButton.SetEnabled(true);
    }

    void OnEnable()
    {
        hostButton.clicked += OnClickedHostButton;
        clientButton.clicked += OnClickedClientButton;
    }

    void OnDisable()
    {
        hostButton.clicked -= OnClickedHostButton;
        clientButton.clicked -= OnClickedClientButton;
    }
}
