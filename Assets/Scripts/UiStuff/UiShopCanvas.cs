using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiShopCanvas : MonoBehaviour
{
    public static Action<bool> OnIsShopMenuVisible;
    public static Action<Material> OnBallMaterialChanged;

    [SerializeField] private Button closeButton;
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private Color selectTabColor;
    [SerializeField] private Color deselectTabColor;
    [SerializeField] private ScrollRect mainScrollRect;
    [SerializeField] private UiShopBallItem uiShopBallItemPrefab;
    [SerializeField] private UiTabButton[] uiTabButtons;
    [SerializeField] private RectTransform[] contents;
    [SerializeField] private RectTransform itemSelector;
    private float panelHeight;
    private int lastClickedTabId;
    private const float animDuration = 0.25f;
    private ShopItems shopItems;
    private Dictionary<string, UiShopBallItem> uiShopBallItems = new Dictionary<string, UiShopBallItem>();

    private void Awake()
    {
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnShopButtonPressed += OnShopButtonPressed;
        closeButton.onClick.AddListener(HideShopMenu);
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
        CreateAllBallItems();
        OnPlayerDataLoaded();
    }

    private void OnShopButtonPressed()
    {
        ShowShopMenu();
    }

    private void ShowShopMenu()
    {
        OnIsShopMenuVisible?.Invoke(true);
        mainPanel.gameObject.SetActive(true);
        mainPanel.DOAnchorPosY(0, animDuration);
        UiStartCanvas.OnToggleUiStartPanel?.Invoke(false);
    }

    private void HideShopMenu()
    {
        OnIsShopMenuVisible?.Invoke(false);
        mainPanel.DOAnchorPosY(-panelHeight, animDuration)
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
        OnShopBallButtonClicked(PlayerDataManager.Instance.CurrentSelectedBallId);
    }

    #region Ball Buttons
    private void CreateAllBallItems()
    {
        TextAsset mytxtData = (TextAsset)Resources.Load(AppData.shopItemsDbJsonPath);
        shopItems = JsonUtility.FromJson<ShopItems>(mytxtData.text);
        for (int i = 0; i < shopItems.ShopBallItems.Count; i++)
        {
            Enum.TryParse(shopItems.ShopBallItems[i].type, out shopItems.ShopBallItems[i].eballTypes);
        }
        for (int i = 0; i < shopItems.ShopBallItems.Count; i++)
        {
            UiShopBallItem uiShopBallItem = Instantiate(uiShopBallItemPrefab, contents[0]);
            switch (shopItems.ShopBallItems[i].eballTypes)
            {
                case BallTypes.Gems:
                    uiShopBallItem.transform.SetParent(contents[0]);
                    uiShopBallItem.InitButton(shopItems.ShopBallItems[i].id, shopItems.ShopBallItems[i].value + AppData.gemIcon);
                    break;
                case BallTypes.Ads:
                    uiShopBallItem.transform.SetParent(contents[1]);
                    uiShopBallItem.InitButton(shopItems.ShopBallItems[i].id, shopItems.ShopBallItems[i].value + " " + AppData.adIcon);
                    break;
                case BallTypes.Paid:
                    uiShopBallItem.transform.SetParent(contents[2]);
                    uiShopBallItem.InitButton(shopItems.ShopBallItems[i].id, "$ " + shopItems.ShopBallItems[i].value);
                    break;
                default:
                    break;
            }
            uiShopBallItem.OnButtonClicked += OnShopBallButtonClicked;
            uiShopBallItems.Add(shopItems.ShopBallItems[i].id, uiShopBallItem);
        }
        SetItemSelector(uiShopBallItems[PlayerDataManager.Instance.CurrentSelectedBallId].GetRect().transform);
    }

    private void OnShopBallButtonClicked(string id)
    {
        if (uiShopBallItems.ContainsKey(id))
        {
            SetItemSelector(uiShopBallItems[id].transform);
            OnBallMaterialChanged?.Invoke(
                Resources.Load(AppData.allBallMatPath + "/" + id, typeof(Material)) as Material);
            PlayerDataManager.Instance.CurrentSelectedBallId = id;
        }
    }

    private void SetItemSelector(Transform parent)
    {
        itemSelector.SetParent(parent);
        itemSelector.anchoredPosition = Vector2.zero;
        itemSelector.SetAsLastSibling();
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
    public BallTypes eballTypes;
    public bool isUnlocked;
    public int incrementValue;
}
public enum BallTypes
{
    Gems,
    Ads,
    Paid
}