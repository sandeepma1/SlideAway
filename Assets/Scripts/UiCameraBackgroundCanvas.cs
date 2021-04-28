using UnityEngine;
using UnityEngine.UI;

public class UiCameraBackgroundCanvas : MonoBehaviour
{
    [SerializeField] private Image mainBgImage;

    private void Awake()
    {
        UiShopCanvas.OnBackgroundChanged += OnBackgroundChanged;
    }

    private void OnDestroy()
    {
        UiShopCanvas.OnBackgroundChanged -= OnBackgroundChanged;
    }

    private void OnBackgroundChanged(string backgroundId)
    {
        mainBgImage.sprite = Resources.Load<Sprite>(AppData.allBackgroundIconsPath + "/" + backgroundId);
    }
}
