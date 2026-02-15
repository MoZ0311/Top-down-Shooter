using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] UIDocument gameUI;
    PlayerStatus playerStatus;
    Label healthLabel;
    const string HealthLabel = "HealthLabel";
    const string Heart = "💚";
    const char Slash = '/';

    void Awake()
    {
        var root = gameUI.rootVisualElement;
        //healthLabel = root.Q<Label>(HealthLabel);
    }

    public void OnHealthChanged(float newValue)
    {
        healthLabel.text = Heart + newValue.ToString("000") + Slash + playerStatus.Health.ToString("000");
    }
}
