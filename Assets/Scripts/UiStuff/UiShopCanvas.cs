using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiShopCanvas : MonoBehaviour
{
    public static Action<bool> OnIsShopMenuVisible;
    public static Action<string> OnBallChanged;
    public static Action<string> OnFloorChanged;
    public static Action<string> OnBackgroundChanged;
    [SerializeField] private Button closeButton;
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private Color selectTabColor;
    [SerializeField] private Color deselectTabColor;
    [SerializeField] private UiTabButton[] uiTabButtons;
    [SerializeField] private UiSingleShopPanel[] uiSingleShopPanels;
    private float panelHeight;
    private int lastClickedTabId;
    private ShopItems allShopItems;
    private bool isPlayerDataLoaded = false;

    private void Awake()
    {
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnShopButtonPressed += OnShopButtonPressed;
        closeButton.onClick.AddListener(HideShopMenu);
    }

    private void Start()
    {
        InitTabs();
        StartCoroutine(GetMainPanelHeight());
    }

    private void OnDestroy()
    {
        PlayerDataManager.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnShopButtonPressed -= OnShopButtonPressed;
        closeButton.onClick.RemoveListener(HideShopMenu);
    }


    #region Show/Hide main Store menu
    private IEnumerator GetMainPanelHeight()
    {
        yield return new WaitForEndOfFrame();
        panelHeight = mainPanel.rect.height + 500;
        if (PlayerDataManager.Instance.isPlayerDataLoaded)
        {
            OnPlayerDataLoaded();
        }
#if UNITY_EDITOR
        OnPlayerDataLoaded();
#endif
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
        OnIsShopMenuVisible?.Invoke(false);
        mainPanel.DOAnchorPosY(-panelHeight, AppData.shopAnimSpeed)
            .OnComplete(() => mainPanel.gameObject.SetActive(false));
        UiStartCanvas.OnToggleUiStartPanel?.Invoke(true);
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
        uiSingleShopPanels[lastClickedTabId].gameObject.SetActive(false);
        uiTabButtons[lastClickedTabId].ToggleButtonPressed(deselectTabColor, false);
        uiTabButtons[tabId].ToggleButtonPressed(selectTabColor, true);
        uiSingleShopPanels[tabId].gameObject.SetActive(true);
        lastClickedTabId = tabId;
    }
    #endregion

    private void OnPlayerDataLoaded()
    {
        isPlayerDataLoaded = true;
        CreateShopPanel();
    }

    private void CreateShopPanel()
    {
        TextAsset mytxtData = (TextAsset)Resources.Load(AppData.shopItemsDbJsonPath);
        allShopItems = JsonUtility.FromJson<ShopItems>(mytxtData.text);
        for (int i = 0; i < allShopItems.BallItems.Count; i++)
        {
            Enum.TryParse(allShopItems.BallItems[i].type, out allShopItems.BallItems[i].typeEnum);
            allShopItems.BallItems[i].isUnlocked = PlayerDataManager.Instance.IsBallIdUnlocked(allShopItems.BallItems[i].id);
        }
        for (int i = 0; i < allShopItems.FloorItems.Count; i++)
        {
            Enum.TryParse(allShopItems.FloorItems[i].type, out allShopItems.FloorItems[i].typeEnum);
            allShopItems.FloorItems[i].isUnlocked = PlayerDataManager.Instance.IsFloorIdUnlocked(allShopItems.FloorItems[i].id);
        }
        for (int i = 0; i < allShopItems.BackgroundItems.Count; i++)
        {
            Enum.TryParse(allShopItems.BackgroundItems[i].type, out allShopItems.BackgroundItems[i].typeEnum);
            allShopItems.BackgroundItems[i].isUnlocked = PlayerDataManager.Instance.IsBackgroundIdUnlocked(allShopItems.BackgroundItems[i].id);
        }
        for (int i = 0; i < uiSingleShopPanels.Length; i++)
        {
            uiSingleShopPanels[i].OnShopItemClicked += OnShopItemClicked;
        }
        uiSingleShopPanels[0].CreateShopItems(allShopItems.BallItems, ShopItemType.Ball);
        uiSingleShopPanels[1].CreateShopItems(allShopItems.FloorItems, ShopItemType.Floor);
        uiSingleShopPanels[2].CreateShopItems(allShopItems.BackgroundItems, ShopItemType.Background);
        uiSingleShopPanels[0].gameObject.SetActive(true);
    }

    private void OnShopItemClicked(ShopItemType shopItemType, string id)
    {
        switch (shopItemType)
        {
            case ShopItemType.Ball:
                OnBallChanged?.Invoke(id);
                break;
            case ShopItemType.Floor:
                OnFloorChanged?.Invoke(id);
                break;
            case ShopItemType.Background:
                OnBackgroundChanged?.Invoke(id);
                break;
            default:
                break;
        }
    }
}


[System.Serializable]
public class ShopItems
{
    public List<ShopItem> BallItems;
    public List<ShopItem> FloorItems;
    public List<ShopItem> BackgroundItems;
}

[System.Serializable]
public class ShopItem
{
    public string id;
    public string type;
    public float value;
    public PurchaseType typeEnum;
    public bool isUnlocked;
}
public enum PurchaseType
{
    Gems,
    Ads,
    Paid
}

public enum ShopItemType
{
    Ball,
    Floor,
    Background
}