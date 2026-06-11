using UnityEngine;
using Unity.Cinemachine;

public enum CameraMode
{
    Overview,
    Player,
    Kill
}

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    [field:SerializeField] public CinemachineCamera OverviewCamera { get; private set;}
    [field:SerializeField] public CinemachineCamera PlayerCamera { get; private set;}
    [field:SerializeField] public CinemachineCamera KillCamera { get; private set;}

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
