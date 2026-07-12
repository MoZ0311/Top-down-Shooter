using UnityEngine;
using Unity.Cinemachine;
using System;

public enum CameraMode
{
    Overview,
    Player,
    Kill
}

public class CameraManager : MonoBehaviour
{
    // シングルトン用のインスタンス
    public static CameraManager Instance { get; private set; } = null;

    [field:Header("Cameras")]
    [field:SerializeField] public CinemachineCamera OverviewCamera { get; private set;}
    [field:SerializeField] public CinemachineCamera PlayerCamera { get; private set;}
    [field:SerializeField] public CinemachineCamera KillCamera { get; private set;}

    [Header("Settings")]
    [SerializeField] float sensitivity;

    [Header("Components")]
    [SerializeField] CinemachineFollowZoom playerFollowZoom;

    float initialWidth;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        initialWidth = playerFollowZoom.Width;
    }

    void Update()
    {
        if (PlayerCamera.Follow == null)
        {
            return;
        }
        float playerScale = PlayerCamera.Follow.transform.localScale.x;

        float zoomMultiplier = 1 + (playerScale - 1) * sensitivity;
        playerFollowZoom.Width = initialWidth * zoomMultiplier;
    }

    public void SwitchCamera(CameraMode mode)
    {
        OverviewCamera.Priority = 0;
        PlayerCamera.Priority = 0;
        KillCamera.Priority = 0;

        switch (mode)
        {
            case CameraMode.Overview:
                OverviewCamera.Priority = 1;
                break;

            case CameraMode.Player:
                PlayerCamera.Priority = 1;
                break;

            case CameraMode.Kill:
                KillCamera.Priority = 1;
                break;
        }
    }
}
