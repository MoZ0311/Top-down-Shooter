using UnityEngine;

public class StageCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 centor;
    [SerializeField] float period;

    const float Circle = 360;
    
    void Update()
    {
        if (period <= 0)
        {
            return;
        }
        
        // 中心点centerの周りを、軸axisで、period周期で円運動
        transform.RotateAround(
            centor,
            Vector3.up,
            Circle / period * Time.deltaTime
        );
    }
}
