using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    public static Camera MainCamera { get; private set; }
    CinemachineCamera playerCamera;

    /// <summary>
    /// Cinemachineカメラが自身を追従するように設定
    /// </summary>
    public void InitializePlayerCamera()
    {
        // メインカメラの検索
        MainCamera = Camera.main;

        // コンポーネント取得
        playerCamera = FindAnyObjectByType<CinemachineCamera>();

        // 追従させる
        playerCamera.Follow = transform;
    }
}