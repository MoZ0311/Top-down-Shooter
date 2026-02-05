using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameterSO", menuName = "Scriptable Objects/PlayerParameterSO")]
public class PlayerParameterSO : ScriptableObject
{
    [Header("Player Settings")]
    [field:SerializeField] public float Health { get; private set; }        // 体力
    [field:SerializeField] public float MoveSpeed { get; private set; }     // 移動速度

    [Header("Bullet Settings")]
    [field:SerializeField] public float AttackPower { get; private set; }   // 攻撃力
    [field:SerializeField] public float BulletSpeed { get; private set; }   // 弾速
    [field:SerializeField] public float FireRate { get; private set; }      // 連射速度(発/s)
    [field:SerializeField] public bool CanRapidFire { get; private set; }   // 自動で連射できるか
    [field:SerializeField] public float ReloadTime { get; private set; }    // リロード時間
}