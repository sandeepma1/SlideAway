using UnityEngine;
using System.Collections;

public class UiSafeFrameCanvas : MonoBehaviour
{
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(GetSetSafeFrame());
    }

    private IEnumerator GetSetSafeFrame()
    {
        yield return new WaitForEndOfFrame();
        Rect safeRect = Screen.safeArea;
        rectTransform.rect.Set(safeRect.x, safeRect.y, safeRect.width, safeRect.height);
    }
}