using UnityEngine;
using Unity.Cinemachine;

public class BulletEffect : MonoBehaviour
{
    [Header("ImpulseSettings")]
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] float force;
    [SerializeField] float screenRatio;

    void OnEnable()
    {
        if (IsPointInCameraView())
        {
            impulseSource.GenerateImpulse(force);
        }
    }

    void OnParticleSystemStopped()
    {
        // 自身をプールに返す
        PoolManager.Instance.EffectPool.Release(this);
    }

    bool IsPointInCameraView()
    {
        // ワールド座標をビューポート座標に変換
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);

        // XとYが0〜1の間、かつZが0より大きい（カメラの前方）かチェック
        bool isInX = viewportPoint.x >= 1 - screenRatio && viewportPoint.x <= screenRatio;
        bool isInY = viewportPoint.y >= 1 - screenRatio && viewportPoint.y <= screenRatio;
        bool isFront = viewportPoint.z > 0;

        // すべてを満たしていれば画面内（true）
        return isInX && isInY && isFront;
    }
}
