using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    public static Camera MainCamera { get; private set; }
    CinemachineCamera playerCamera;

    public void InitializePlayerCamera()
    {
        MainCamera = Camera.main;
        playerCamera = FindAnyObjectByType<CinemachineCamera>();
        playerCamera.Follow = transform;
    }
}