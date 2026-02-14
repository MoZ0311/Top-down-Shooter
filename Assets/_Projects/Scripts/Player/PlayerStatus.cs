using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("ParameterSO")]
    [SerializeField] PlayerParameterSO defaultParameter;    // 参照する初期値

    [Header("Ref Level")]
    [SerializeField] PlayerLevel playerLevel;               // 参照するレベル
    public float Health { get; private set; }               // 体力
    public float MoveSpeed { get; private set; }            // 移動速度
    public float AttackPower { get; private set; }          // 攻撃力
    public float BulletSpeed { get; private set; }          // 弾速
    public float FireRate { get; private set; }             // 連射速度
    public bool CanRapidFire { get; private set; }          // 連射できるかどうか
    public float ReloadTime { get; private set; }           // リロード時間
    void Awake()
    {
        Health = defaultParameter.Health;
        MoveSpeed = defaultParameter.MoveSpeed;
        AttackPower = defaultParameter.AttackPower;
        BulletSpeed = defaultParameter.BulletSpeed;
        FireRate = defaultParameter.FireRate;
        CanRapidFire = defaultParameter.CanRapidFire;
        ReloadTime = defaultParameter.ReloadTime;
    }
}