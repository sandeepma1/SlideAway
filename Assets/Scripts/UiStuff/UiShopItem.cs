using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiShopItem : MonoBehaviour
{
    public Action<string> OnButtonClicked;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject valueGO;
    private string itemId;

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    public RectTransform GetRect()
    {
        return rect;
    }

    public void InitButton(string itemId)
    {
        this.itemId = itemId;
        button.onClick.AddListener(OnButtonClick);
        itemImage.sprite = Resources.Load<Sprite>(AppData.allShopItemsIconPath + "/" + itemId);
        UpdateItemStatus();
        UpdateItemValue();
    }

    private void UpdateItemValue()
    {
        float value = ShopItems.allShopItems[itemId].value;
        switch (ShopItems.allShopItems[itemId].currencyTypeEnum)
        {
            case CurrencyType.Gems:
                valueText.text = value + " " + AppData.gemIcon;
                break;
            case CurrencyType.Ads:
                UpdateAdItemValue();
                break;
            case CurrencyType.Paid:
                valueText.text = "$ " + value;
                break;
            default:
                break;
        }
    }

    public void UpdateAdItemValue()
    {
        if (PlayerDataManager.Instance.playerData.adsWatched.ContainsKey(itemId))
        {
            valueText.text = PlayerDataManager.Instance.playerData.adsWatched[itemId] + " " + AppData.adIcon;
        }
        else
        {
            valueText.text = ShopItems.allShopItems[itemId].value + " " + AppData.adIcon;
        }
    }

    public void UpdateItemStatus()
    {
        bool isItemUnlocked = PlayerDataManager.Instance.IsItemUnlocked(itemId);
        valueGO.SetActive(!isItemUnlocked);
        if (isItemUnlocked)
        {
            itemImage.color = Color.white;
        }
        else
        {
            itemImage.color = Color.gray;
        }
    }

    private void OnButtonClick()
    {
        OnButtonClicked?.Invoke(itemId);
    }
}