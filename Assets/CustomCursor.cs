using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    [SerializeField]
    Image cursorImage;
    [SerializeField]
    float cursorScale = 1.0f;
    const int gridSize = 16;

    void Start()
    {
        Cursor.visible = false;
        if (cursorImage == null) return;
        
        cursorImage.rectTransform.localScale = Vector3.one * cursorScale;
    }

    void Update()
    {
        if (cursorImage == null) return;

        RectTransform canvasRect = cursorImage.canvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            cursorImage.canvas.worldCamera,
            out Vector2 localPoint
        );

        localPoint.x = Mathf.Round(localPoint.x / gridSize) * gridSize;
        localPoint.y = Mathf.Round(localPoint.y / gridSize) * gridSize;
        cursorImage.rectTransform.localPosition = localPoint;
    }
}
