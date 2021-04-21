using UnityEngine;
using DG.Tweening;

public class UiGemSpawn : MonoBehaviour
{
    private RectTransform rectTransform;
    private const float animDuration = 1f;

    public void InitItem(Transform worldObject, Canvas mainCanvas, Vector2 endPos, Camera camera)
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = worldObject.WorldSpaceToUiSpace(mainCanvas);
        rectTransform.DOAnchorPos(endPos, animDuration).OnComplete(() => Destroy(gameObject));
    }
}