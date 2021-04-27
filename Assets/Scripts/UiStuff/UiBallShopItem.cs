using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiBallShopItem : MonoBehaviour
{
    public Action<ShopBallItem> OnButtonClicked;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image ballImage;
    [SerializeField] private GameObject valueGO;
    public ShopBallItem shopBallItem;

    public RectTransform GetRect()
    {
        return rect;
    }

    public void InitButton(ShopBallItem shopBallItem)
    {
        button.onClick.AddListener(OnButtonClick);
        this.shopBallItem = shopBallItem;
        switch (shopBallItem.eballTypes)
        {
            case BallType.Gems:
                valueText.text = this.shopBallItem.value + AppData.gemIcon;
                break;
            case BallType.Ads:
                valueText.text = this.shopBallItem.value + AppData.adIcon;
                break;
            case BallType.Paid:
                valueText.text = "$" + this.shopBallItem.value;
                break;
            default:
                break;
        }
        ballImage.sprite = Resources.Load<Sprite>(AppData.allBallIconsPath + "/" + this.shopBallItem.id);
        valueGO.SetActive(!this.shopBallItem.isUnlocked);
        if (this.shopBallItem.isUnlocked)
        {
            ballImage.color = Color.white;
        }
        else
        {
            ballImage.color = Color.gray;
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        OnButtonClicked?.Invoke(shopBallItem);
    }
}