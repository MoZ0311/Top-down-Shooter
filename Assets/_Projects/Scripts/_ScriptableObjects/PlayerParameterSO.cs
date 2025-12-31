using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameterSO", menuName = "Scriptable Objects/PlayerParameterSO")]
public class PlayerParameterSO : ScriptableObject
{
    [Header("Player Settings")]
    [field:SerializeField] public float HitPoint { get; private set; } = 100.0f;
    [field:SerializeField] public float MoveSpeed { get; private set; } = 5.0f;

    [Header("Bullet Settings")]
    [field:SerializeField] public float AttackPower { get; private set; } = 35.0f;
    [field:SerializeField] public float BulletSpeed { get; private set; } = 20.0f;
    [field:SerializeField] public float FireRate { get; private set; } = 5.0f;
    [field:SerializeField] public bool CanRapidFire { get; private set; } = false;
    [field:SerializeField] public float ReloadTime { get; private set; } = 3.5f;
}