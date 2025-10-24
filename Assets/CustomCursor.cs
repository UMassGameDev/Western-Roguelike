using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    [SerializeField]
    Image cursorImage;
    [SerializeField]
    float cursorScale = 1.0f;

    void Start()
    {
        Cursor.visible = false;
        if (cursorImage != null) { cursorImage.rectTransform.localScale = Vector3.one * cursorScale; }
    }

    void Update()
    {
        if (cursorImage != null) { cursorImage.transform.position = Input.mousePosition; }
    }
}
