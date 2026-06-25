using UnityEngine;
using UnityEngine.UIElements;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] PanelRenderer titleUI;
    MatchingManager matchingManager;
    Button hostButton;
    Button clientButton;
    Label connectingMessageLabel;
    int uiVersion;
    const string HostButton = "HostButton";
    const string ClientButton = "ClientButton";
    const string ConnectingMessageLabel = "ConnectingMessageLabel";
    const string ConnectingText = "接続中…";
    const string FailedText = "接続に失敗しました";

    void Awake()
    {
        titleUI.RegisterUIReloadCallback(OnUIReload);
    }

    void Start()
    {
        matchingManager = FindAnyObjectByType<MatchingManager>();
    }

    void OnDestroy()
    {
        titleUI.UnregisterUIReloadCallback(OnUIReload);
    }

    /// <summary>
    /// UIを再構成するコールバック
    /// </summary>
    void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
    {
        if (uiVersion == version)
        {
            return;
        }
        uiVersion = version;

        hostButton = root.Q<Button>(HostButton);
        clientButton = root.Q<Button>(ClientButton);
        connectingMessageLabel = root.Q<Label>(ConnectingMessageLabel);

        hostButton.RegisterCallback<ClickEvent>(OnClickedHostButton);
        clientButton.RegisterCallback<ClickEvent>(OnClickedClientButton);
    }

    /// <summary>
    /// ホストとして開始(つくるボタン)したときの処理
    /// </summary>
    async void OnClickedHostButton(ClickEvent evt)
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
    async void OnClickedClientButton(ClickEvent evt)
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

        // ボタンクリック時の音を出す
        AudioPlayer.Instance.PlaySE("button");
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
}
