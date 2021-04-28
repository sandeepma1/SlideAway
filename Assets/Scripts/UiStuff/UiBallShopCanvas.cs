using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class UiBallShopCanvas : MonoBehaviour
{
    public static Action<bool> OnIsShopMenuVisible;
    public static Action<Material> OnBallMaterialChanged;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button unlockButton;
    [SerializeField] private TextMeshProUGUI unlockButtonText;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private Color selectTabColor;
    [SerializeField] private Color deselectTabColor;
    [SerializeField] private ScrollRect mainScrollRect;
    [SerializeField] private UiShopItem uiShopBallItemPrefab;
    [SerializeField] private UiShopItem uiShopFloorItemPrefab;
    [SerializeField] private UiShopItem uiShopBackgroundItemPrefab;
    [SerializeField] private UiTabButton[] uiTabButtons;
    [SerializeField] private RectTransform[] contents;
    [SerializeField] private RectTransform[] itemSelectors;
    //[SerializeField] private RectTransform floorItemSelector;
    //[SerializeField] private RectTransform backgroundItemSelector;
    private float panelHeight;
    private int lastClickedTabId;
    private ShopItems allShopItems;
    private Dictionary<string, UiShopItem> uiBallItems = new Dictionary<string, UiShopItem>();
    private Dictionary<string, UiShopItem> uiFloorItems = new Dictionary<string, UiShopItem>();
    private Dictionary<string, UiShopItem> uiBackgroundItems = new Dictionary<string, UiShopItem>();
    private bool isPlayerDataLoaded = false;
    private bool isShopCreated = false;
    private ShopItem curentSelectedBallItem;
    private ShopItem currentSelectedUnlocedBallItem;
    private ShopItem curentSelectedFloorItem;
    private ShopItem currentSelectedUnlocedFloorItem;
    private ShopItem curentSelectedBackgroundItem;
    private ShopItem currentSelectedUnlocedBackgroundItem;

    private void Awake()
    {
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnShopButtonPressed += OnShopButtonPressed;
        closeButton.onClick.AddListener(HideShopMenu);
        unlockButton.onClick.AddListener(OnBallUnlockButtonClicked);
        watchAdButton.onClick.AddListener(OnWatchAdButtonClicked);
        StartCoroutine(GetMainPanelHeight());
    }

    private void Start()
    {
        InitTabs();
    }

    private void OnDestroy()
    {
        PlayerDataManager.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnShopButtonPressed -= OnShopButtonPressed;
        closeButton.onClick.RemoveListener(HideShopMenu);
        unlockButton.onClick.RemoveListener(OnBallUnlockButtonClicked);
        watchAdButton.onClick.RemoveListener(OnWatchAdButtonClicked);
    }

    private void OnWatchAdButtonClicked()
    {
        GameAdManager.OnWatchAdClicked?.Invoke();
    }


    #region Show/Hide main Store menu
    private IEnumerator GetMainPanelHeight()
    {
        yield return new WaitForEndOfFrame();
        panelHeight = mainPanel.rect.height + 200;
        for (int i = 0; i < contents.Length; i++)
        {
            float cellSize = contents[i].GetComponent<GridLayoutGroup>().FitGridCell();
            itemSelectors[i].sizeDelta = new Vector2(cellSize, cellSize);
            contents[i].gameObject.SetActive(false);
        }
        contents[0].gameObject.SetActive(true);
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

        OnBallButtonClicked(currentSelectedUnlocedBallItem);
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
        contents[lastClickedTabId].gameObject.SetActive(false);
        uiTabButtons[lastClickedTabId].ToggleButtonPressed(deselectTabColor, false);
        uiTabButtons[tabId].ToggleButtonPressed(selectTabColor, true);
        contents[tabId].gameObject.SetActive(true);
        mainScrollRect.content = contents[tabId];
        switch (tabId)
        {
            default:
                break;
        }
        lastClickedTabId = tabId;
    }
    #endregion

    private void OnPlayerDataLoaded()
    {
        isPlayerDataLoaded = true;
        CreateAllBallItems();
        OnBallButtonClicked(uiBallItems[PlayerDataManager.Instance.CurrentSelectedBallId].shopItem);
    }

    #region Ball Buttons
    private void CreateAllBallItems()
    {
        if (isShopCreated)
        {
            return;
        }
        isShopCreated = true;
        TextAsset mytxtData = (TextAsset)Resources.Load(AppData.shopItemsDbJsonPath);
        allShopItems = JsonUtility.FromJson<ShopItems>(mytxtData.text);
        for (int i = 0; i < allShopItems.BallItems.Count; i++)
        {
            Enum.TryParse(allShopItems.BallItems[i].type, out allShopItems.BallItems[i].typeEnum);
            allShopItems.BallItems[i].isUnlocked = PlayerDataManager.Instance.IsBallIdUnloced(allShopItems.BallItems[i].id);
        }
        for (int i = 0; i < allShopItems.FloorItems.Count; i++)
        {
            Enum.TryParse(allShopItems.FloorItems[i].type, out allShopItems.FloorItems[i].typeEnum);
            allShopItems.FloorItems[i].isUnlocked = PlayerDataManager.Instance.IsFloorIdUnloced(allShopItems.FloorItems[i].id);
        }
        for (int i = 0; i < allShopItems.BackgroundItems.Count; i++)
        {
            Enum.TryParse(allShopItems.BackgroundItems[i].type, out allShopItems.BackgroundItems[i].typeEnum);
            allShopItems.BackgroundItems[i].isUnlocked = PlayerDataManager.Instance.IsBackgroundIdUnloced(allShopItems.BackgroundItems[i].id);
        }

        for (int i = 0; i < allShopItems.BallItems.Count; i++)
        {
            UiShopItem uiShopItem = Instantiate(uiShopBallItemPrefab, contents[0]);
            uiShopItem.InitButton(allShopItems.BallItems[i], AppData.allBallIconsPath + "/" + allShopItems.BallItems[i].id);
            uiShopItem.OnButtonClicked += OnBallButtonClicked;
            uiBallItems.Add(allShopItems.BallItems[i].id, uiShopItem);
        }
        for (int i = 0; i < allShopItems.FloorItems.Count; i++)
        {
            UiShopItem uiShopItem = Instantiate(uiShopFloorItemPrefab, contents[1]);
            uiShopItem.InitButton(allShopItems.FloorItems[i], AppData.allFloorIconsPath + "/" + allShopItems.FloorItems[i].id);
            uiShopItem.OnButtonClicked += OnShopFloorButtonClicked;
            uiFloorItems.Add(allShopItems.FloorItems[i].id, uiShopItem);
        }
        for (int i = 0; i < allShopItems.BackgroundItems.Count; i++)
        {
            UiShopItem uiShopItem = Instantiate(uiShopBackgroundItemPrefab, contents[2]);
            uiShopItem.InitButton(allShopItems.FloorItems[i], AppData.allBackgroundIconsPath + "/" + allShopItems.BackgroundItems[i].id);
            //uiShopBallItem.OnButtonClicked += OnShopBackgroundButtonClicked;
            uiBackgroundItems.Add(allShopItems.BackgroundItems[i].id, uiShopItem);
        }
        SetItemSelector(uiBallItems[PlayerDataManager.Instance.CurrentSelectedBallId].GetRect().transform, 0);
        SetItemSelector(uiFloorItems[PlayerDataManager.Instance.CurrentSelectedFloorId].GetRect().transform, 0);
        SetItemSelector(uiBackgroundItems[PlayerDataManager.Instance.CurrentSelectedBackgroundId].GetRect().transform, 0);
    }

    #region Ball
    private void OnBallButtonClicked(ShopItem shopBallItem)
    {
        curentSelectedBallItem = shopBallItem;
        if (uiBallItems.ContainsKey(curentSelectedBallItem.id))
        {
            if (curentSelectedBallItem.isUnlocked)
            {
                currentSelectedUnlocedBallItem = curentSelectedBallItem;
            }

            OnBallMaterialChanged?.Invoke(
                Resources.Load(AppData.allBallMatPath + "/" + curentSelectedBallItem.id, typeof(Material)) as Material);
            SetItemSelector(uiBallItems[curentSelectedBallItem.id].transform, 0);
            PlayerDataManager.Instance.CurrentSelectedBallId = curentSelectedBallItem.id;
            unlockButton.gameObject.SetActive(!PlayerDataManager.Instance.IsBallIdUnloced(curentSelectedBallItem.id));
            switch (curentSelectedBallItem.typeEnum)
            {
                case ShopItemType.Gems:
                    unlockButtonText.text = "Unlock " + curentSelectedBallItem.value + AppData.gemIcon;
                    break;
                case ShopItemType.Ads:
                    unlockButtonText.text = "Watch " + curentSelectedBallItem.value + " " + AppData.adIcon;
                    break;
                case ShopItemType.Paid:
                    unlockButtonText.text = "Unlock $" + curentSelectedBallItem.value;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnBallUnlockButtonClicked()
    {
        switch (curentSelectedBallItem.typeEnum)
        {
            case ShopItemType.Gems:
                if (PlayerDataManager.Instance.GetGems() >= curentSelectedBallItem.value)
                {
                    curentSelectedBallItem.isUnlocked = true;
                    PlayerDataManager.Instance.AddUnlockedId(curentSelectedBallItem.id);
                    PlayerDataManager.Instance.DecrementGems((int)curentSelectedBallItem.value);
                    unlockButton.gameObject.SetActive(false);
                    uiBallItems[curentSelectedBallItem.id].InitButton(curentSelectedBallItem);
                    currentSelectedUnlocedBallItem = curentSelectedBallItem;
                }
                else
                {
                    Debug.LogError("Gems are less, by more");
                }
                break;
            case ShopItemType.Ads:
                break;
            case ShopItemType.Paid:
                break;
            default:
                break;
        }
    }

    private void SetItemSelector(Transform parent, int itemSelectorId)
    {
        itemSelectors[itemSelectorId].SetParent(parent);
        itemSelectors[itemSelectorId].DOAnchorPos(Vector2.zero, 0.15f);
        itemSelectors[itemSelectorId].SetAsFirstSibling();
    }
    #endregion

    #region Floor
    private void OnShopFloorButtonClicked(ShopItem shopItem)
    {
        curentSelectedFloorItem = shopItem;
        if (uiFloorItems.ContainsKey(curentSelectedBallItem.id))
        {
            if (curentSelectedBallItem.isUnlocked)
            {
                currentSelectedUnlocedBallItem = curentSelectedBallItem;
            }

            OnBallMaterialChanged?.Invoke(
                Resources.Load(AppData.allBallMatPath + "/" + curentSelectedBallItem.id, typeof(Material)) as Material);
            SetItemSelector(uiBallItems[curentSelectedBallItem.id].transform, 1);
            PlayerDataManager.Instance.CurrentSelectedBallId = curentSelectedBallItem.id;
            unlockButton.gameObject.SetActive(!PlayerDataManager.Instance.IsBallIdUnloced(curentSelectedBallItem.id));
            switch (curentSelectedBallItem.typeEnum)
            {
                case ShopItemType.Gems:
                    unlockButtonText.text = "Unlock " + curentSelectedBallItem.value + AppData.gemIcon;
                    break;
                case ShopItemType.Ads:
                    unlockButtonText.text = "Watch " + curentSelectedBallItem.value + " " + AppData.adIcon;
                    break;
                case ShopItemType.Paid:
                    unlockButtonText.text = "Unlock $" + curentSelectedBallItem.value;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnFloorUnlockButtonClicked()
    {
        switch (curentSelectedBallItem.typeEnum)
        {
            case ShopItemType.Gems:
                if (PlayerDataManager.Instance.GetGems() >= curentSelectedBallItem.value)
                {
                    curentSelectedBallItem.isUnlocked = true;
                    PlayerDataManager.Instance.AddUnlockedId(curentSelectedBallItem.id);
                    PlayerDataManager.Instance.DecrementGems((int)curentSelectedBallItem.value);
                    unlockButton.gameObject.SetActive(false);
                    uiBallItems[curentSelectedBallItem.id].InitButton(curentSelectedBallItem);
                    currentSelectedUnlocedBallItem = curentSelectedBallItem;
                }
                else
                {
                    Debug.LogError("Gems are less, by more");
                }
                break;
            case ShopItemType.Ads:
                break;
            case ShopItemType.Paid:
                break;
            default:
                break;
        }
    }

    #endregion

    #region Background
    #endregion

    #endregion
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
    public ShopItemType typeEnum;
    public bool isUnlocked;
}
public enum ShopItemType
{
    Gems,
    Ads,
    Paid
}