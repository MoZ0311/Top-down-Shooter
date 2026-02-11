using UnityEngine;
using UnityEngine.UIElements;

public class HealthGaugeManager : MonoBehaviour
{
    [SerializeField] UIDocument gameUI;
    [SerializeField] PlayerHealth playerHealth;
    const string FillString = "Fill";
    VisualElement fill;
    void Awake()
    {
        var root = gameUI.rootVisualElement;
        fill = root.Q<VisualElement>(FillString);
    }

    void OnHealthChanged(float prevValue, float newValue)
    {
        UpdateHealthGauge(newValue);
    }

    void UpdateHealthGauge(float currentHealth)
    {
        fill.style.flexGrow = currentHealth / playerHealth.MaxHealth;
    }

    void OnEnable()
    {
        playerHealth.CurrentHealth.OnValueChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        playerHealth.CurrentHealth.OnValueChanged -= OnHealthChanged;
    }
}
