using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class Player
{
    internal static Action OnPlayerDataLoaded;
    internal static Action<string> OnUpdateRewardTimer;
    internal static Action OnRewardAvailable;

    internal static DateTime lastPlayedDateTime;
    internal static DateTime rewardsDateTime;
    internal static bool isPlayerDataLoaded = false;
    internal static bool isCloudDataLoaded = false;

    internal static PlayerData save;

    internal static void InitPlayer()
    {
        PlayerController.OnGameOver += OnGameOver;
        GpsManager.OnSaveDataLoaded += OnSaveDataLoaded;
    }

    internal static void OnDestroyPlayer()
    {
        PlayerController.OnGameOver -= OnGameOver;
        GpsManager.OnSaveDataLoaded -= OnSaveDataLoaded;
    }

    internal static bool IsItemUnlocked(string itemId)
    {
        return save.unlockedItemIds.Contains(itemId);
    }

    #region Local Save/Load Sytem <---- Replace with a encrypt save load
    private static void SaveLocalData(string save)
    {
        PlayerPrefs.SetString(AppData.localSaveKey, save);
    }

    internal static string LoadLocalData()
    {
        return PlayerPrefs.GetString(AppData.localSaveKey);
    }
    #endregion


    #region Load data at start *** Very Imp 
    private static void OnSaveDataLoaded(bool isCloudDataLoaded, string saveData)
    {
        Hud.SetHudText?.Invoke("isCloudData: " + isCloudDataLoaded + "\n" + saveData);
        Player.isCloudDataLoaded = isCloudDataLoaded;
        //If saved data is empty, create new
        if (String.IsNullOrEmpty(saveData))
        {
            CreateNewSaveData();
        }
        else
        {
            if (isCloudDataLoaded) //If has data and cloud data
            {
                CompareCloudWithLocalSaveAndLoad(saveData);
            }
            else
            {
                LoadSaveData(LoadLocalData());
            }
        }
        isPlayerDataLoaded = true;
        OnPlayerDataLoaded?.Invoke();
        AnalyticsManager.GameStartSaveType(isCloudDataLoaded);
    }

    private static void CreateNewSaveData()
    {
        save = new PlayerData();
        if (PlayerPrefs.HasKey(AppData.oldSaveKey))
        {
            save.highScore = PlayerPrefs.GetInt(AppData.oldSaveKey);
            PlayerPrefs.DeleteKey(AppData.oldSaveKey);
        }
        rewardsDateTime = DateTime.UtcNow;
        Hud.SetHudText?.Invoke("Created new save, checking achievements");
        if (save.highScore >= AppData.achievementValue1)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_50);
        }
        if (save.highScore >= AppData.achievementValue2)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_100);
        }
        if (save.highScore >= AppData.achievementValue3)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_250);
        }
        if (save.highScore >= AppData.achievementValue4)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_500);
        }
        if (save.highScore >= AppData.achievementValue5)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_1000);
        }
        if (save.highScore >= AppData.achievementValue6)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_1500);
        }
        if (save.highScore >= AppData.achievementValue7)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_3000);
        }
        if (save.highScore >= AppData.achievementValue8)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_5000);
        }
        if (save.highScore >= AppData.achievementValue9)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_10000);
        }
        SaveGameUserData();
    }

    private static void CompareCloudWithLocalSaveAndLoad(string cloudData)
    {
        long cloudTicks = 0;
        long localTicks = 0;

        PlayerData tempCloudSave = JsonUtility.FromJson<PlayerData>(cloudData);
        if (tempCloudSave != null && !String.IsNullOrEmpty(tempCloudSave.lastPlayedDateTime))
        {
            DateTime cloud = JsonUtility.FromJson<JsonDateTime>(tempCloudSave.lastPlayedDateTime);
            cloudTicks = cloud.Ticks;
            Hud.SetHudText?.Invoke("cloudTicks " + cloudTicks);
        }
        else
        {
            Hud.SetHudText?.Invoke("tempCloudSave is null or.. CreateNewSaveData");
            CreateNewSaveData();
        }

        PlayerData tempLocalSave = JsonUtility.FromJson<PlayerData>(LoadLocalData());
        if (tempLocalSave != null && !String.IsNullOrEmpty(tempLocalSave.lastPlayedDateTime))
        {
            DateTime local = JsonUtility.FromJson<JsonDateTime>(tempLocalSave.lastPlayedDateTime);
            localTicks = local.Ticks;
            Hud.SetHudText?.Invoke("localTicks " + localTicks);
        }
        else
        {
            Hud.SetHudText?.Invoke("tempLocalSave is null " + LoadLocalData());
            LoadSaveData(cloudData);
            return;
        }

        if (localTicks > cloudTicks)
        {
            LoadSaveData(LoadLocalData());
            Hud.SetHudText?.Invoke("Loading local");
        }
        else
        {
            LoadSaveData(cloudData);
            Hud.SetHudText?.Invoke("Loading cloud");
        }
    }

    private static void LoadSaveData(string saveData)
    {
        save = JsonUtility.FromJson<PlayerData>(saveData);
        save.ListToDictonary();
        lastPlayedDateTime = JsonUtility.FromJson<JsonDateTime>(save.lastPlayedDateTime);
        rewardsDateTime = JsonUtility.FromJson<JsonDateTime>(save.nextRewardDateTime);
    }

    #endregion


    #region Get Set Player Data
    internal static bool IsSoundEnabled
    {
        get { return save.isSoundEnabled; }
        set { save.isSoundEnabled = value; }
    }

    internal static bool IsVibrateEnabled
    {
        get { return save.isVibrateEnabled; }
        set { save.isVibrateEnabled = value; }
    }

    internal static DateTime RewardDateTime
    {
        get { return rewardsDateTime; }
        set
        {
            rewardsDateTime = value;
            save.nextRewardDateTime = JsonUtility.ToJson((JsonDateTime)rewardsDateTime);
        }
    }

    internal static bool IsPlayerDataNull()
    {
        return save == null;
    }

    internal static int GetHighScore()
    {
        if (save != null)
        {
            return save.highScore;
        }
        else
        {
            return 0;
        }
    }

    internal static int GetRetries()
    {
        if (save != null)
        {
            return save.retries;
        }
        else
        {
            return 0;
        }
    }

    internal static int GetGems()
    {
        if (save != null)
        {
            return save.gems;
        }
        else
        {
            return 0;
        }
    }

    internal static void IncrementGems(int adder)
    {
        save.gems += adder;
        SaveGameUserData();
    }

    internal static void DecrementGems(int adder)
    {
        if (save.gems >= adder)
        {
            save.gems -= adder;
            UiPlayerDataHud.OnUpdateGemsValue?.Invoke();
            SaveGameUserData();
        }
    }

    internal static void IncrementRetries()
    {
        save.retries++;
        SaveGameUserData();
    }

    internal static void AddItemUnlockedId(string itemId)
    {
        if (!save.unlockedItemIds.Contains(itemId))
        {
            save.unlockedItemIds.Add(itemId);
            SaveGameUserData();
            ShopItem item = Shop.items[itemId];
            switch (item.currencyTypeEnum)
            {
                case CurrencyType.Gems:
                    AnalyticsManager.ItemAcquired(UnityEngine.Analytics.AcquisitionType.Soft,
                        item.itemType, item.value, itemId);
                    break;
                case CurrencyType.Ads:
                    AnalyticsManager.ItemAcquired(UnityEngine.Analytics.AcquisitionType.Soft,
                      item.itemType, item.value, itemId);
                    break;
                case CurrencyType.Paid:
                    AnalyticsManager.ItemAcquired(UnityEngine.Analytics.AcquisitionType.Premium,
                      item.itemType, item.value, itemId);
                    break;
                default:
                    break;
            }
        }
    }

    internal static void AddWatchedAdsIds(string itemId)
    {
        if (Player.save.adsWatched.ContainsKey(itemId))// Already watched one or more ads
        {
            Player.save.adsWatched[itemId]--;
            if (Player.save.adsWatched[itemId] <= 0) // Item is unlocked, watched all ads
            {
                Player.AddItemUnlockedId(itemId);
                Player.save.adsWatched.Remove(itemId);
                AnalyticsManager.ItemSpent(UnityEngine.Analytics.AcquisitionType.Soft,
                   "Ads Unlocked", Shop.items[itemId].value, itemId);
            }
        }
        else //First time ad watched
        {
            int value = (int)Shop.items[itemId].value - 1;
            Player.save.adsWatched.Add(itemId, value);
        }
        SaveGameUserData();
    }
    #endregion

    private static void OnGameOver()
    {
        if (AppData.currentScore > save.highScore)
        {
            save.highScore = AppData.currentScore;
            //GpsManager.Instance.PostScoreToLeaderboard(save.highScore, GPGSIds.leaderboard_high_score);
        }
        GpsManager.Instance.PostScoreToLeaderboard(save.highScore, GPGSIds.leaderboard_high_score);
        SaveGameUserData();
    }

    internal static void SaveGameUserData()
    {
        if (!isPlayerDataLoaded)
        {
            return;
        }
        Player.save.lastPlayedDateTime = JsonUtility.ToJson((JsonDateTime)DateTime.UtcNow);
        Player.save.DictonaryToList();
        string save = JsonUtility.ToJson(Player.save);
        Hud.SetHudText?.Invoke("saving ");// + save);
        SaveLocalData(save);
        GpsManager.Instance.SaveToCloud(save);
    }
}

[System.Serializable]
public class PlayerData
{
    public int gems;
    public int highScore;
    public int retries;
    public List<string> unlockedItemIds;
    public List<AdsWatched> adsWatchedItemIds;
    public Dictionary<string, int> adsWatched = new Dictionary<string, int>();
    public string[] currentSelectedItemIds;
    public string nextRewardDateTime;
    public bool isSoundEnabled;
    public bool isVibrateEnabled;
    public string lastPlayedDateTime;

    public PlayerData()
    {
        gems = 100;
        highScore = 0;
        retries = 0;
        unlockedItemIds = new List<string>();
        unlockedItemIds.Add("0Ball");
        unlockedItemIds.Add("0Floor");
        unlockedItemIds.Add("0Background");
        currentSelectedItemIds = new string[3];
        currentSelectedItemIds[0] = "0Ball";
        currentSelectedItemIds[1] = "0Floor";
        currentSelectedItemIds[2] = "0Background";
        nextRewardDateTime = JsonUtility.ToJson((JsonDateTime)DateTime.UtcNow);
        lastPlayedDateTime = JsonUtility.ToJson((JsonDateTime)DateTime.UtcNow);
        isSoundEnabled = true;
        isVibrateEnabled = true;
    }

    public void ListToDictonary()
    {
        for (int i = 0; i < adsWatchedItemIds.Count; i++)
        {
            adsWatched.Add(adsWatchedItemIds[i].id, adsWatchedItemIds[i].count);
        }
    }

    public void DictonaryToList()
    {
        adsWatchedItemIds = new List<AdsWatched>();
        foreach (KeyValuePair<string, int> item in adsWatched)
        {
            adsWatchedItemIds.Add(new AdsWatched(item.Key, item.Value));
        }
    }
}

[System.Serializable]
public class AdsWatched
{
    public string id;
    public int count;
    public AdsWatched(string id, int count)
    {
        this.id = id;
        this.count = count;
    }
    public bool IsUnlocked()
    {
        return count <= 0;
    }
}
[System.Serializable]
public class JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt)
    {
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        JsonDateTime jdt = new JsonDateTime();
        if (dt.Ticks > 0)
        {
            jdt.value = dt.ToFileTimeUtc();
            return jdt;
        }
        return null;
    }
    //Example
    //DateTime time = DateTime.UtcNow;
    //print(time);
    //string json = JsonUtility.ToJson((JsonDateTime)time);
    //print(json);
    //DateTime timeFromJson = JsonUtility.FromJson<JsonDateTime>(json);
    //print(timeFromJson);
}