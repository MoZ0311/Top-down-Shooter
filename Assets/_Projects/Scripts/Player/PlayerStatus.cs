using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float scaleGrowthRate;
    [SerializeField] float healthGrowthRate;
    [SerializeField] float moveSpeedGrowthRate;
    [SerializeField] float attackPowerGrowthRate;
    [SerializeField] float bulletSpeedGrowthRate;
    [SerializeField] float fireRateGrowthRate;
    [SerializeField] float reloadTimeGrowthRate;

    [Header("ParameterSO")]
    [SerializeField] PlayerParameterSO defaultParameter;        // 参照する初期値
    public float Health { get; private set; }                   // 体力
    public float MoveSpeed { get; private set; }                // 移動速度
    public float AttackPower { get; private set; }              // 攻撃力
    public float BulletSpeed { get; private set; }              // 弾速
    public float FireRate { get; private set; }                 // 連射速度
    public bool CanRapidFire => defaultParameter.CanRapidFire;  // 連射できるかどうか
    public float ReloadTime { get; private set; }               // リロード時間

    const float BaseMultiplier = 1.0f;

    public void UpdateStatus(int level)
    {
        // レベルアップに伴い、各ステータスを乗算して再計算
        transform.localScale = Vector3.one * CalculateGrowth(scaleGrowthRate, level);
        Health = defaultParameter.Health * CalculateGrowth(healthGrowthRate, level);
        MoveSpeed = defaultParameter.MoveSpeed * CalculateGrowth(moveSpeedGrowthRate, level);
        AttackPower = defaultParameter.AttackPower * CalculateGrowth(attackPowerGrowthRate, level);
        BulletSpeed = defaultParameter.BulletSpeed * CalculateGrowth(bulletSpeedGrowthRate, level);
        FireRate = defaultParameter.FireRate * CalculateGrowth(fireRateGrowthRate, level);
        ReloadTime = defaultParameter.ReloadTime * CalculateGrowth(reloadTimeGrowthRate, level);
    }

    /// <summary>
    /// レベルに応じた成長倍率を計算する
    /// </summary>
    float CalculateGrowth(float rate, int level)
    {
        // (レベル - 1)をステータス計算の係数とする
        float growthSteps = level - 1;

        // 基礎倍率 + (係数 * 成長率)で目的のステータスを算出
        return BaseMultiplier + (growthSteps * rate);
    }
}