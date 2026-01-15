using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    void OnParticleSystemStopped()
    {
        // 自身をプールに返す
        PoolManager.Instance.EffectPool.Release(this);
    }
}
