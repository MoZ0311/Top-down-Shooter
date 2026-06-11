using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] UIDocument overviewUI;
    [SerializeField] UIDocument gameUI;

    void Awake()
    {
        overviewUI.rootVisualElement.style.display = DisplayStyle.Flex;
        gameUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void SwitchUI()
    {
        overviewUI.rootVisualElement.style.display = DisplayStyle.None;
        gameUI.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
