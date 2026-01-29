using UnityEngine;
using UnityEngine.UIElements;

public class UITracking : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Transform target;
    [SerializeField] Vector2 offset;
    [Header("Components")]
    [SerializeField] UIDocument gameUI;
    VisualElement healthGauge;
    const string HealthGauge = "HealthGauge";
    void Awake()
    {
        var root = gameUI.rootVisualElement;
        healthGauge = root.Q<VisualElement>(HealthGauge);
    }

    void Update()
    {
        // 追従するオブジェクトの座標をスクリーン座標に変換
        Vector2 targetPosition = Camera.main.WorldToScreenPoint(target.position);

        // 座標系のずれを補正して、UIの位置を移動させる
        targetPosition += offset;
        targetPosition.y = -targetPosition.y;
        healthGauge.style.translate = targetPosition;
    }
}
