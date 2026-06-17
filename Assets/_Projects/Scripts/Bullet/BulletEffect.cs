using UnityEngine;
using Unity.Cinemachine;

public class BulletEffect : MonoBehaviour
{
    [Header("ImpulseSettings")]
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] float force;
    [Min(0)][SerializeField] float screenRatio;

    [Header("Components")]
    [SerializeField] AudioSource audioSource;

    void OnEnable()
    {
        if (IsPointInCameraView())
        {
            AudioPlayer.Instance.PlaySE("hit", audioSource);
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

        // XとYが指定した値の間、かつカメラの前方にあるかチェック
        float min = 1 - screenRatio;
        float max = screenRatio;

        bool isInX = (min <= viewportPoint.x) && (viewportPoint.x <= max);
        bool isInY = (min <= viewportPoint.y) && (viewportPoint.y <= max);
        bool isFront = viewportPoint.z > 0;

        // すべてを満たしていれば画面内（true）
        return isInX && isInY && isFront;
    }
}
