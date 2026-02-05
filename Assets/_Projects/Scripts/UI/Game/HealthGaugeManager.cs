using UnityEngine;
using UnityEngine.UIElements;

public class HealthGaugeManager : MonoBehaviour
{
    [SerializeField] UIDocument gameUI;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] float interpolate = 10;
    const string Fill = "Fill";
    VisualElement fill;
    void Awake()
    {
        var root = gameUI.rootVisualElement;
        fill = root.Q<VisualElement>(Fill);
    }

    void Update()
    {
        fill.style.flexGrow = Mathf.Lerp(
            fill.style.flexGrow.value,
            playerHealth.HealthRatio,
            interpolate * Time.deltaTime
        );
    }
}
