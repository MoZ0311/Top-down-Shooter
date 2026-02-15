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

    /// <summary>
    /// ホストとして開始(つくるボタン)したときの処理
    /// </summary>
    async void OnClickedHostButton()
    {
        OnConnected();
        if (!await matchingManager.StartHost())
        {
            OnFailedConnection();
        }
    }

    /// <summary>
    /// クライアントとして開始(さがすボタン)したときの処理
    /// </summary>
    async void OnClickedClientButton()
    {
        OnConnected();
        if (!await matchingManager.StartClient())
        {
            OnFailedConnection();
        }
    }

    /// <summary>
    /// 接続時、UIへのアクセスを禁止する処理
    /// </summary>
    void OnConnected()
    {
        connectingMessageLabel.style.display = DisplayStyle.Flex;
        connectingMessageLabel.text = ConnectingText;
        hostButton.SetEnabled(false);
        clientButton.SetEnabled(false);
    }

    /// <summary>
    /// 接続失敗時、UIへのアクセスを解禁する処理
    /// </summary>
    void OnFailedConnection()
    {
        connectingMessageLabel.text = FailedText;
        hostButton.SetEnabled(true);
        clientButton.SetEnabled(true);
    }

    void OnEnable()
    {
        // UI要素の検索/取得
        var root = titleUI.rootVisualElement;
        hostButton = root.Q<Button>(HostButton);
        clientButton = root.Q<Button>(ClientButton);
        connectingMessageLabel = root.Q<Label>(ConnectingMessageLabel);

        hostButton.clicked += OnClickedHostButton;
        clientButton.clicked += OnClickedClientButton;
    }

    void OnDisable()
    {
        hostButton.clicked -= OnClickedHostButton;
        clientButton.clicked -= OnClickedClientButton;
    }
}
