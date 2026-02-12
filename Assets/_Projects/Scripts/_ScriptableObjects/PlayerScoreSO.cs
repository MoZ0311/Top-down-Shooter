using UnityEngine;

[CreateAssetMenu(fileName = "PlayerScoreSO", menuName = "Scriptable Objects/PlayerScoreSO")]
public class PlayerScoreSO : ScriptableObject
{
    [HideInInspector] public int killCount;
    [HideInInspector] public int deathCount;
    [HideInInspector] public int maxLevel;
    [HideInInspector] public int finishLevel;
    [HideInInspector] public int rank;

    public void Reset()
    {
        killCount = 0;
        deathCount = 0;
        maxLevel = 0;
        finishLevel = 0;
        rank = 0;
    }
}
