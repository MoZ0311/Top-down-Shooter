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
        // カーソルを非表示にする
        UnityEngine.Cursor.visible = false;
        crosshair = gameUI.rootVisualElement.Q<VisualElement>(CrosshairString);
    }

    void Update()
    {
        // スクリーン上のマウスの座標を取得
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // 座標系のずれを補正して、UIの位置を移動させる
        mousePosition.y = -mousePosition.y;
        crosshair.style.translate = mousePosition;
    }
}