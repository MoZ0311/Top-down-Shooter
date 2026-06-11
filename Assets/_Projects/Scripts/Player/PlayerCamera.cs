using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static Camera MainCamera { get; private set; }

    /// <summary>
    /// Cinemachineカメラが自身を追従するように設定
    /// </summary>
    public void InitializePlayerCamera()
    {
        // メインカメラの検索
        MainCamera = Camera.main;

        // コンポーネント取得
        CameraManager cameraManager = FindAnyObjectByType<CameraManager>();

        // 追従させる
        cameraManager.PlayerCamera.Follow = transform;
    }
}