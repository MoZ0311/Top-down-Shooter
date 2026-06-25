using UnityEngine;
using UnityEngine.UIElements;

public class HealthGaugeManager : MonoBehaviour
{
    [SerializeField] PanelRenderer trackingUI;
    [SerializeField] PlayerHealth playerHealth;
    VisualElement fill;
    Label healthLabel;
    const string Fill = "Fill";

    void Awake()
    {
        trackingUI.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDestroy()
    {
        trackingUI.UnregisterUIReloadCallback(OnUIReload);
    }

    void OnEnable()
    {
        playerHealth.CurrentHealth.OnValueChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        playerHealth.CurrentHealth.OnValueChanged -= OnHealthChanged;
    }

    /// <summary>
    /// UIを再構成するコールバック
    /// </summary>
    void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
    {
        fill = root.Q<VisualElement>(Fill);
        healthLabel = root.Q<Label>();
    }

    void OnHealthChanged(float prevValue, float newValue)
    {
        UpdateHealthGauge(newValue);
    }

    void UpdateHealthGauge(float currentHealth)
    {
        if (fill == null || healthLabel == null) return;
        fill.style.flexGrow = currentHealth / playerHealth.MaxHealth;
        healthLabel.text = currentHealth.ToString();
    }
}
