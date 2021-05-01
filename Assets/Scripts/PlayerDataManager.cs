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
    private bool isShopItemsLoaded = false;
    public ShopItems allShopItems;


    protected override void Awake()
    {
        base.Awake();
        PlayerController.OnGameOver += OnGameOver;
        GpsManager.OnCloudDataLoaded += OnCloudDataLoaded;
        LoadDatabse();
    }

    private void OnDestroy()
    {
        PlayerController.OnGameOver -= OnGameOver;
        GpsManager.OnCloudDataLoaded -= OnCloudDataLoaded;
    }

    public void LoadDatabse()
    {
        if (isShopItemsLoaded)
        {
            return;
        }
        TextAsset mytxtData = (TextAsset)Resources.Load(AppData.shopItemsDbJsonPath);
        allShopItems = JsonUtility.FromJson<ShopItems>(mytxtData.text);
        for (int i = 0; i < allShopItems.BallItems.Count; i++)
        {
            Enum.TryParse(allShopItems.BallItems[i].type, out allShopItems.BallItems[i].typeEnum);
            allShopItems.BallItems[i].isUnlocked = IsBallIdUnlocked(allShopItems.BallItems[i].id);
        }
        for (int i = 0; i < allShopItems.FloorItems.Count; i++)
        {
            Enum.TryParse(allShopItems.FloorItems[i].type, out allShopItems.FloorItems[i].typeEnum);
            allShopItems.FloorItems[i].isUnlocked = IsFloorIdUnlocked(allShopItems.FloorItems[i].id);
        }
        for (int i = 0; i < allShopItems.BackgroundItems.Count; i++)
        {
            Enum.TryParse(allShopItems.BackgroundItems[i].type, out allShopItems.BackgroundItems[i].typeEnum);
            allShopItems.BackgroundItems[i].isUnlocked = IsBackgroundIdUnlocked(allShopItems.BackgroundItems[i].id);
        }
        isShopItemsLoaded = true;
    }

    #region Local Save/Load Sytem <---- Replace with a encrypt save load
    public void SaveLocalData(string save)
    {
        PlayerPrefs.SetString(AppData.localSaveKey, save);
    }

    public string LoadLocalData()
    {
        return PlayerPrefs.GetString(AppData.localSaveKey);
    }
    #endregion


    #region Load data at start *** Very Imp 
    private void OnCloudDataLoaded(bool isCloudDataLoaded, string cloudData)
    {
        //print("OnCloudDataLoaded");
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

        if (allShopItems == null)
        {
            LoadDatabse();
        }
        playerData.adsWatchedBall = new List<AdsWatched>();
        for (int i = 0; i < allShopItems.BallItems.Count; i++)
        {
            if (allShopItems.BallItems[i].typeEnum == PurchaseType.Ads)
            {
                playerData.adsWatchedBall.Add(new AdsWatched(allShopItems.BallItems[i].id, (int)allShopItems.BallItems[i].value));
            }
        }
        playerData.adsWatchedFloor = new List<AdsWatched>();
        for (int i = 0; i < allShopItems.FloorItems.Count; i++)
        {
            if (allShopItems.FloorItems[i].typeEnum == PurchaseType.Ads)
            {
                playerData.adsWatchedFloor.Add(new AdsWatched(allShopItems.FloorItems[i].id, (int)allShopItems.FloorItems[i].value));
            }
        }
        playerData.adsWatchedBackground = new List<AdsWatched>();
        for (int i = 0; i < allShopItems.BackgroundItems.Count; i++)
        {
            if (allShopItems.BackgroundItems[i].typeEnum == PurchaseType.Ads)
            {
                playerData.adsWatchedBackground.Add(new AdsWatched(allShopItems.BackgroundItems[i].id, (int)allShopItems.BackgroundItems[i].value));
            }
        }

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
    public bool IsBallIdUnlocked(string id)
    {
        return playerData.unlockedBallIds.Contains(id);
    }

    public bool IsFloorIdUnlocked(string id)
    {
        return playerData.unlockedFloorIds.Contains(id);
    }

    public bool IsBackgroundIdUnlocked(string id)
    {
        return playerData.unlockedBackgroundIds.Contains(id);
    }

    public string CurrentSelectedBallId
    {
        get { return playerData.currentBallId; }
        set { playerData.currentBallId = value; SaveGameUserData(); }
    }

    public string CurrentSelectedFloorId
    {
        get { return playerData.currentFloorId; }
        set { playerData.currentFloorId = value; SaveGameUserData(); }
    }

    public string CurrentSelectedBackgroundId
    {
        get { return playerData.currentBackgroundId; }
        set { playerData.currentBackgroundId = value; SaveGameUserData(); }
    }

    public bool IsSoundEnabled
    {
        get { return playerData.isSoundEnabled; }
        set { playerData.isSoundEnabled = value; }
    }

    public bool IsVibrateEnabled
    {
        get { return playerData.isVibrateEnabled; }
        set { playerData.isVibrateEnabled = value; }
    }

    public DateTime RewardDateTime
    {
        get { return rewardsDateTime; }
        set
        {
            rewardsDateTime = value;
            playerData.nextRewardDateTime = JsonUtility.ToJson((JsonDateTime)rewardsDateTime);
        }
    }

    public bool IsPlayerDataNull()
    {
        return playerData == null;
    }

    public int GetHighScore()
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

    public int GetRetries()
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

    public int GetGems()
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

    public void IncrementGems(int adder)
    {
        playerData.gems += adder;
        UiPlayerDataHud.OnUpdateGemsValue?.Invoke();
        // SaveGameUserDataOnCloud();
    }

    public void DecrementGems(int adder)
    {
        if (playerData.gems >= adder)
        {
            playerData.gems -= adder;
            UiPlayerDataHud.OnUpdateGemsValue?.Invoke();
            SaveGameUserData();
        }
    }

    public void IncrementRetries()
    {
        playerData.retries++;
        SaveGameUserData();
    }

    public void AddBallUnlockedId(string ballId)
    {
        if (!playerData.unlockedBallIds.Contains(ballId))
        {
            playerData.unlockedBallIds.Add(ballId);
            SaveGameUserData();
        }
    }

    public void AddFloorUnlockedId(string floorId)
    {
        if (!playerData.unlockedFloorIds.Contains(floorId))
        {
            playerData.unlockedFloorIds.Add(floorId);
            SaveGameUserData();
        }
    }

    public void AddBackgroundUnlockedId(string backgroundId)
    {
        if (!playerData.unlockedBackgroundIds.Contains(backgroundId))
        {
            playerData.unlockedBackgroundIds.Add(backgroundId);
            SaveGameUserData();
        }
    }

    public void AdWatchedRewardBall(string ballId)
    {
        for (int i = 0; i < playerData.adsWatchedBall.Count; i++)
        {
            if (playerData.adsWatchedBall[i].id == ballId)
            {
                playerData.adsWatchedBall[i].count--;
                if (playerData.adsWatchedBall[i].count <= 0)
                {
                    AddBallUnlockedId(playerData.adsWatchedBall[i].id);
                    //be aware
                    playerData.adsWatchedBall.Remove(playerData.adsWatchedBall[i]);
                    break;
                }
                break;
            }
        }
        SaveGameUserData();
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

    public void SaveGameUserData()
    {
        if (!isPlayerDataLoaded)
        {
            return;
        }
        playerData.lastPlayedDateTime = JsonUtility.ToJson((JsonDateTime)DateTime.UtcNow);
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
    public List<string> unlockedBallIds;
    public List<AdsWatched> adsWatchedBall;
    public string currentBallId;
    public List<string> unlockedFloorIds;
    public List<AdsWatched> adsWatchedFloor;
    public string currentFloorId;
    public List<string> unlockedBackgroundIds;
    public List<AdsWatched> adsWatchedBackground;
    public string currentBackgroundId;
    public string nextRewardDateTime;
    public bool isSoundEnabled;
    public bool isVibrateEnabled;
    public string lastPlayedDateTime;

    public PlayerData()
    {
        gems = 100;
        highScore = 0;
        retries = 0;

        unlockedBallIds = new List<string>();
        unlockedBallIds.Add("gem0");
        currentBallId = "gem0";

        unlockedFloorIds = new List<string>();
        unlockedFloorIds.Add("gem0");
        currentFloorId = "gem0";

        unlockedBackgroundIds = new List<string>();
        unlockedBackgroundIds.Add("gem0");
        currentBackgroundId = "gem0";
        nextRewardDateTime = JsonUtility.ToJson((JsonDateTime)DateTime.UtcNow);
        lastPlayedDateTime = JsonUtility.ToJson((JsonDateTime)DateTime.UtcNow);
        isSoundEnabled = true;
        isVibrateEnabled = true;
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

[System.Serializable]
public class ShopItems
{
    public List<ShopItem> BallItems;
    public List<ShopItem> FloorItems;
    public List<ShopItem> BackgroundItems;
}
[System.Serializable]
public class ShopItem
{
    public string id;
    public string type;
    public float value;
    public PurchaseType typeEnum;
    public bool isUnlocked;
}
public enum PurchaseType
{
    Gems,
    Ads,
    Paid
}
