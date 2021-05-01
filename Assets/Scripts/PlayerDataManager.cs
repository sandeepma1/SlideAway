using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    public static Action OnPlayerDataLoaded;
    public static Action<PlayerData> UpdatePlayerDataOnUI;
    public static Action<string> OnUpdateRewardTimer;
    public static Action OnRewardAvailable;
    public PlayerData playerData;
    private DateTime rewardsDateTime;
    private DateTime lastPlayedDateTime;
    [SerializeField] private TimeSpan rewardTimeSpan;
    public bool isPlayerDataLoaded = false;
    public bool isCloudDataLoaded = false;

    protected override void Awake()
    {
        base.Awake();
        PlayerController.OnGameOver += OnGameOver;
        GpsManager.OnCloudDataLoaded += OnCloudDataLoaded;
        ShopItems.LoadShopDatabse();
    }

    private void OnDestroy()
    {
        PlayerController.OnGameOver -= OnGameOver;
        GpsManager.OnCloudDataLoaded -= OnCloudDataLoaded;
    }

    internal bool IsItemUnlocked(string itemId)
    {
        return playerData.unlockedItemIds.Contains(itemId);
    }


    #region Local Save/Load Sytem <---- Replace with a encrypt save load
    private void SaveLocalData(string save)
    {
        PlayerPrefs.SetString(AppData.localSaveKey, save);
    }

    private string LoadLocalData()
    {
        return PlayerPrefs.GetString(AppData.localSaveKey);
    }
    #endregion


    #region Load data at start *** Very Imp 
    private void OnCloudDataLoaded(bool isCloudDataLoaded, string cloudData)
    {
        this.isCloudDataLoaded = isCloudDataLoaded;
        Hud.SetHudText?.Invoke("Started loading data " + cloudData);
        if (PlayerPrefs.HasKey(AppData.oldSaveKey)) //Has old saves, add it
        {
            Hud.SetHudText?.Invoke("PlayerPrefs.HasKey(BestScore)... Creting new save");
            CreateNewSaveData();
        }
        else
        {
            if (isCloudDataLoaded) //Load cloud data
            {
                if (string.IsNullOrEmpty(cloudData)) //New game save
                {
                    CreateNewSaveData();
                }
                else //Load from cloud save
                {
                    LoadSaveData(true, cloudData);
                }
            }
            else //load local save
            {
                if (PlayerPrefs.HasKey(AppData.localSaveKey))
                {
                    LoadSaveData(false, LoadLocalData());
                }
                else
                {
                    CreateNewSaveData();//New game save from old istalled device 
                }
            }
        }
        isPlayerDataLoaded = true;
        OnPlayerDataLoaded?.Invoke();
        InvokeRepeating("CheckReward", 1f, 1f);
    }

    private void LoadSaveData(bool isCloudSave, string saveData)
    {
        //ProcessLatestSave(isCloudSave, saveData);
        playerData = JsonUtility.FromJson<PlayerData>(saveData);
        //Created dictonary from a list of AdsWatchedItems for ease
        playerData.ListToDictonary();
        lastPlayedDateTime = JsonUtility.FromJson<JsonDateTime>(playerData.lastPlayedDateTime);
        rewardsDateTime = JsonUtility.FromJson<JsonDateTime>(playerData.nextRewardDateTime);
    }

    private void CreateNewSaveData()
    {
        playerData = new PlayerData();
        playerData.highScore = PlayerPrefs.GetInt(AppData.oldSaveKey);
        PlayerPrefs.DeleteKey(AppData.oldSaveKey);
        rewardsDateTime = DateTime.UtcNow;
        Hud.SetHudText?.Invoke("Created new save");
        if (playerData.highScore >= AppData.achievementValue1)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_first_50);
        }
        if (playerData.highScore >= AppData.achievementValue2)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_century_100);
        }
        if (playerData.highScore >= AppData.achievementValue3)
        {
            GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_next_250);
        }
        SaveGameUserData();
    }

    //private void ProcessLatestSave(bool isCloudSave, string saveData)
    //{
    //    DateTime cloudLastPlayedDateTime;
    //    DateTime localLastPlayedDateTime;

    //    if (isCloudSave)
    //    {
    //        cloudLastPlayedDateTime = JsonUtility.FromJson<JsonDateTime>(playerData.lastPlayedDateTime);
    //    }
    //    else
    //    {
    //        string localDataString = LoadLocalData();
    //        if (!String.IsNullOrEmpty(localDataString))
    //        {
    //            PlayerData localData = JsonUtility.FromJson<PlayerData>(localDataString);
    //            localLastPlayedDateTime = JsonUtility.FromJson<JsonDateTime>(localData.lastPlayedDateTime);
    //        }

    //    }
    //}

    //Do not delete InvokeRepeating("CheckReward", 1f, 1f);
    private void CheckReward()
    {
        rewardTimeSpan = rewardsDateTime.Subtract(DateTime.UtcNow);
        if (rewardTimeSpan.TotalSeconds <= 0)
        {
            OnRewardAvailable?.Invoke();
        }
        else
        {
            OnUpdateRewardTimer?.Invoke(rewardTimeSpan.ToFormattedDuration());
        }
    }
    #endregion


    #region Get Set Player Data
    internal bool IsSoundEnabled
    {
        get { return playerData.isSoundEnabled; }
        set { playerData.isSoundEnabled = value; }
    }

    internal bool IsVibrateEnabled
    {
        get { return playerData.isVibrateEnabled; }
        set { playerData.isVibrateEnabled = value; }
    }

    internal DateTime RewardDateTime
    {
        get { return rewardsDateTime; }
        set
        {
            rewardsDateTime = value;
            playerData.nextRewardDateTime = JsonUtility.ToJson((JsonDateTime)rewardsDateTime);
        }
    }

    internal bool IsPlayerDataNull()
    {
        return playerData == null;
    }

    internal int GetHighScore()
    {
        if (playerData != null)
        {
            return playerData.highScore;
        }
        else
        {
            return 0;
        }
    }

    internal int GetRetries()
    {
        if (playerData != null)
        {
            return playerData.retries;
        }
        else
        {
            return 0;
        }
    }

    internal int GetGems()
    {
        if (playerData != null)
        {
            return playerData.gems;
        }
        else
        {
            return 0;
        }
    }

    internal void IncrementGems(int adder)
    {
        playerData.gems += adder;
        UiPlayerDataHud.OnUpdateGemsValue?.Invoke();
        // SaveGameUserDataOnCloud();
    }

    internal void DecrementGems(int adder)
    {
        if (playerData.gems >= adder)
        {
            playerData.gems -= adder;
            UiPlayerDataHud.OnUpdateGemsValue?.Invoke();
            SaveGameUserData();
        }
    }

    internal void IncrementRetries()
    {
        playerData.retries++;
        SaveGameUserData();
    }

    internal void AddItemUnlockedId(string itemId)
    {
        if (!playerData.unlockedItemIds.Contains(itemId))
        {
            playerData.unlockedItemIds.Add(itemId);
            SaveGameUserData();
        }
    }
    #endregion

    private void OnGameOver()
    {
        if (AppData.currentScore > playerData.highScore)
        {
            playerData.highScore = AppData.currentScore;
        }
        GpsManager.Instance.PostScoreToLeaderboard(playerData.highScore, GPGSIds.leaderboard_high_score__slide_away);
        SaveGameUserData();
    }

    #region Save Game on Exit or Pause
    private void OnApplicationQuit()
    {
        SaveGameUserData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) { SaveGameUserData(); }
    }

    internal void SaveGameUserData()
    {
        if (!isPlayerDataLoaded)
        {
            return;
        }
        playerData.lastPlayedDateTime = JsonUtility.ToJson((JsonDateTime)DateTime.UtcNow);
        playerData.DictonaryToList();
        string save = JsonUtility.ToJson(playerData);
        Hud.SetHudText?.Invoke("saving " + save);
        SaveLocalData(save);
        GpsManager.Instance.SaveToCloud(save);
    }
    #endregion
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
        unlockedItemIds.Add("0BallGems");
        unlockedItemIds.Add("0FloorGems");
        unlockedItemIds.Add("0BackgroundGems");
        currentSelectedItemIds = new string[3];
        currentSelectedItemIds[0] = "0BallGems";
        currentSelectedItemIds[1] = "0FloorGems";
        currentSelectedItemIds[2] = "0BackgroundGems";
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