using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    void OnParticleSystemStopped()
    {
        BulletPoolManager.Instance.EffectPool.Release(this);
    }
}
