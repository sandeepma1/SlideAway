using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiShopItem : MonoBehaviour
{
    public Action<ShopItem> OnButtonClicked;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject valueGO;
    public ShopItem shopItem;

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    public RectTransform GetRect()
    {
        return rect;
    }

    public void InitButton(ShopItem shopItem, string spritePath = "")
    {
        button.onClick.AddListener(OnButtonClick);
        this.shopItem = shopItem;
        switch (shopItem.typeEnum)
        {
            case PurchaseType.Gems:
                valueText.text = this.shopItem.value + " " + AppData.gemIcon;
                break;
            case PurchaseType.Ads:
                valueText.text = this.shopItem.value + " " + AppData.adIcon;
                break;
            case PurchaseType.Paid:
                valueText.text = "$ " + this.shopItem.value;
                break;
            default:
                break;
        }
        if (!String.IsNullOrEmpty(spritePath))
        {
            itemImage.sprite = Resources.Load<Sprite>(spritePath);
        }
        valueGO.SetActive(!this.shopItem.isUnlocked);
        if (this.shopItem.isUnlocked)
        {
            itemImage.color = Color.white;
        }
        else
        {
            itemImage.color = Color.gray;
        }
    }

    public void DecreaseAdValueText()
    {
        shopItem.value--;
        valueText.text = this.shopItem.value + " " + AppData.adIcon;
        if (shopItem.value <= 0)
        {
            shopItem.isUnlocked = true;
        }
    }

    private void OnButtonClick()
    {
        OnButtonClicked?.Invoke(shopItem);
    }
}