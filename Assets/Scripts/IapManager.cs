using UnityEngine.Purchasing;
using System.Collections;
using System;
using Unity;
using System.Collections.Generic;

public class IapManager : Singleton<IapManager>
{
    public static Action<Product> OnPurchaseComplete;
    public static Action<Product> OnPurchaseFailed;
    private IAPListener iAPListener;
    private Dictionary<string, ProductCatalogItem> products = new Dictionary<string, ProductCatalogItem>();

    protected override void Awake()
    {
        base.Awake();
        iAPListener = GetComponent<IAPListener>();
        iAPListener.onPurchaseComplete.AddListener(onPurchaseComplete);
        iAPListener.onPurchaseFailed.AddListener(onPurchaseFailed);
        ProductCatalog productCatalog = ProductCatalog.LoadDefaultCatalog();
        foreach (var item in productCatalog.allProducts)
        {
            products.Add(item.id, item);
        }
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        iAPListener.onPurchaseComplete.RemoveListener(onPurchaseComplete);
        iAPListener.onPurchaseFailed.RemoveListener(onPurchaseFailed);
    }

    private void onPurchaseComplete(Product product)
    {
        OnPurchaseComplete?.Invoke(product);
        Hud.SetHudText?.Invoke(product.definition.id + "  OnPurchaseComplete");
    }

    private void onPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        OnPurchaseFailed?.Invoke(product);
        Hud.SetHudText?.Invoke(product.definition.id + "  OnPurchaseFailed " + "/n" + failureReason);
    }

    public ProductCatalogItem GetProductByItem(string itemId)
    {
        return products[itemId];
    }
}