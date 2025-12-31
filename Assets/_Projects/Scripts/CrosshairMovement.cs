using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class CrosshairMovement : MonoBehaviour
{
    [SerializeField] UIDocument gameUI;
    VisualElement crosshair;
    const string CrosshairString = "Crosshair";

    void Awake()
    {
        UnityEngine.Cursor.visible = false;
        crosshair = gameUI.rootVisualElement.Q<VisualElement>(CrosshairString);
    }

    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.y = -mousePosition.y;
        crosshair.style.translate = mousePosition;
    }
}