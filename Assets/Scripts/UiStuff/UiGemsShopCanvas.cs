using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

//Gems icons from here
//https://sketchfab.com/3d-models/classic-gems-pack-4e5a753646444773babebd6ce186ec1b
public class UiGemsShopCanvas : MonoBehaviour, IStoreListener
{
    public static Action OnShowBuyGemsMenu;
    [SerializeField] private Button closeButton;
    [SerializeField] private RectTransform mainPanelRect;
    [SerializeField] private RectTransform menuPanelRect;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private UiIapSimpleButton[] uiIapSimpleButtons;
    [SerializeField] private Button watchAdForFreeGemsButton;
    private CanvasGroup mainPanelCanvasGroup;
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    private float rectHeight;

    private void Awake()
    {
        mainPanelCanvasGroup = mainPanelRect.GetComponent<CanvasGroup>();
        OnShowBuyGemsMenu += ShowBuyGemsMenu;
        watchAdForFreeGemsButton.onClick.AddListener(WatchAdForFreeGemsButton);
    }

    private void Start()
    {
        mainPanelRect.gameObject.SetActive(false);
        mainPanelCanvasGroup.alpha = 0;
        closeButton.onClick.AddListener(HideBuyGemsMenu);
        StartCoroutine(GetRectHeight());
        for (int i = 0; i < uiIapSimpleButtons.Length; i++)
        {
            uiIapSimpleButtons[i].id = i;
            uiIapSimpleButtons[i].OnIapButtonPressed += OnIapButtonPressed;
        }
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(HideBuyGemsMenu);
        OnShowBuyGemsMenu -= ShowBuyGemsMenu;
        watchAdForFreeGemsButton.onClick.RemoveListener(WatchAdForFreeGemsButton);
    }

    private void WatchAdForFreeGemsButton()
    {
        GameAdManager.OnWatchAd?.Invoke(AdRewardType.FreeGems, "");
        AnalyticsManager.ButtonPressed(GameButtons.FreeGemsByAd);
    }

    private IEnumerator GetRectHeight()
    {
        yield return new WaitForEndOfFrame();
        rectHeight = menuPanelRect.rect.height + (menuPanelRect.rect.height / 2);
        HideBuyGemsMenu();
        mainPanelRect.gameObject.SetActive(false);
    }

    private void ShowBuyGemsMenu()
    {
        mainPanelRect.gameObject.SetActive(true);
        mainPanelCanvasGroup.DOFade(1, AppData.shopAnimSpeed);
        menuPanelRect.DOAnchorPosY(0, AppData.shopAnimSpeed);
        AnalyticsManager.ScreenVisit(GameScreens.GemsBuyMenu);
    }

    private void HideBuyGemsMenu()
    {
        menuPanelRect.DOAnchorPosY(-rectHeight, AppData.shopAnimSpeed)
            .OnComplete(() => mainPanelRect.gameObject.SetActive(false));
        mainPanelCanvasGroup.DOFade(0, AppData.shopAnimSpeed);
    }

    #region All IAP functions
    private void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(AppData.gemsTier1ProductId, ProductType.Consumable);
        builder.AddProduct(AppData.gemsTier2ProductId, ProductType.Consumable);
        builder.AddProduct(AppData.gemsTier3ProductId, ProductType.Consumable);
        builder.AddProduct(AppData.gemsTier4ProductId, ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    private void OnIapButtonPressed(int tierId)
    {
        switch (tierId)
        {
            case 0:
                BuyProductID(AppData.gemsTier1ProductId);
                break;
            case 1:
                BuyProductID(AppData.gemsTier2ProductId);
                break;
            case 2:
                BuyProductID(AppData.gemsTier3ProductId);
                break;
            case 3:
                BuyProductID(AppData.gemsTier4ProductId);
                break;
            default:
                break;
        }
    }

    private void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");
            //var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            //apple.RestoreTransactions((result) =>
            //{
            //    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            //});
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        switch (args.purchasedProduct.definition.id)
        {
            case AppData.gemsTier1ProductId:
                UiGemsSpawnCanvas.OnSpawnMultipleGem2d?.Invoke(AppData.gemsTier1GemsValue);
                break;
            case AppData.gemsTier2ProductId:
                UiGemsSpawnCanvas.OnSpawnMultipleGem2d?.Invoke(AppData.gemsTier2GemsValue);
                break;
            case AppData.gemsTier3ProductId:
                UiGemsSpawnCanvas.OnSpawnMultipleGem2d?.Invoke(AppData.gemsTier3GemsValue);
                break;
            case AppData.gemsTier4ProductId:
                UiGemsSpawnCanvas.OnSpawnMultipleGem2d?.Invoke(AppData.gemsTier4GemsValue);
                break;
            default:
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
                break;
        }
        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
    #endregion
}