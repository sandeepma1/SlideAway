using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiSingleShopPanel : MonoBehaviour
{
    public Action<ShopItemType, string> OnShopItemClicked;
    public ShopItemType shopItemType;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private ScrollRect mainScrollRect;
    [SerializeField] private UiShopItem uiShopItemPrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform itemSelector;
    [SerializeField] private TextMeshProUGUI unlockButtonText;
    private string UnlockButtonText
    {
        set
        {
            switch (curentSelectedItem.typeEnum)
            {
                case PurchaseType.Gems:
                    unlockButtonText.text = "Unlock " + value + AppData.gemIcon;
                    break;
                case PurchaseType.Ads:
                    unlockButtonText.text = "Watch " + value + " " + AppData.adIcon;
                    break;
                case PurchaseType.Paid:
                    unlockButtonText.text = "Unlock $" + value;
                    break;
                default:
                    unlockButtonText.text = value.ToString();
                    break;
            }
        }
    }
    private List<ShopItem> shopItems;
    private Dictionary<string, UiShopItem> uishopItems = new Dictionary<string, UiShopItem>();
    private ShopItem curentSelectedItem;
    private ShopItem currentSelectedUnlocedItem;
    private bool isThisPanelCreated = false;

    private void Awake()
    {
        UiShopCanvas.OnIsShopMenuVisible += OnIsShopMenuVisible;
        unlockButton.onClick.AddListener(OnUnlockButtonClicked);
        watchAdButton.onClick.AddListener(OnWatchAdButtonClicked);
    }

    private void OnDestroy()
    {
        UiShopCanvas.OnIsShopMenuVisible -= OnIsShopMenuVisible;
        unlockButton.onClick.RemoveListener(OnUnlockButtonClicked);
        watchAdButton.onClick.RemoveListener(OnWatchAdButtonClicked);
    }

    private void OnIsShopMenuVisible(bool isShopMenuVisible) //If shop is closed
    {
        if (!isShopMenuVisible)
        {
            OnItemButtonClicked(currentSelectedUnlocedItem);
        }
    }

    private void OnWatchAdButtonClicked()
    {
        GameAdManager.OnWatchAdClicked?.Invoke();
    }

    public void CreateShopItems(List<ShopItem> shopItems, ShopItemType shopItemType)
    {
        if (isThisPanelCreated)
        {
            return;
        }
        isThisPanelCreated = true;
        this.shopItemType = shopItemType;
        itemSelector.sizeDelta = content.GetComponent<GridLayoutGroup>().FitGridCell2d();
        this.shopItems = shopItems;
        for (int i = 0; i < this.shopItems.Count; i++)
        {
            UiShopItem uiShopItem = Instantiate(uiShopItemPrefab, content);
            switch (this.shopItemType)
            {
                case ShopItemType.Ball:
                    uiShopItem.InitButton(this.shopItems[i], AppData.allBallIconsPath + "/" + this.shopItems[i].id);
                    break;
                case ShopItemType.Floor:
                    uiShopItem.InitButton(this.shopItems[i], AppData.allFloorIconsPath + "/" + this.shopItems[i].id);
                    break;
                case ShopItemType.Background:
                    uiShopItem.InitButton(this.shopItems[i], AppData.allBackgroundIconsPath + "/" + this.shopItems[i].id);
                    break;
                default:
                    break;
            }
            uiShopItem.OnButtonClicked += OnItemButtonClicked;
            if (!uishopItems.ContainsKey(this.shopItems[i].id))
            {
                uishopItems.Add(this.shopItems[i].id, uiShopItem);
            }
            else
            {
                print("Id already exists" + this.shopItems[i].id);
            }
        }
        SetItemSelector(null);
        gameObject.SetActive(false);
    }

    private void OnItemButtonClicked(ShopItem shopItem)
    {
        if (!uishopItems.ContainsKey(shopItem.id))
        {
            Hud.SetHudText?.Invoke("Something gone wrong, check UiSingleShopPanel");
            return;
        }
        curentSelectedItem = shopItem;
        if (curentSelectedItem.isUnlocked)
        {
            currentSelectedUnlocedItem = curentSelectedItem;
            SaveCurrentSelectedId(currentSelectedUnlocedItem.id);
        }
        OnShopItemClicked?.Invoke(shopItemType, curentSelectedItem.id);
        SetItemSelector(uishopItems[curentSelectedItem.id].transform);
        SetUnlockButtonText(curentSelectedItem.value.ToString());
    }

    private void SetUnlockButtonText(string text = "")
    {
        switch (shopItemType)
        {
            case ShopItemType.Ball:
                bool isUnlocked = PlayerDataManager.Instance.IsBallIdUnlocked(curentSelectedItem.id);
                unlockButton.gameObject.SetActive(!isUnlocked);
                if (isUnlocked)
                {
                    return;
                }
                break;
            case ShopItemType.Floor:
                bool isFloorUnlocked = PlayerDataManager.Instance.IsFloorIdUnlocked(curentSelectedItem.id);
                unlockButton.gameObject.SetActive(!isFloorUnlocked);
                if (isFloorUnlocked)
                {
                    return;
                }
                break;
            case ShopItemType.Background:
                bool isBackgroundUnlocked = PlayerDataManager.Instance.IsBackgroundIdUnlocked(curentSelectedItem.id);
                unlockButton.gameObject.SetActive(!isBackgroundUnlocked);
                if (isBackgroundUnlocked)
                {
                    return;
                }
                break;
            default:
                break;
        }

        switch (curentSelectedItem.typeEnum)
        {
            case PurchaseType.Gems:
                unlockButtonText.text = "Unlock " + text + AppData.gemIcon;
                break;
            case PurchaseType.Ads:
                unlockButtonText.text = "Watch " + text + " " + AppData.adIcon;
                break;
            case PurchaseType.Paid:
                unlockButtonText.text = "Unlock $" + text;
                break;
            default:
                unlockButtonText.text = text.ToString();
                break;
        }
    }

    private void OnUnlockButtonClicked()
    {
        switch (curentSelectedItem.typeEnum)
        {
            case PurchaseType.Gems:
                if (PlayerDataManager.Instance.GetGems() >= curentSelectedItem.value)
                {
                    currentSelectedUnlocedItem = curentSelectedItem;
                    SaveCurrentSelectedId(curentSelectedItem.id);
                    curentSelectedItem.isUnlocked = true;
                    PlayerDataManager.Instance.DecrementGems((int)curentSelectedItem.value);
                    SaveAddUnlockedItemId(curentSelectedItem.id);
                    unlockButton.gameObject.SetActive(false);
                    uishopItems[curentSelectedItem.id].InitButton(curentSelectedItem);
                }
                else
                {
                    Debug.LogError("Gems are less, by more");
                }
                break;
            case PurchaseType.Ads:
                break;
            case PurchaseType.Paid:
                break;
            default:
                break;
        }
    }

    private void SaveAddUnlockedItemId(string id)
    {
        switch (shopItemType)
        {
            case ShopItemType.Ball:
                PlayerDataManager.Instance.AddBallUnlockedId(id);
                break;
            case ShopItemType.Floor:
                PlayerDataManager.Instance.AddFloorUnlockedId(id);
                break;
            case ShopItemType.Background:
                PlayerDataManager.Instance.AddBackgroundUnlockedId(id);
                break;
            default:
                break;
        }
    }

    private void SaveCurrentSelectedId(string id)
    {
        switch (shopItemType)
        {
            case ShopItemType.Ball:
                PlayerDataManager.Instance.CurrentSelectedBallId = id;
                //unlockButton.gameObject.SetActive(!PlayerDataManager.Instance.IsBallIdUnloced(id));
                break;
            case ShopItemType.Floor:
                PlayerDataManager.Instance.CurrentSelectedFloorId = id;
                //unlockButton.gameObject.SetActive(!PlayerDataManager.Instance.IsFloorIdUnloced(id));
                break;
            case ShopItemType.Background:
                PlayerDataManager.Instance.CurrentSelectedBackgroundId = id;
                //unlockButton.gameObject.SetActive(!PlayerDataManager.Instance.IsBackgroundIdUnloced(id));
                break;
            default:
                break;
        }
    }

    private void SetItemSelector(Transform transform = null)
    {
        switch (shopItemType)
        {
            case ShopItemType.Ball:
                if (transform == null)
                {
                    itemSelector.SetParent(uishopItems[PlayerDataManager.Instance.CurrentSelectedBallId].transform);
                    OnItemButtonClicked(uishopItems[PlayerDataManager.Instance.CurrentSelectedBallId].shopItem);
                }
                else
                    itemSelector.SetParent(transform);
                break;
            case ShopItemType.Floor:
                if (transform == null)
                {
                    itemSelector.SetParent(uishopItems[PlayerDataManager.Instance.CurrentSelectedFloorId].transform);
                    OnItemButtonClicked(uishopItems[PlayerDataManager.Instance.CurrentSelectedFloorId].shopItem);
                }
                else
                    itemSelector.SetParent(transform);
                break;
            case ShopItemType.Background:
                if (transform == null)
                {
                    itemSelector.SetParent(uishopItems[PlayerDataManager.Instance.CurrentSelectedBackgroundId].transform);
                    OnItemButtonClicked(uishopItems[PlayerDataManager.Instance.CurrentSelectedBackgroundId].shopItem);
                }
                else
                    itemSelector.SetParent(transform);
                break;
            default:
                break;
        }
        itemSelector.DOAnchorPos(Vector2.zero, 0.15f);
        itemSelector.SetAsFirstSibling();
    }
}