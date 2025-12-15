using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [Header("Scripts")]
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerRotation playerRotation;
    [SerializeField] PlayerShot playerShot;

    Vector2 inputAxis;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // スポーン時の処理
            playerCamera.InitializePlayerCamera();
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            // クライアントのマウス座標を取得
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            playerMovement.HandleMovement(inputAxis);
            playerRotation.HandleRotation(mousePosition);
        }

        if (IsServer)
        {
            playerShot.HandleShot();
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
            SetShootingServerRpc(context.performed);
        }
    }

    [ServerRpc]
    void SetShootingServerRpc(bool isShooting)
    {
        playerShot.IsShooting = isShooting;
    }
}
