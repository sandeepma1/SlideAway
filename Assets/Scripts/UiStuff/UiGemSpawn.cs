using UnityEngine;
using DG.Tweening;

public class UiGemSpawn : MonoBehaviour
{
    public void InitItem(Transform worldObject, Canvas mainCanvas, Vector2 endPos, float animDuration)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = worldObject.WorldSpaceToUiSpace(mainCanvas);
        rectTransform.DOAnchorPos(endPos, animDuration).OnComplete(() => Destroy(gameObject));
    }

    public void InitItem2d(Vector2 position, Vector2 endPos, float animDuration)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.DOAnchorPos(endPos, animDuration).OnComplete(() => Destroy(gameObject));
    }
}