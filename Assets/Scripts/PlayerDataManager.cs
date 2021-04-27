using System;
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
    [SerializeField] private TimeSpan rewardTimeSpan;
    public bool isPlayerDataLoaded = false;

    protected override void Awake()
    {
        base.Awake();
        PlayerController.OnGameOver += OnGameOver;
        GpsManager.OnCloudDataLoaded += OnCloudDataLoaded;
    }

    private void Start()
    {
#if UNITY_EDITOR
        OnCloudDataLoaded("");
#endif
    }

    private void OnDestroy()
    {
        PlayerController.OnGameOver -= OnGameOver;
        GpsManager.OnCloudDataLoaded -= OnCloudDataLoaded;
    }

    private void OnCloudDataLoaded(string cloudData)
    {
        if (string.IsNullOrEmpty(cloudData))
        {
            playerData = new PlayerData();
            rewardsDateTime = DateTime.UtcNow;
            SaveGameUserDataOnCloud();
        }
        else
        {
            playerData = JsonUtility.FromJson<PlayerData>(cloudData);
            rewardsDateTime = JsonUtility.FromJson<JsonDateTime>(playerData.nextRewardDateTime);
        }
        isPlayerDataLoaded = true;
        UpgradeSaveOfLastVersion();
        OnPlayerDataLoaded?.Invoke();
        InvokeRepeating("CheckReward", 1f, 1f);
    }

    private void UpgradeSaveOfLastVersion()
    {
        if (PlayerPrefs.HasKey("BestScore"))
        {
            playerData.highScore = PlayerPrefs.GetInt("BestScore");
            PlayerPrefs.DeleteKey("BestScore");
            SaveGameUserDataOnCloud();
        }
    }

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


    #region Get Set Player Data

    public bool IsIdUnloced(string id)
    {
        return playerData.unlockedBallIds.Contains(id);
    }

    public string CurrentSelectedBallId
    {
        get { return playerData.currentBallId; }
        set { playerData.currentBallId = value; }
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
    }

    public void DecrementGems(int adder)
    {
        if (playerData.gems >= adder)
        {
            playerData.gems -= adder;
        }
    }

    public void IncrementRetries()
    {
        playerData.retries++;
    }

    public void SetHighScore(int highScore)
    {
        playerData.highScore = highScore;
    }

    public void AddUnlockedId(string ballId)
    {
        if (!playerData.unlockedBallIds.Contains(ballId))
        {
            playerData.unlockedBallIds.Add(ballId);
        }
    }
    #endregion

    private void OnGameOver()
    {
        if (AppData.currentScore > playerData.highScore)
        {
            playerData.highScore = AppData.currentScore;
            GpsManager.Instance.PostScoreToLeaderboard(playerData.highScore, GPGSIds.leaderboard_high_score__slide_away);
        }
        SaveGameUserDataOnCloud();
    }

    #region Save Game on Exit or Pause
    private void OnApplicationQuit()
    {
        SaveGameUserDataOnCloud();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) { SaveGameUserDataOnCloud(); }
    }

    public void SaveGameUserDataOnCloud()
    {
        string save = JsonUtility.ToJson(playerData);
        Hud.SetHudText?.Invoke("saving " + save);
#if !UNITY_EDITOR
        GpsManager.Instance.SaveToCloud(save);
#endif
    }
    #endregion
}

[System.Serializable]
public class PlayerData
{
    public int gems = 0;
    public int highScore = 0;
    public int retries = 0;
    public List<string> unlockedBallIds = new List<string>();
    public string currentBallId = "";
    public string nextRewardDateTime = "";
    public bool isSoundEnabled = true;
    public bool isVibrateEnabled = true;

    public PlayerData()
    {
        gems = 0;
        highScore = 0;
        retries = 0;
        unlockedBallIds = new List<string>();
        unlockedBallIds.Add("gem0");
        currentBallId = "gem0";
        nextRewardDateTime = JsonUtility.ToJson((JsonDateTime)DateTime.UtcNow);
        isSoundEnabled = true;
        isVibrateEnabled = true;
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