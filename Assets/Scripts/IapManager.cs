using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class IapManager : MonoBehaviour
{
    [SerializeField] private IAPButton add500CoinsIAPButton;
    [SerializeField] private IAPButton removeAdsIAPButton;
    [SerializeField] private IAPButton restorePurchaseIAPButton;


    private void Start()
    {
        add500CoinsIAPButton.onPurchaseComplete.AddListener(OnAdd500CoinsPurchasedComplete);
        add500CoinsIAPButton.onPurchaseFailed.AddListener(OnAdd500CoinsPurchasedFailed);

        removeAdsIAPButton.onPurchaseComplete.AddListener(OnRemoveAdsPurchasedComplete);
        removeAdsIAPButton.onPurchaseFailed.AddListener(OnRemoveAdsPurchasedFailed);

        restorePurchaseIAPButton.onPurchaseComplete.AddListener(OnRestorePurchasePressed);
        restorePurchaseIAPButton.onPurchaseFailed.AddListener(OnRestorePurchaseFailed);
    }

    private void OnAdd500CoinsPurchasedFailed(Product product, PurchaseFailureReason purchaseFailureReason)
    {
        Hud.SetHudText?.Invoke("OnAdd500CoinsPurchasedFailed \n" + purchaseFailureReason);
    }

    private void OnRemoveAdsPurchasedFailed(Product product, PurchaseFailureReason purchaseFailureReason)
    {
        Hud.SetHudText?.Invoke("OnRemoveAdsPurchasedFailed \n" + purchaseFailureReason);
    }

    private void OnRestorePurchaseFailed(Product product, PurchaseFailureReason purchaseFailureReason)
    {
        Hud.SetHudText?.Invoke("OnRestorePurchaseFailed \n" + purchaseFailureReason);
    }

    private void OnRestorePurchasePressed(Product product)
    {
        Hud.SetHudText?.Invoke("OnRestorePurchasePressed");
    }

    private void OnRemoveAdsPurchasedComplete(Product product)
    {
        Hud.SetHudText?.Invoke("OnRemoveAdsPurchasedComplete");
    }

    private void OnAdd500CoinsPurchasedComplete(Product product)
    {
        Hud.SetHudText?.Invoke("OnAdd500CoinsPurchasedComplete");
    }
}
