using UnityEngine;
using Unity.Netcode;

public class PlayerScore : NetworkBehaviour
{
    [Header("ScoreSO")]
    [SerializeField] PlayerScoreSO playerScore;

    [HideInInspector] public NetworkVariable<int> killCount = new();
    [HideInInspector] public NetworkVariable<int> deathCount = new();
    [HideInInspector] public NetworkVariable<int> maxLevel = new();
    [HideInInspector] public NetworkVariable<int> finishLevel = new();
    [HideInInspector] public NetworkVariable<int> rank = new();

    void Awake()
    {
        if (IsOwner)
        {
            // スコアの初期化
            playerScore.Reset();
        }
    }

    void OnScoreChanged(int prevValue, int newValue)
    {
        playerScore.killCount = killCount.Value;
        playerScore.deathCount = deathCount.Value;
        playerScore.maxLevel = maxLevel.Value;
        playerScore.finishLevel = finishLevel.Value;
        playerScore.rank = rank.Value;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            killCount.OnValueChanged += OnScoreChanged;
            deathCount.OnValueChanged += OnScoreChanged;
            maxLevel.OnValueChanged += OnScoreChanged;
            finishLevel.OnValueChanged += OnScoreChanged;
            rank.OnValueChanged += OnScoreChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            killCount.OnValueChanged -= OnScoreChanged;
            deathCount.OnValueChanged -= OnScoreChanged;
            maxLevel.OnValueChanged -= OnScoreChanged;
            finishLevel.OnValueChanged -= OnScoreChanged;
            rank.OnValueChanged -= OnScoreChanged;
        }
    }
}
