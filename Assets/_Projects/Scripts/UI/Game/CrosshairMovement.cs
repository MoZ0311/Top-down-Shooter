using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairMovement : MonoBehaviour
{
    [SerializeField] GameUIManager gameUIManager;
    void Awake()
    {
        // カーソルを非表示にする
        Cursor.visible = false;
    }

    void Update()
    {
        if (gameUIManager.CrossHair == null)
        {
            return;
        }
        // スクリーン上のマウスの座標を取得
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // 座標系のずれを補正して、UIの位置を移動させる
        mousePosition.y = -mousePosition.y;
        gameUIManager.CrossHair.style.translate = mousePosition;
    }
}