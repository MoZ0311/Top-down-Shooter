using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [Header("Prefab")]
    [SerializeField] NetworkObject playerPrefab;

    [Header("Settings")]
    [SerializeField] Transform[] spawnPositions;
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
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
        var spawnPosition = spawnPositions[clientID].position;
        NetworkObject playerObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        playerObject.SpawnAsPlayerObject(clientID);
    }
}