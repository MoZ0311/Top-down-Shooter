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
        if (inputAxis == Vector2.zero)
        {
            playerVelocity = Vector3.zero;
            return;
        }

        Transform cameraTransform = PlayerCamera.MainCamera.transform;

        Vector3 right = cameraTransform.right * inputAxis.x;
        Vector3 forward = cameraTransform.forward * inputAxis.y;
        Vector3 direction = right + forward;
        direction.y = 0.0f;
        playerVelocity = direction.normalized * status.MoveSpeed;
    }

    public void ApplyMovement()
    {
        playerRigidbody.linearVelocity = new(playerVelocity.x, playerRigidbody.linearVelocity.y, playerVelocity.z);
    }
}