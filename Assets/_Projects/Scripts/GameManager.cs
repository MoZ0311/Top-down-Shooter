using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [SerializeField] NetworkObject playerPrefab;
    [SerializeField] Transform[] spawnPositions;
    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            OnClientConnected(client.ClientId);
        }
    }

    void OnClientConnected(ulong clientID)
    {
        Debug.Log($"{clientID}: connected!");
        var spawnPosition = spawnPositions[clientID].position;
        NetworkObject playerObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        playerObject.SpawnAsPlayerObject(clientID);
    }
}
