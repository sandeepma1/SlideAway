using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

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

    #region Unity functions
    private void Awake()
    {
        GameAdManager.OnAdWatchRewardPlayer += OnAdWatchRewardPlayer;
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
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
        GameAdManager.OnAdWatchRewardPlayer -= OnAdWatchRewardPlayer;
        PlayerDataManager.OnPlayerDataLoaded -= OnPlayerDataLoaded;
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
        foreach (KeyValuePair<string, ShopItem> item in ShopItems.allShopItems)
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
            SetItemSelector(i, PlayerDataManager.Instance.playerData.currentSelectedItemIds[i]);
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
        if (PlayerDataManager.Instance.isPlayerDataLoaded)
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
            string itemId = PlayerDataManager.Instance.playerData.currentSelectedItemIds[i];
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
        if (lastClickedTabId == tabId)
        {
            return;
        }
        SetItemSelector(tabId, PlayerDataManager.Instance.playerData.currentSelectedItemIds[tabId]);
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
        if (PlayerDataManager.Instance.playerData.unlockedItemIds.Contains(currentItemId))
        {
            currentUnlockedItemId = currentItemId;
            PlayerDataManager.Instance.playerData.currentSelectedItemIds[lastClickedTabId] = currentUnlockedItemId;
        }
        SetItemSelector(lastClickedTabId, currentItemId);
    }

    private void UnlockButtonPressed()
    {
        ShopItem shopItem = ShopItems.allShopItems[currentItemId];
        switch (shopItem.currencyTypeEnum)
        {
            case CurrencyType.Gems:
                if (PlayerDataManager.Instance.GetGems() >= (int)shopItem.value)
                {
                    currentUnlockedItemId = currentItemId;
                    PlayerDataManager.Instance.DecrementGems((int)shopItem.value);
                    PlayerDataManager.Instance.playerData.currentSelectedItemIds[lastClickedTabId] = currentItemId;
                    if (PlayerDataManager.Instance.playerData.unlockedItemIds.Contains(currentItemId))
                    {
                        Debug.LogError("Something is wrong here");
                    }
                    else
                    {
                        PlayerDataManager.Instance.playerData.unlockedItemIds.Add(currentItemId);
                    }
                    OnItemButtonClicked(currentItemId);
                    uiShopItems[currentItemId].UpdateItemStatus();
                }
                else
                {
                    Debug.LogError("Gems are less, by more");
                }
                break;
            case CurrencyType.Ads:
                currentRequestedAdItemId = currentItemId;
                GameAdManager.OnWatchAd?.Invoke(AdRewardType.SingleReward, currentRequestedAdItemId);
                break;
            case CurrencyType.Paid:
                break;
            default:
                break;
        }
    }

    private void OnAdWatchRewardPlayer(string itemId)
    {
        if (ShopItems.allShopItems[itemId].currencyTypeEnum != CurrencyType.Ads)
        {
            return;
        }
        if (currentRequestedAdItemId == itemId)
        {
            currentRequestedAdItemId = "";
            if (PlayerDataManager.Instance.playerData.adsWatched.ContainsKey(itemId))// Already watched one or more ads
            {
                PlayerDataManager.Instance.playerData.adsWatched[itemId]--;
                uiShopItems[itemId].UpdateAdItemValue();
                if (PlayerDataManager.Instance.playerData.adsWatched[itemId] <= 0) // Item is unlocked, watched all ads
                {
                    PlayerDataManager.Instance.AddItemUnlockedId(itemId);
                    PlayerDataManager.Instance.playerData.adsWatched.Remove(itemId);
                    uiShopItems[itemId].UpdateItemStatus();
                    OnItemButtonClicked(currentItemId);
                }
            }
            else //First time ad watched
            {
                int value = (int)ShopItems.allShopItems[itemId].value - 1;
                PlayerDataManager.Instance.playerData.adsWatched.Add(itemId, value);
                uiShopItems[itemId].UpdateAdItemValue();
            }
        }
    }

    private void WatchAdForFreeGemsButtonPressed()
    {
        GameAdManager.OnWatchAd?.Invoke(AdRewardType.FreeGems, "");
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
        bool isItemUnlocked = PlayerDataManager.Instance.playerData.unlockedItemIds.Contains(itemId);
        unlockButton.gameObject.SetActive(!isItemUnlocked);
        if (isItemUnlocked)
        {
            return;
        }
        float value = ShopItems.allShopItems[itemId].value;
        switch (ShopItems.allShopItems[itemId].currencyTypeEnum)
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
        switch (ShopItems.allShopItems[itemId].itemTypeEnum)
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