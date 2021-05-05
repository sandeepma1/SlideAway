using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiShopItem : MonoBehaviour
{
    public Action<string> OnButtonClicked;
    public Action<string> OnPaidPurchaseComplete;
    public Action<string> OnPaidPurchaseFailed;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject valueGO;
    private string itemId;
    private float value;

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
        value = Shop.items[itemId].value;
        switch (Shop.items[itemId].currencyTypeEnum)
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
        valueText.text = Player.AdsWatchedOfItemId(itemId) + "/" + Shop.items[itemId].value + " " + AppData.adIcon;
        //if (Player.save.adsWatched.ContainsKey(itemId))
        //{
        //    valueText.text = Player.save.adsWatched[itemId] + " " + AppData.adIcon;
        //}
        //else
        //{
        //    valueText.text = Shop.items[itemId].value + " " + AppData.adIcon;
        //}
    }

    public void UpdateItemStatus()
    {
        bool isItemUnlocked = Player.IsItemUnlocked(itemId);
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