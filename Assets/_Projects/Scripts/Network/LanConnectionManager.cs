using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class LanConnectionManager : MonoBehaviour
{
    const string HostIP = "192.168.11.100";
    const string IPv4 = "0.0.0.0";
    const ushort Port = 7777;

    // 遷移先
    const string LobbyScene = "LobbyScene";

    UnityTransport transport;

    void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

    public bool StartHost()
    {
        transport.SetConnectionData(
            IPv4,
            Port
        );

        bool result = NetworkManager.Singleton.StartHost();

        if (result)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(LobbyScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("Host Start Failed");
        }
        return result;
    }

    public bool StartClient()
    {
        transport.SetConnectionData(
            HostIP,
            Port
        );

        return NetworkManager.Singleton.StartClient();
    }
}
