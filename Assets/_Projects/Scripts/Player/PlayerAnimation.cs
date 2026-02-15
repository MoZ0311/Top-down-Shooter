using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float dampTime;    // アニメーション遷移時の補正値

    [Header("Components")]
    [SerializeField] Animator animator;

    const string MoveX = "moveX";
    const string MoveZ = "moveZ";

    /// <summary>
    /// アニメーションの遷移処理
    /// </summary>
    /// <param name="inputAxis">入力された方向ベクトル</param>
    public void HandleAnimation(Vector2 inputAxis)
    {
        // 入力をプレイヤーの向きで補正
        Vector3 inputDirection = new(inputAxis.x, 0, inputAxis.y);
        Vector3 localDirection = transform.InverseTransformDirection(inputDirection);

        // さらにdampTimeで値の変動を緩やかに補正
        animator.SetFloat(MoveX, localDirection.x, dampTime,Time.deltaTime);
        animator.SetFloat(MoveZ, localDirection.z, dampTime,Time.deltaTime);
    }
}
