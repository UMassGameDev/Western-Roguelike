using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private Image cursorImage;
    [SerializeField] private float cursorScale = 1.0f;
    private const int gridSize = 16;

    void Start()
    {
        Cursor.visible = false;
        if (cursorImage == null) return;

        cursorImage.rectTransform.localScale = Vector3.one * cursorScale;
    }

    void Update()
    {
        if (cursorImage == null) return;

        Vector2 pos = Input.mousePosition;

        pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
        pos.y = Mathf.Round(pos.y / gridSize) * gridSize;

        cursorImage.rectTransform.position = pos;
    }
}
