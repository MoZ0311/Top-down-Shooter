using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody playerRigidbody;

    const string IsIdling = "isIdling";
    void Update()
    {
        Vector2 horizontalVector = new(playerRigidbody.linearVelocity.x, playerRigidbody.linearVelocity.z);
        animator.SetBool(IsIdling, horizontalVector == Vector2.zero);
    }
}
