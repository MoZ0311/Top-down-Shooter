using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [Header("Scripts")]
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerRotation playerRotation;
    [SerializeField] PlayerShoot playerShot;

    Vector2 inputAxis;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // スポーン時の処理
            playerCamera.InitializePlayerCamera();
        }

        // シーン破棄と同時に自信を破壊するように設定
        NetworkObject.DestroyWithScene = true;
    }

    void Update()
    {
        if (IsOwner && playerHealth.CurrentHealth.Value > 0)
        {
            // クライアントのマウス座標を取得し、回転に適用
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            playerRotation.HandleRotation(mousePosition);

            // プレイヤーの移動を実行
            playerMovement.HandleMovement(inputAxis);

            // アニメーションを制御/再生
            playerAnimation.HandleAnimation(inputAxis);

            // 射撃制御
            playerShot.HandleShoot();
        }
    }

    void FixedUpdate()
    {
        // 速度を適用
        if (IsOwner)
        {
            playerMovement.ApplyMovement();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            inputAxis = context.ReadValue<Vector2>();
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            playerShot.IsShooting = context.performed;
        }
    }
}