using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ref Status")]
    [SerializeField] PlayerStatus status;

    [Header("Components")]
    [SerializeField] Rigidbody playerRigidbody;
    Vector3 playerVelocity;

    public void HandleMovement(Vector2 inputAxis)
    {
        // 入力がなければ、早期return
        if (inputAxis == Vector2.zero)
        {
            playerVelocity = Vector3.zero;
            return;
        }

        // カメラの位置を検索して取得
        Transform cameraTransform = PlayerCamera.MainCamera.transform;

        Vector3 right = cameraTransform.right * inputAxis.x;
        Vector3 forward = cameraTransform.forward * inputAxis.y;
        Vector3 direction = right + forward;

        // y(上下)方向の移動は0にしておく
        direction.y = 0.0f;

        // 新しい速度として格納
        playerVelocity = direction.normalized * status.MoveSpeed;
    }

    /// <summary>
    /// Rigidbodyコンポーネントに速度を適用する
    /// </summary>
    public void ApplyMovement()
    {
        playerRigidbody.linearVelocity = new(playerVelocity.x, playerRigidbody.linearVelocity.y, playerVelocity.z);
    }
}