using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class Purchaser : Singleton<Purchaser>, IStoreListener
{
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    private void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
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

    public void BuyConsumable(int tierId)
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

    void BuyProductID(string productId)
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
                break;
            case AppData.gemsTier2ProductId:
                break;
            case AppData.gemsTier3ProductId:
                break;
            case AppData.gemsTier4ProductId:
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
}