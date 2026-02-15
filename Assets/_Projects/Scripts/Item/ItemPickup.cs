using UnityEngine;

public class ItemPickup : Pickup
{
    [SerializeField] int exp;   // 入手経験値

    /// <summary>
    /// 拾われた時の処理
    /// </summary>
    /// <param name="collider">拾った相手のコライダー</param>
    protected override void OnPickedUp(Collider collider)
    {
        // PlayerLevelにアクセスし、経験値を渡す
        if (collider.TryGetComponent(out PlayerLevel component))
        {
            component.PickedExp(exp);
        }
    }
}
