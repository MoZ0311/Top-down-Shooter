using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float rotationSpeed;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] Transform muzzle;

    public void HandleRotation(Vector2 mousePosition)
    {
        Vector3 targetPoint = Vector3.zero;

        // 渡されたマウス座標からレイを計算
        Ray ray = PlayerCamera.MainCamera.ScreenPointToRay(mousePosition);

        // Physics.Raycastでコライダーとの衝突を検知
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, targetLayers))
        {
            if (hitInfo.collider.gameObject == gameObject)
            {
                return;
            }
            targetPoint = hitInfo.point;
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
