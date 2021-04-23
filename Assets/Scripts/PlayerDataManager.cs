using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    public static Action OnPlayerDataLoaded;
    [SerializeField] private PlayerData playerData;
    private DateTime rewardsDateTime;

    protected override void Awake()
    {
        base.Awake();
        BallController.OnGameOver += OnGameOver;
        GpsManager.OnCloudDataLoaded += OnCloudDataLoaded;
#if UNITY_EDITOR

#endif
    }

    private void Start()
    {
#if UNITY_EDITOR
        rewardsDateTime = DateTime.UtcNow;
        OnCloudDataLoaded("");
#endif
    }

    private void OnDestroy()
    {
        BallController.OnGameOver -= OnGameOver;
        GpsManager.OnCloudDataLoaded -= OnCloudDataLoaded;
    }

    private void OnCloudDataLoaded(string cloudData)
    {
        if (string.IsNullOrEmpty(cloudData))
        {
            playerData = new PlayerData();
            SaveGameUserDataOnCloud();
        }
        else
        {
            playerData = JsonUtility.FromJson<PlayerData>(cloudData);
            rewardsDateTime = JsonUtility.FromJson<JsonDateTime>(playerData.nextRewardDateTime);
        }
        UpgradeSaveFromLastVersion();
        OnPlayerDataLoaded?.Invoke();
    }

    private void UpgradeSaveFromLastVersion()
    {
        if (PlayerPrefs.HasKey("BestScore"))
        {
            playerData.highScore = PlayerPrefs.GetInt("BestScore");
            SaveGameUserDataOnCloud();
            PlayerPrefs.DeleteKey("BestScore");
        }
    }


    #region Get Set Player Data
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

    public DateTime GetRewardsDateTime()
    {
        return rewardsDateTime;
    }

    public void SetNextRewardTime()
    {
        // DateTime nextRewardTIme = DateTime.UtcNow.AddHours(AppData.nextRewardInHours);
        DateTime nextRewardTIme = DateTime.UtcNow.AddSeconds(60);
        playerData.nextRewardDateTime = JsonUtility.ToJson((JsonDateTime)nextRewardTIme);
        rewardsDateTime = nextRewardTIme;
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

    public void IncrementRetries()
    {
        playerData.retries++;
    }

    public void SetHighScore(int highScore)
    {
        playerData.highScore = highScore;
    }
    #endregion


    private void OnGameOver()
    {
        Handheld.Vibrate();
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
        if (pause)
        {
            SaveGameUserDataOnCloud();
        }
    }

    private void SaveGameUserDataOnCloud()
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