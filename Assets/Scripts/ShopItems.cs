using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShopItems
{
    public static Dictionary<string, ShopItem> allShopItems = new Dictionary<string, ShopItem>();
    private static bool isShopItemsLoaded;

    public static void LoadShopDatabse()
    {
        if (isShopItemsLoaded)
        {
            return;
        }
        TextAsset mytxtData = (TextAsset)Resources.Load(AppData.shopItemsDbJsonPath);
        AllShopItems items = JsonUtility.FromJson<AllShopItems>(mytxtData.text);
        for (int i = 0; i < items.ShopItems.Count; i++)
        {
            Enum.TryParse(items.ShopItems[i].currencyType, out items.ShopItems[i].currencyTypeEnum);
            Enum.TryParse(items.ShopItems[i].itemType, out items.ShopItems[i].itemTypeEnum);
            allShopItems.Add(items.ShopItems[i].id, items.ShopItems[i]);
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