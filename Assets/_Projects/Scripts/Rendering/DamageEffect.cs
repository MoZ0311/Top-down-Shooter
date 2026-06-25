using UnityEngine;
using UnityEngine.Rendering;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] Volume damageVolume;
    [SerializeField] float fadeDuration;

    float currentTimer;
    bool isAnimating;

    void Update()
    {
        if (isAnimating)
        {
            currentTimer -= Time.deltaTime;
            // 時間の経過とともにWeightを1から0へフェードアウトさせる
            damageVolume.weight = Mathf.Clamp01(currentTimer / fadeDuration);

            if (currentTimer <= 0f)
            {
                isAnimating = false;
            }
        }
    }

    public void PlayDamageEffect()
    {
        // 被弾した瞬間にボリュームの影響度をMAX（1）にする
        damageVolume.weight = 1f;
        currentTimer = fadeDuration;
        isAnimating = true;
    }
}
