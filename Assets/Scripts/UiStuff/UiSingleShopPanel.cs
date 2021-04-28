using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiSingleShopPanel : MonoBehaviour
{
    public Action OnCloseShopPanel;
    public Action<ShopItemType, string> OnShopItemClicked;
    public ShopItemType shopItemType;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button unlockButton;
    [SerializeField] private TextMeshProUGUI unlockButtonText;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private ScrollRect mainScrollRect;
    [SerializeField] private UiShopItem uiShopItemPrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform itemSelector;
    private float panelHeight;
    private List<ShopItem> shopItems;
    private Dictionary<string, UiShopItem> uishopItems = new Dictionary<string, UiShopItem>();
    private bool isShopCreated = false;
    private ShopItem curentSelectedItem;
    private ShopItem currentSelectedUnlocedItem;

    private void Awake()
    {
        closeButton.onClick.AddListener(HideShopMenu);
        unlockButton.onClick.AddListener(OnUnlockButtonClicked);
        watchAdButton.onClick.AddListener(OnWatchAdButtonClicked);
    }

    private void Start()
    {
        StartCoroutine(GetMainPanelHeight());
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(HideShopMenu);
        unlockButton.onClick.RemoveListener(OnUnlockButtonClicked);
        watchAdButton.onClick.RemoveListener(OnWatchAdButtonClicked);
    }

    private IEnumerator GetMainPanelHeight()
    {
        yield return new WaitForEndOfFrame();
        float cellSize = content.GetComponent<GridLayoutGroup>().FitGridCell();
        itemSelector.sizeDelta = new Vector2(cellSize, cellSize);
    }

    private void OnWatchAdButtonClicked()
    {
        GameAdManager.OnWatchAdClicked?.Invoke();
    }

    private void HideShopMenu()
    {
        OnCloseShopPanel?.Invoke();
        OnItemButtonClicked(currentSelectedUnlocedItem);
    }

    public void CreateShopItems(List<ShopItem> shopItems, ShopItemType shopItemType)
    {
        this.shopItemType = shopItemType;
        if (isShopCreated)
        {
            return;
        }
        isShopCreated = true;
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
            uishopItems.Add(this.shopItems[i].id, uiShopItem);
        }
        SetItemSelector(uishopItems[PlayerDataManager.Instance.CurrentSelectedBallId].GetRect().transform, 0);
        OnItemButtonClicked(uishopItems[PlayerDataManager.Instance.CurrentSelectedBallId].shopItem);
    }

    private void OnItemButtonClicked(ShopItem shopItem)
    {
        curentSelectedItem = shopItem;
        print(shopItem.id);
        if (uishopItems.ContainsKey(curentSelectedItem.id))
        {
            if (curentSelectedItem.isUnlocked)
            {
                currentSelectedUnlocedItem = curentSelectedItem;
            }
            OnShopItemClicked?.Invoke(shopItemType, curentSelectedItem.id);
            SetItemSelector(uishopItems[curentSelectedItem.id].transform, 0);
            PlayerDataManager.Instance.CurrentSelectedBallId = curentSelectedItem.id;
            unlockButton.gameObject.SetActive(!PlayerDataManager.Instance.IsBallIdUnloced(curentSelectedItem.id));
            switch (curentSelectedItem.typeEnum)
            {
                case PurchaseType.Gems:
                    unlockButtonText.text = "Unlock " + curentSelectedItem.value + AppData.gemIcon;
                    break;
                case PurchaseType.Ads:
                    unlockButtonText.text = "Watch " + curentSelectedItem.value + " " + AppData.adIcon;
                    break;
                case PurchaseType.Paid:
                    unlockButtonText.text = "Unlock $" + curentSelectedItem.value;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnUnlockButtonClicked()
    {
        switch (curentSelectedItem.typeEnum)
        {
            case PurchaseType.Gems:
                if (PlayerDataManager.Instance.GetGems() >= curentSelectedItem.value)
                {
                    curentSelectedItem.isUnlocked = true;
                    PlayerDataManager.Instance.AddUnlockedId(curentSelectedItem.id);
                    PlayerDataManager.Instance.DecrementGems((int)curentSelectedItem.value);
                    unlockButton.gameObject.SetActive(false);
                    uishopItems[curentSelectedItem.id].InitButton(curentSelectedItem);
                    currentSelectedUnlocedItem = curentSelectedItem;
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

    private void SetItemSelector(Transform parent, int itemSelectorId)
    {
        itemSelector.SetParent(parent);
        itemSelector.DOAnchorPos(Vector2.zero, 0.15f);
        itemSelector.SetAsFirstSibling();
    }
}
