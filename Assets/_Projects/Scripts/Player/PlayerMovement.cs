using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5.0f;
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
        Vector3 movementVector = right + forward;
        movementVector.y = 0.0f;
        playerVelocity = movementVector.normalized * movementSpeed;
    }

    public void ApplyMovement()
    {
        playerRigidbody.linearVelocity = new(playerVelocity.x, playerRigidbody.linearVelocity.y, playerVelocity.z);
    }
}
