using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("ParameterSO")]
    [SerializeField] PlayerParameterSO defaultParameter;
    public float HitPoint { get; private set; }
    public float MoveSpeed { get; private set; }
    public float AttackPower { get; private set; }
    public float BulletSpeed { get; private set; }
    public float FireRate { get; private set; }
    public bool CanRapidFire { get; private set; }
    public float ReloadTime { get; private set; }
    void Awake()
    {
        HitPoint = defaultParameter.HitPoint;
        MoveSpeed = defaultParameter.MoveSpeed;
        AttackPower = defaultParameter.AttackPower;
        BulletSpeed = defaultParameter.BulletSpeed;
        FireRate = defaultParameter.FireRate;
        CanRapidFire = defaultParameter.CanRapidFire;
        ReloadTime = defaultParameter.ReloadTime;
    }
}