using System;
using System.Collections.Generic;
using UnityEngine;

public static class Shop
{
    public static Dictionary<string, ShopItem> items = new Dictionary<string, ShopItem>();
    private static bool isShopItemsLoaded;

    public static void LoadShopDatabse()
    {
        if (isShopItemsLoaded)
        {
            return;
        }
        TextAsset mytxtData = (TextAsset)Resources.Load(AppData.shopItemsDbJsonPath);
        AllShopItems shopItems = JsonUtility.FromJson<AllShopItems>(mytxtData.text);
        for (int i = 0; i < shopItems.ShopItems.Count; i++)
        {
            Enum.TryParse(shopItems.ShopItems[i].currencyType, out shopItems.ShopItems[i].currencyTypeEnum);
            Enum.TryParse(shopItems.ShopItems[i].itemType, out shopItems.ShopItems[i].itemTypeEnum);
            items.Add(shopItems.ShopItems[i].id, shopItems.ShopItems[i]);
        }
        isShopItemsLoaded = true;
    }
}

#region Shop Database
[System.Serializable]
public class AllShopItems
{
    public List<ShopItem> ShopItems;
}
[System.Serializable]
public class ShopItem
{
    public string id;
    public string itemType;
    public string currencyType;
    public float value;
    public CurrencyType currencyTypeEnum;
    public ShopItemType itemTypeEnum;
}
public enum CurrencyType
{
    Gems,
    Ads,
    Paid
}
public enum ShopItemType
{
    Ball,
    Floor,
    Background
}
#endregion