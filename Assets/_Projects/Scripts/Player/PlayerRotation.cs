using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float rotationSpeed;       // マウスへの追従回転速度
    [SerializeField] LayerMask targetLayers;    // 照準できるレイヤー
    [SerializeField] Transform muzzle;          // 銃口の位置

    public void HandleRotation(Vector2 mousePosition)
    {
        Vector3 targetPoint = Vector3.zero;

        // 渡されたマウス座標からレイを計算
        Ray ray = PlayerCamera.MainCamera.ScreenPointToRay(mousePosition);

        // Physics.Raycastでコライダーとの衝突を検知
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, targetLayers, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.collider.gameObject == gameObject)
            {
                // 自身に照準している時、return
                return;
            }

            // 他オブジェクトに照準している時、その中心に補正
            targetPoint = hitInfo.collider.bounds.center;
        }
        else
        {
            // 銃口の高さをもとに、地面と平行な無限平面を計算
            Plane plane = new(Vector3.up, new Vector3(0, muzzle.position.y, 0));

            // PlaneとRayの交点を求める
            if (plane.Raycast(ray, out float hitDistance))
            {
                targetPoint = ray.GetPoint(hitDistance);
            }
        }

        // 高さを補正してピッチを0にする
        targetPoint.y = transform.position.y;

        // 向きベクトルの作成
        Vector3 direction = (targetPoint - transform.position).normalized;

        // 念のため0ベクトルのチェック
        if (direction != Vector3.zero)
        {
            // Quaternionに適用
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
