using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    void OnParticleSystemStopped()
    {
        PoolManager.Instance.EffectPool.Release(this);
    }
}
