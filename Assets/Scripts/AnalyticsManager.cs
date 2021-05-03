using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public enum GameButtons
{
    Achievements,
    Leaderboards,
    RateUs,
    Settings,
    Shop,
    UnlockItem,
    PaidUnlock,
    GemsUnlock,
    AdUnclock,
    DailyReward,
    Sound,
    Vibrate,
    Facebook,
    Instagram,
    GpsLogin,
    FreeGemsByAd,
    Restart,
    Share
}

public enum GameScreens
{
    Achievements,
    Leaderboards,
    SettingsMenu,
    ShopMenu,
    BallShopMenu,
    FloorShopMenu,
    BackgroundShopMenu,
    RestartMenu,
    GameOver
}

public static class AnalyticsManager
{
    internal static Dictionary<string, object> customEvent = new Dictionary<string, object>();
    internal static bool showAnalyticsResults = true;

    internal static void InitAnalytics()
    {
        customEvent.Add("Game Over", AppData.currentScore);
    }

    internal static void GameOverCurrentScore()
    {
        AnalyticsResult("GameOverCurrentScore: ", AnalyticsEvent.Custom
            ("CurrentScore", new Dictionary<string, object> { { "CurrentScore", AppData.currentScore } }));
    }

    internal static void ButtonPressed(GameButtons gameButtons)
    {
        AnalyticsResult("ButtonPressed: ", AnalyticsEvent.Custom(gameButtons.ToString()));
    }

    internal static void ShopItemClicked(string itemId)
    {
        AnalyticsResult("ShopItemClicked: ", AnalyticsEvent.Custom
            ("ShopItemClicked: ", new Dictionary<string, object>
             { {  itemId, Player.IsItemUnlocked(itemId) } }));
    }

    internal static void UnlockButtonClicked(string itemId)
    {
        AnalyticsResult("UnlockButtonClicked: ", AnalyticsEvent.Custom
            ("UnlockButtonClicked: ", new Dictionary<string, object>
             {
                {  itemId, Shop.items[itemId].currencyTypeEnum},
                 {  "Gems", Player.save.gems}
            }));
    }

    internal static void GameStartSaveType(bool isCloud)
    {
        AnalyticsResult("GameStartSaveType: ", AnalyticsEvent.Custom
            ("SaveType: ", new Dictionary<string, object>
             {
                {  "SaveType", isCloud}
            }));
    }

    private static void AnalyticsResult(string name, AnalyticsResult result)
    {
        if (showAnalyticsResults) { Debug.Log(name + ": " + result); }
    }

    #region Standard Events
    internal static void GameStart()
    {
        AnalyticsResult("GameStart: ", AnalyticsEvent.GameStart());
    }

    internal static void AchievementUnlocked(string achievementId, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("AchievementUnlocked: ", AnalyticsEvent.AchievementUnlocked(achievementId));
    }

    internal static void AdComplete(bool isRewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("AdComplete: ", AnalyticsEvent.AdComplete(isRewarded));
    }

    internal static void AdSkip(bool isRewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("AdSkip: ", AnalyticsEvent.AdSkip(isRewarded));
    }

    internal static void AdStart(bool isRewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("AdStart: ", AnalyticsEvent.AdStart(isRewarded));
    }

    internal static void GameOver()
    {
        AnalyticsResult("GameOver: ", AnalyticsEvent.GameOver());
    }

    internal static void IAPTransaction(string transactionContext, float price, string itemId, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("IAPTransaction: ", AnalyticsEvent.IAPTransaction(transactionContext, price, itemId));
    }

    internal static void ItemAcquired(AcquisitionType currencyType, string transactionContext, float amount, string itemId, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("ItemAcquired: ", AnalyticsEvent.ItemAcquired(currencyType, transactionContext, amount, itemId));
    }

    internal static void ItemSpent(AcquisitionType currencyType, string transactionContext, float amount, string itemId)
    {
        AnalyticsResult("ItemSpent: ", AnalyticsEvent.ItemSpent(currencyType, transactionContext, amount, itemId));
    }

    internal static void ScreenVisit(GameScreens screenName, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("ScreenVisit: ", AnalyticsEvent.ScreenVisit(screenName.ToString()));
    }

    internal static void SocialShare(ShareType shareType, SocialNetwork socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("SocialShare: ", AnalyticsEvent.SocialShare(shareType, socialNetwork));
    }

    internal static void StoreItemClick(StoreType storeType, string itemId, string itemName = null, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("StoreItemClick: ", AnalyticsEvent.StoreItemClick(storeType, itemId));
    }

    internal static void StoreOpened(StoreType storeType, IDictionary<string, object> eventData = null)
    {
        AnalyticsResult("StoreOpened: ", AnalyticsEvent.StoreOpened(storeType));
    }
    #endregion
}