using UnityEngine;
using UnityEngine.UIElements;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] UIDocument titleUI;
    [SerializeField] MatchingManager matchingManager;
    Button hostButton;
    Button clientButton;
    Label connectingMessageLabel;
    const string HostButtonString = "HostButton";
    const string ClientButtonString = "ClientButton";
    const string ConnectingMessageLabelString = "ConnectingMessageLabel";
    const string ConnectingText = "接続中…";
    const string FailedText = "接続に失敗しました";
    void Awake()
    {
        var root = titleUI.rootVisualElement;
        hostButton = root.Q<Button>(HostButtonString);
        clientButton = root.Q<Button>(ClientButtonString);
        connectingMessageLabel = root.Q<Label>(ConnectingMessageLabelString);

        hostButton.clicked += OnClickedHostButton;
        clientButton.clicked += OnClickedClientButton;
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
}
