using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private void Awake()
    {
        GpsManager.OnCloudDataLoaded += OnCloudDataLoaded;
        BallController.OnGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        GpsManager.OnCloudDataLoaded -= OnCloudDataLoaded;
        BallController.OnGameOver -= OnGameOver;
    }

    private void OnCloudDataLoaded(string cloudData)
    {
        if (string.IsNullOrEmpty(cloudData))
        {
            cloudData = "0|0|0";
            SaveGameUserDataOnCloud();
        }
        UpgradeSaveFromLastVersion();
        Hud.SetHudText?.Invoke("cloudData");
        Hud.SetHudText?.Invoke(cloudData);
        print("loaded cloudData " + cloudData);
        string[] data = cloudData.Split('|');
        AppData.gems = int.Parse(data[0]);
        AppData.highScore = int.Parse(data[1]);
        AppData.retries = int.Parse(data[2]);
    }

    private void UpgradeSaveFromLastVersion()
    {
        if (PlayerPrefs.HasKey("BestScore"))
        {
            AppData.highScore = PlayerPrefs.GetInt("BestScore");
            SaveGameUserDataOnCloud();
            PlayerPrefs.DeleteKey("BestScore");
        }
    }

    private void OnGameOver()
    {
        if (AppData.currentScore > AppData.highScore)
        {
            AppData.highScore = AppData.currentScore;
            GpsManager.Instance.PostScoreToLeaderboard(AppData.highScore, GPGSIds.leaderboard_high_score__slide_away);
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
        string save = AppData.gems + "|" + AppData.highScore + "|" + AppData.retries;
        Hud.SetHudText?.Invoke("saving " + save);
        GpsManager.Instance.SaveToCloud(save);
    }
    #endregion
}
