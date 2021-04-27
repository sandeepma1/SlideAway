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
    [SerializeField] private UiBallShopItem uiShopBallItemPrefab;
    [SerializeField] private UiTabButton[] uiTabButtons;
    [SerializeField] private RectTransform[] contents;
    [SerializeField] private RectTransform itemSelector;
    private float panelHeight;
    private int lastClickedTabId;
    private ShopItems shopItems;
    private Dictionary<string, UiBallShopItem> uiShopBallItems = new Dictionary<string, UiBallShopItem>();
    private bool isPlayerDataLoaded = false;
    private bool isShopCreated = false;
    private ShopBallItem curentSelectedBallItem;
    private ShopBallItem currentSelectedUnlocedBallItem;

    private void Awake()
    {
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnShopButtonPressed += OnShopButtonPressed;
        closeButton.onClick.AddListener(HideShopMenu);
        unlockButton.onClick.AddListener(OnUnlockButtonClicked);
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
        unlockButton.onClick.RemoveListener(OnUnlockButtonClicked);
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
            itemSelector.sizeDelta = new Vector2(cellSize, cellSize);
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

        OnShopBallButtonClicked(currentSelectedUnlocedBallItem);
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
        OnShopBallButtonClicked(uiShopBallItems[PlayerDataManager.Instance.CurrentSelectedBallId].shopBallItem);
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
        shopItems = JsonUtility.FromJson<ShopItems>(mytxtData.text);
        for (int i = 0; i < shopItems.ShopBallItems.Count; i++)
        {
            Enum.TryParse(shopItems.ShopBallItems[i].type, out shopItems.ShopBallItems[i].eballTypes);
            shopItems.ShopBallItems[i].isUnlocked = PlayerDataManager.Instance.IsIdUnloced(shopItems.ShopBallItems[i].id);
        }

        for (int i = 0; i < shopItems.ShopBallItems.Count; i++)
        {
            UiBallShopItem uiShopBallItem = Instantiate(uiShopBallItemPrefab, contents[0]);
            uiShopBallItem.InitButton(shopItems.ShopBallItems[i]);
            uiShopBallItem.OnButtonClicked += OnShopBallButtonClicked;
            uiShopBallItems.Add(shopItems.ShopBallItems[i].id, uiShopBallItem);
        }
        SetItemSelector(uiShopBallItems[PlayerDataManager.Instance.CurrentSelectedBallId].GetRect().transform);
    }

    private void OnShopBallButtonClicked(ShopBallItem shopBallItem)
    {
        curentSelectedBallItem = shopBallItem;
        if (uiShopBallItems.ContainsKey(curentSelectedBallItem.id))
        {
            if (curentSelectedBallItem.isUnlocked)
            {
                currentSelectedUnlocedBallItem = curentSelectedBallItem;
            }

            OnBallMaterialChanged?.Invoke(
                Resources.Load(AppData.allBallMatPath + "/" + curentSelectedBallItem.id, typeof(Material)) as Material);
            SetItemSelector(uiShopBallItems[curentSelectedBallItem.id].transform);
            PlayerDataManager.Instance.CurrentSelectedBallId = curentSelectedBallItem.id;
            unlockButton.gameObject.SetActive(!PlayerDataManager.Instance.IsIdUnloced(curentSelectedBallItem.id));
            switch (curentSelectedBallItem.eballTypes)
            {
                case BallType.Gems:
                    unlockButtonText.text = "Unlock " + curentSelectedBallItem.value + AppData.gemIcon;
                    break;
                case BallType.Ads:
                    unlockButtonText.text = "Watch " + curentSelectedBallItem.value + " " + AppData.adIcon;
                    break;
                case BallType.Paid:
                    unlockButtonText.text = "Unlock $" + curentSelectedBallItem.value;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnUnlockButtonClicked()
    {
        switch (curentSelectedBallItem.eballTypes)
        {
            case BallType.Gems:
                if (PlayerDataManager.Instance.GetGems() >= curentSelectedBallItem.value)
                {
                    curentSelectedBallItem.isUnlocked = true;
                    PlayerDataManager.Instance.AddUnlockedId(curentSelectedBallItem.id);
                    PlayerDataManager.Instance.DecrementGems((int)curentSelectedBallItem.value);
                    unlockButton.gameObject.SetActive(false);
                    uiShopBallItems[curentSelectedBallItem.id].InitButton(curentSelectedBallItem);
                    currentSelectedUnlocedBallItem = curentSelectedBallItem;
                }
                else
                {
                    Debug.LogError("Gems are less, by more");
                }
                break;
            case BallType.Ads:
                break;
            case BallType.Paid:
                break;
            default:
                break;
        }
    }

    private void SetItemSelector(Transform parent)
    {
        itemSelector.SetParent(parent);
        itemSelector.DOAnchorPos(Vector2.zero, 0.15f);
        itemSelector.SetAsFirstSibling();
    }
    #endregion
}


[System.Serializable]
public class ShopItems
{
    public List<ShopBallItem> ShopBallItems;
}

[System.Serializable]
public class ShopBallItem
{
    public string id;
    public string type;
    public float value;
    public BallType eballTypes;
    public bool isUnlocked;
    public int incrementValue;
}
public enum BallType
{
    Gems,
    Ads,
    Paid
}