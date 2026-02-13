using UnityEngine;

public class ItemBillboard : MonoBehaviour
{
    void LateUpdate()
    {
        // 常にカメラの正面を向かせる
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}