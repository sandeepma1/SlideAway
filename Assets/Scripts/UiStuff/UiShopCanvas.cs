using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Purchasing;

public class UiShopCanvas : MonoBehaviour
{
    public static Action<bool> OnIsShopMenuVisible;
    public static Action<string> OnBallChanged;
    public static Action<string> OnFloorChanged;
    public static Action<string> OnBackgroundChanged;
    [SerializeField] private Button watchAdForFreeGemsButton;
    [SerializeField] private Button unlockButton;
    [SerializeField] private TextMeshProUGUI unlockButtonText;
    [SerializeField] private Button closeButton;
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private Color selectTabColor;
    [SerializeField] private Color deselectTabColor;
    [SerializeField] private UiShopItem uiShopItemPrefab;
    [SerializeField] private ScrollRect mainScrollRect;
    [SerializeField] private UiTabButton[] uiTabButtons;
    [SerializeField] private RectTransform[] contents;
    [SerializeField] private RectTransform[] itemSelectors;
    private Dictionary<string, UiShopItem> uiShopItems = new Dictionary<string, UiShopItem>();
    private float panelHeight;
    private int lastClickedTabId;
    private bool isPlayerDataLoaded = false;
    private bool isShopItemsCreated = false;
    private string currentItemId;
    private string currentUnlockedItemId;
    private string currentRequestedAdItemId;
    private string currentRequestedPaidItemId;
    #region Unity functions
    private void Awake()
    {
        IapManager.OnPurchaseComplete += OnPurchaseComplete;
        IapManager.OnPurchaseFailed += OnPurchaseFailed;
        GameAdManager.OnAdWatchRewardPlayer += OnAdWatchRewardPlayer;
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnShopButtonPressed += OnShopButtonPressed;
        closeButton.onClick.AddListener(HideShopMenu);
        unlockButton.onClick.AddListener(UnlockButtonPressed);
        watchAdForFreeGemsButton.onClick.AddListener(WatchAdForFreeGemsButtonPressed);
    }

    private void Start()
    {
        mainPanel.gameObject.SetActive(false);
        StartCoroutine(GetMainPanelHeight());
    }

    private void OnDestroy()
    {
        IapManager.OnPurchaseComplete -= OnPurchaseComplete;
        IapManager.OnPurchaseFailed -= OnPurchaseFailed;
        GameAdManager.OnAdWatchRewardPlayer -= OnAdWatchRewardPlayer;
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnShopButtonPressed -= OnShopButtonPressed;
        closeButton.onClick.RemoveListener(HideShopMenu);
        unlockButton.onClick.RemoveListener(UnlockButtonPressed);
        watchAdForFreeGemsButton.onClick.RemoveListener(WatchAdForFreeGemsButtonPressed);
    }
    #endregion


    #region Load and Create Shop Items
    private void OnPlayerDataLoaded()
    {
        isPlayerDataLoaded = true;
        CreateShopItems();
        InitTabs();
    }

    private void CreateShopItems()
    {
        if (isShopItemsCreated)
        {
            return;
        }
        isShopItemsCreated = true;
        foreach (KeyValuePair<string, ShopItem> item in Shop.items)
        {
            UiShopItem uiShopItem = Instantiate(uiShopItemPrefab, this.transform);
            uiShopItem.InitButton(item.Value.id);
            uiShopItem.OnButtonClicked += OnItemButtonClicked;
            switch (item.Value.itemTypeEnum)
            {
                case ShopItemType.Ball:
                    uiShopItem.transform.SetParent(contents[0]);
                    break;
                case ShopItemType.Floor:
                    uiShopItem.transform.SetParent(contents[1]);
                    break;
                case ShopItemType.Background:
                    uiShopItem.transform.SetParent(contents[2]);
                    break;
                default:
                    break;
            }
            uiShopItems.Add(item.Value.id, uiShopItem);
        }
        for (int i = 0; i < itemSelectors.Length; i++)
        {
            itemSelectors[i].sizeDelta = contents[i].GetComponent<GridLayoutGroup>().FitGridCell2d();
            SetItemSelector(i, Player.save.currentSelectedItemIds[i]);
            contents[i].gameObject.SetActive(false);
        }
        contents[0].gameObject.SetActive(true);
        LoadDefaultSelectedItems();
    }
    #endregion


    #region Show/Hide main Store menu
    private IEnumerator GetMainPanelHeight()
    {
        yield return new WaitForEndOfFrame();
        panelHeight = mainPanel.rect.height + 500;
        if (Player.isPlayerDataLoaded)
        {
            OnPlayerDataLoaded();
        }
    }

    private void OnShopButtonPressed()
    {
        if (!isPlayerDataLoaded)
        {
            return;
        }
        ShowShopMenu();
    }

    private void ShowShopMenu()
    {
        OnIsShopMenuVisible?.Invoke(true);
        mainPanel.gameObject.SetActive(true);
        mainPanel.DOAnchorPosY(0, AppData.shopAnimSpeed);
        UiStartCanvas.OnToggleUiStartPanel?.Invoke(false);
        AnalyticsManager.ScreenVisit(GameScreens.ShopMenu);
        AnalyticsManager.StoreOpened(UnityEngine.Analytics.StoreType.Soft);
    }

    private void HideShopMenu()
    {
        LoadDefaultSelectedItems();
        OnIsShopMenuVisible?.Invoke(false);
        mainPanel.DOAnchorPosY(-panelHeight, AppData.shopAnimSpeed)
            .OnComplete(() => mainPanel.gameObject.SetActive(false));
        UiStartCanvas.OnToggleUiStartPanel?.Invoke(true);
    }

    private void LoadDefaultSelectedItems()
    {
        for (int i = 0; i < itemSelectors.Length; i++)
        {
            string itemId = Player.save.currentSelectedItemIds[i];
            SetItemSelector(i, itemId);
        }
    }
    #endregion


    #region Tabs stuff
    private void InitTabs()
    {
        for (int i = 0; i < uiTabButtons.Length; i++)
        {
            uiTabButtons[i].Init(i);
            uiTabButtons[i].OnTabButtonClicked += OnTabButtonClicked;
        }
    }

    private void OnTabButtonClicked(int tabId)
    {
        switch (tabId)
        {
            case 0:
                AnalyticsManager.ScreenVisit(GameScreens.BallShopMenu);
                break;
            case 1:
                AnalyticsManager.ScreenVisit(GameScreens.FloorShopMenu);
                break;
            case 2:
                AnalyticsManager.ScreenVisit(GameScreens.BackgroundShopMenu);
                break;
            default:
                break;
        }
        if (lastClickedTabId == tabId)
        {
            return;
        }
        SetItemSelector(tabId, Player.save.currentSelectedItemIds[tabId]);
        contents[lastClickedTabId].gameObject.SetActive(false);
        uiTabButtons[lastClickedTabId].ToggleButtonPressed(deselectTabColor, false);
        uiTabButtons[tabId].ToggleButtonPressed(selectTabColor, true);
        contents[tabId].gameObject.SetActive(true);
        mainScrollRect.content = contents[tabId];
        lastClickedTabId = tabId;
    }
    #endregion


    #region Shop Buttons
    private void OnItemButtonClicked(string itemId)
    {
        currentItemId = itemId;
        if (Player.save.unlockedItemIds.Contains(currentItemId))
        {
            currentUnlockedItemId = currentItemId;
            Player.save.currentSelectedItemIds[lastClickedTabId] = currentUnlockedItemId;
        }
        SetItemSelector(lastClickedTabId, currentItemId);
        AnalyticsManager.ShopItemClicked(itemId);
        AnalyticsManager.StoreItemClick(UnityEngine.Analytics.StoreType.Soft, itemId);
    }

    private void UnlockButtonPressed()
    {
        ShopItem shopItem = Shop.items[currentItemId];
        switch (shopItem.currencyTypeEnum)
        {
            case CurrencyType.Gems:
                if (Player.GetGems() >= (int)shopItem.value)
                {
                    currentUnlockedItemId = currentItemId;
                    Player.DecrementGems((int)shopItem.value);
                    Player.save.currentSelectedItemIds[lastClickedTabId] = currentItemId;
                    if (Player.save.unlockedItemIds.Contains(currentItemId))
                    {
                        Debug.LogError("Something is wrong here");
                    }
                    else
                    {
                        Player.save.unlockedItemIds.Add(currentItemId);
                    }
                    OnItemButtonClicked(currentItemId);
                    uiShopItems[currentItemId].UpdateItemStatus();
                    AnalyticsManager.ItemSpent(UnityEngine.Analytics.AcquisitionType.Soft,
                        "Gems Unlocked", shopItem.value, currentItemId);
                }
                else
                {
                    Debug.LogError("Gems are less, by more");
                }
                AnalyticsManager.ButtonPressed(GameButtons.GemsUnlock);
                break;
            case CurrencyType.Ads:
                currentRequestedAdItemId = currentItemId;
                GameAdManager.OnWatchAd?.Invoke(AdRewardType.SingleReward, currentRequestedAdItemId);
                AnalyticsManager.ButtonPressed(GameButtons.AdUnclock);
                break;
            case CurrencyType.Paid:
                OnPurchaseButtonClicked(currentItemId);
                AnalyticsManager.ButtonPressed(GameButtons.PaidUnlock);
                break;
            default:
                break;
        }
        AnalyticsManager.UnlockButtonClicked(currentItemId);
    }

    private void OnPurchaseButtonClicked(string itemId)
    {
        currentRequestedPaidItemId = itemId;
        string productId = "";
        float value = Shop.items[currentRequestedPaidItemId].value;
        if (value < 1)
        {
            productId = AppData.commonball;
        }
        else if (value < 3)
        {
            productId = AppData.greatball;
        }
        else if (value < 5)
        {
            productId = AppData.epicBall;
        }
        CodelessIAPStoreListener.Instance.InitiatePurchase(productId);
    }

    private void OnPurchaseComplete(Product obj)
    {
        Player.AddItemUnlockedId(currentRequestedPaidItemId);
        uiShopItems[currentRequestedPaidItemId].UpdateItemStatus();
        OnItemButtonClicked(currentRequestedPaidItemId);
        currentRequestedPaidItemId = "";
        AnalyticsManager.IAPTransaction(currentRequestedPaidItemId,
            Shop.items[currentRequestedPaidItemId].value, currentRequestedPaidItemId);
        AnalyticsManager.ItemSpent(UnityEngine.Analytics.AcquisitionType.Premium,
                      "Paid Unlocked", Shop.items[currentRequestedPaidItemId].value, currentRequestedPaidItemId);
    }

    private void OnPurchaseFailed(Product obj)
    {
        Debug.LogError("Show transaction failed dialog");
        currentRequestedPaidItemId = "";
    }

    private void OnAdWatchRewardPlayer(string itemId)
    {
        if (Shop.items[itemId].currencyTypeEnum != CurrencyType.Ads)
        {
            return;
        }
        if (currentRequestedAdItemId == itemId)
        {
            Player.AddWatchedAdsIds(itemId);
            uiShopItems[itemId].UpdateAdItemValue();
            uiShopItems[itemId].UpdateItemStatus();
            OnItemButtonClicked(currentItemId);
        }
        AnalyticsManager.AdComplete(true);
    }

    private void WatchAdForFreeGemsButtonPressed()
    {
        GameAdManager.OnWatchAd?.Invoke(AdRewardType.FreeGems, "");
        AnalyticsManager.ButtonPressed(GameButtons.FreeGemsByAd);
    }
    #endregion

    private void SetItemSelector(int selectorId, string itemId)
    {
        itemSelectors[selectorId].SetParent(uiShopItems[itemId].transform);
        itemSelectors[selectorId].DOAnchorPos(Vector2.zero, 0.15f);
        itemSelectors[selectorId].SetAsFirstSibling();
        ChangeMaterial(itemId);
        SetUnlockButtonText(itemId);
    }

    private void SetUnlockButtonText(string itemId)
    {
        bool isItemUnlocked = Player.save.unlockedItemIds.Contains(itemId);
        unlockButton.gameObject.SetActive(!isItemUnlocked);
        if (isItemUnlocked)
        {
            return;
        }
        float value = Shop.items[itemId].value;
        switch (Shop.items[itemId].currencyTypeEnum)
        {
            case CurrencyType.Gems:
                unlockButtonText.text = "Unlock " + value + AppData.gemIcon;
                break;
            case CurrencyType.Ads:
                unlockButtonText.text = "Watch Ads " + AppData.adIcon;
                break;
            case CurrencyType.Paid:
                unlockButtonText.text = "Unlock $" + value;
                break;
            default:
                unlockButtonText.text = value.ToString();
                break;
        }
    }

    private void ChangeMaterial(string itemId)
    {
        switch (Shop.items[itemId].itemTypeEnum)
        {
            case ShopItemType.Ball:
                OnBallChanged?.Invoke(itemId);
                break;
            case ShopItemType.Floor:
                OnFloorChanged?.Invoke(itemId);
                break;
            case ShopItemType.Background:
                OnBackgroundChanged?.Invoke(itemId);
                break;
            default:
                break;
        }
    }
}