using UnityEngine;
using UnityEngine.UIElements;

public class HealthGaugeManager : MonoBehaviour
{
    [SerializeField] UIDocument gameUI;
    [SerializeField] PlayerHealth playerHealth;
    VisualElement fill;
    Label healthLabel;
    const string Fill = "Fill";

    void OnHealthChanged(float prevValue, float newValue)
    {
        UpdateHealthGauge(newValue);
    }

    void UpdateHealthGauge(float currentHealth)
    {
        fill.style.flexGrow = currentHealth / playerHealth.MaxHealth;
        healthLabel.text = currentHealth.ToString();
    }

    void OnEnable()
    {
        // UI要素を検索/取得
        var root = gameUI.rootVisualElement;
        fill = root.Q<VisualElement>(Fill);
        healthLabel = root.Q<Label>();

        playerHealth.CurrentHealth.OnValueChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        playerHealth.CurrentHealth.OnValueChanged -= OnHealthChanged;
    }
}
