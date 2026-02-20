using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameterSO", menuName = "Scriptable Objects/PlayerParameterSO")]
public class PlayerParameterSO : ScriptableObject
{
    [Header("Player Settings")]
    [SerializeField] float health;               // 体力
    [SerializeField] float moveSpeed;            // 移動速度

    public float Health => health;
    public float MoveSpeed => moveSpeed;

    [Header("Bullet Settings")]
    [SerializeField] float attackPower;           // 攻撃力
    [SerializeField] float bulletSpeed;           // 弾速
    [SerializeField] float fireRate;              // 連射速度(発/s)
    [SerializeField] bool canRapidFire;           // 自動で連射できるか
    [SerializeField] float reloadTime;            // リロード時間

    public float AttackPower => attackPower;
    public float BulletSpeed => bulletSpeed;
    public float FireRate => fireRate;
    public bool CanRapidFire => canRapidFire;
    public float ReloadTime => reloadTime;

    [Header("GrowthRate Settings")]
    [SerializeField] float scaleGrowthRate;       // 大きさの成長率
    [SerializeField] float healthGrowthRate;      // 体力の成長率
    [SerializeField] float moveSpeedGrowthRate;   // 移動速度の成長率
    [SerializeField] float attackPowerGrowthRate; // 攻撃力の成長率
    [SerializeField] float bulletSpeedGrowthRate; // 弾速の成長率
    [SerializeField] float fireRateGrowthRate;    // 連射速度の成長率
    [SerializeField] float reloadTimeGrowthRate;  // リロード時間の成長率

    public float ScaleGrowthRate => scaleGrowthRate;
    public float HealthGrowthRate => healthGrowthRate;
    public float MoveSpeedGrowthRate => moveSpeedGrowthRate;
    public float AttackPowerGrowthRate => attackPowerGrowthRate;
    public float BulletSpeedGrowthRate => bulletSpeedGrowthRate;
    public float FireRateGrowthRate => fireRateGrowthRate;
    public float ReloadTimeGrowthRate => reloadTimeGrowthRate;
}