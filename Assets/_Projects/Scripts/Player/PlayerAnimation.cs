using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator animator;

    const string MoveX = "moveX";
    const string MoveZ = "moveZ";

    public void HandleAnimation(Vector2 inputAxis)
    {
        Vector3 inputDirection = new(inputAxis.x, 0, inputAxis.y);
        Vector3 localDirection = transform.InverseTransformDirection(inputDirection);
        animator.SetFloat(MoveX, localDirection.x);
        animator.SetFloat(MoveZ, localDirection.z);
    }
}
