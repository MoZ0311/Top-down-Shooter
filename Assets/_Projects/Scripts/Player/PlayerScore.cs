using UnityEngine;
using Unity.Netcode;

public class PlayerScore : NetworkBehaviour
{
    [Header("ScoreSO")]
    [SerializeField] PlayerScoreSO playerScore;

    [HideInInspector] public NetworkVariable<int> killCount = new();
    [HideInInspector] public NetworkVariable<int> deathCount = new();

    void OnScoreChanged(int prevValue, int newValue)
    {
        playerScore.killCount = killCount.Value;
        playerScore.deathCount = deathCount.Value;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerScore.Reset();
            killCount.OnValueChanged += OnScoreChanged;
            deathCount.OnValueChanged += OnScoreChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            killCount.OnValueChanged -= OnScoreChanged;
            deathCount.OnValueChanged -= OnScoreChanged;
        }
    }
}
