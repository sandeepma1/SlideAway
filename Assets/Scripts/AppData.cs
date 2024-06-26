﻿internal static class AppData
{
    internal static int currentScore;
    //Achivements
    internal const int achievementValue1 = 50;
    internal const int achievementValue2 = 100;
    internal const int achievementValue3 = 250;
    internal const int achievementValue4 = 500;
    internal const int achievementValue5 = 1000;
    internal const int achievementValue6 = 1500;
    internal const int achievementValue7 = 3000;
    internal const int achievementValue8 = 5000;
    internal const int achievementValue9 = 10000;

    //IAP
    internal const string gemsTier1ProductId = "com.bronz.slideway.coins500";
    internal const string gemsTier2ProductId = "com.bronz.slideway.coins3500";
    internal const string gemsTier3ProductId = "com.bronz.slideway.coins8000";
    internal const string gemsTier4ProductId = "com.bronz.slideway.coins15000";

    internal const int gemsTier1GemsValue = 500;
    internal const int gemsTier2GemsValue = 3000;
    internal const int gemsTier3GemsValue = 7500;
    internal const int gemsTier4GemsValue = 16000;

    //Ball variables
    internal const float minSpeed = 12;
    internal const float maxSpeed = 18;

    //Tile Manager
    internal const int gemSpawnRate = 8;//higher is less spawn

    //Rewards
    internal const int nextRewardInHours = 6;
    internal const int adGemsRewards = 50;
    internal const int dailyGemsRewards = 75;

    //Misc
    internal const float floorSaturation = 0.8f;
    internal const float floorLightness = 0.75f;

    internal const float shopAnimSpeed = 0.25f;
    internal const float gemsAnimSpeed = 0.75f;

    //Local Playerprefs keys
    internal const string localSaveKey = "localSave";
    internal const string oldSaveKey = "BestScore";

    internal const string shopItemsDbJsonPath = "ShopDatabase/ShopDatabase";
    internal const string allShopItemsIconPath = "AllItemIcons";
    internal const string allShopItemsMatPath = "AllItemMaterials";

    internal const string gemIcon = "<sprite=0>";
    internal const string adIcon = "<sprite=1>";

    internal const string saveVersion = "0.1";
}