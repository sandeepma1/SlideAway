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
    private float gridLayoutWidth;
    private ShopItems shopItems;
    private List<UiShopBallItem> uiShopBallItemGem = new List<UiShopBallItem>();
    private List<UiShopBallItem> uiShopBallItemAd = new List<UiShopBallItem>();
    private List<UiShopBallItem> uiShopBallItemPaid = new List<UiShopBallItem>();

    private void Awake()
    {
        UiStartPanel.OnShopButtonPressed += OnShopButtonPressed;
        closeButton.onClick.AddListener(HideShopMenu);
        StartCoroutine(GetMainPanelHeight());
    }

    private void Start()
    {
        InitTabs();
        CreateBallItemButtons();
    }

    private void OnDestroy()
    {
        UiStartPanel.OnShopButtonPressed -= OnShopButtonPressed;
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
        UiStartPanel.OnToggleUiStartPanel?.Invoke(false);
    }

    private void HideShopMenu()
    {
        OnIsShopMenuVisible?.Invoke(false);
        mainPanel.DOAnchorPosY(-panelHeight, animDuration)
            .OnComplete(() => mainPanel.gameObject.SetActive(false));
        UiStartPanel.OnToggleUiStartPanel?.Invoke(true);
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


    #region Ball Buttons
    private void CreateBallItemButtons()
    {
        TextAsset mytxtData = (TextAsset)Resources.Load(AppData.dbPath);
        shopItems = JsonUtility.FromJson<ShopItems>(mytxtData.text);

        for (int i = 0; i < shopItems.GemBalls.Count; i++)
        {
            UiShopBallItem uiShopBallItem = Instantiate(uiShopBallItemPrefab, contents[0]);
            uiShopBallItem.InitButton(i, shopItems.GemBalls[i].value + AppData.gemIcon);
            uiShopBallItem.OnButtonClicked += OnGemBallButtonClicked;
            uiShopBallItemGem.Add(uiShopBallItem);
        }
        SetItemSelector(uiShopBallItemGem[0].rect.transform);
        for (int i = 0; i < shopItems.AdBalls.Count; i++)
        {
            UiShopBallItem uiShopBallItem = Instantiate(uiShopBallItemPrefab, contents[1]);
            uiShopBallItem.InitButton(i, shopItems.AdBalls[i].value + AppData.adIcon);
            uiShopBallItem.OnButtonClicked += OnAdBallButtonClicked;
            uiShopBallItemAd.Add(uiShopBallItem);
        }
        for (int i = 0; i < shopItems.PaidBalls.Count; i++)
        {
            UiShopBallItem uiShopBallItem = Instantiate(uiShopBallItemPrefab, contents[2]);
            uiShopBallItem.InitButton(i, "$" + shopItems.PaidBalls[i].value);
            uiShopBallItem.OnButtonClicked += OnPaidBallButtonClicked;
            uiShopBallItemPaid.Add(uiShopBallItem);
        }
    }

    private void OnGemBallButtonClicked(int id)
    {
        SetItemSelector(uiShopBallItemGem[id].transform);
        OnBallMaterialChanged?.Invoke(
            Resources.Load(AppData.gemsBallMatPath + "/" + id, typeof(Material)) as Material);
    }

    private void OnAdBallButtonClicked(int id)
    {
        SetItemSelector(uiShopBallItemAd[id].transform);
        OnBallMaterialChanged?.Invoke(
           Resources.Load(AppData.AdsBallMatPath + "/" + id, typeof(Material)) as Material);
    }

    private void OnPaidBallButtonClicked(int id)
    {
        SetItemSelector(uiShopBallItemPaid[id].transform);
        OnBallMaterialChanged?.Invoke(
           Resources.Load(AppData.paidBallMatPath + "/" + id, typeof(Material)) as Material);
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
public class ShoppItem
{
    public int id;
    public float value;
}

[System.Serializable]
public class ShopItems
{
    public List<ShoppItem> GemBalls;
    public List<ShoppItem> AdBalls;
    public List<ShoppItem> PaidBalls;
}

[System.Serializable]
public class UnlockedItem
{
    public int id;
    public bool isUnlocked;
    public int incrementValue;
}
[System.Serializable]
public class UnlockedItems
{
    public List<UnlockedItem> GemBalls;
    public List<UnlockedItem> AdBalls;
    public List<UnlockedItem> PaidBalls;
}