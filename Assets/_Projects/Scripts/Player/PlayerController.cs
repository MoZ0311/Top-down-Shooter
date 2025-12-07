using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerRotation playerRotation;

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
            UpdateServerRpc(inputAxis, mousePosition);
        }
    }

    void FixedUpdate()
    {
        // 速度を適用
        if (IsServer)
        {
            playerMovement.ApplyVelocity();
        }
    }

    [ServerRpc]
    void UpdateServerRpc(Vector2 inputAxis, Vector2 mousePosition)
    {
        playerMovement.HandleMovement(inputAxis);
        playerRotation.HandleRotation(mousePosition);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputAxis = context.ReadValue<Vector2>();
    }
}
