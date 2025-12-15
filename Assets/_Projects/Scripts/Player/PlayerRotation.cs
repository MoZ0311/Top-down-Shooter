using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float rotationSpeed = 10.0f;

    public void HandleRotation(Vector2 mousePosition)
    {
        // プレイヤーの中心点を通る、地面と平行な無限平面を計算
        Plane plane = new(Vector3.up, transform.position);

        // 渡されたマウス座標からレイを計算
        Ray ray = PlayerCamera.MainCamera.ScreenPointToRay(mousePosition);

        // PlaneとRayの交点を求め、そこに向けるQuatenion作成
        if (plane.Raycast(ray, out float hitDistance))
        {
            Vector3 targetPoint = ray.GetPoint(hitDistance);
            Vector3 direction = (targetPoint - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
