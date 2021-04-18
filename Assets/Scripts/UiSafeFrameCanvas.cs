using UnityEngine;

public class UiSafeFrameCanvas : MonoBehaviour
{
    private void Awake()
    {
        Rect safeRect = Screen.safeArea;
        Hud.SetHudText?.Invoke(safeRect.x + " " + safeRect.y + " " + safeRect.width + " " + safeRect.height);
        GetComponent<RectTransform>().rect.Set(safeRect.x, safeRect.y, safeRect.width, safeRect.height);
    }
}