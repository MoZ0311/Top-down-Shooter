using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static Camera MainCamera { get; private set; } = null;

    /// <summary>
    /// Cinemachineカメラが自身を追従するように設定
    /// </summary>
    public void InitializePlayerCamera()
    {
        // メインカメラの検索
        MainCamera = Camera.main;

        // 追従させる
        CameraManager.Instance.PlayerCamera.Follow = transform;
    }
}