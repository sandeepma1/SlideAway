using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiStartPanel : MonoBehaviour
{
    public static Action OnGameStart;
    [SerializeField] private Button tapToStartButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button achievementsButton;
    [SerializeField] private Button achievementsTestButton;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI retriesText;

    private void Awake()
    {
        GpsManager.OnCloudDataLoaded += OnCloudDataLoaded;
        tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.AddListener(OnAchievementsButtonClicked);
        achievementsTestButton.onClick.AddListener(OnAchievementsTestButtonClicked);
    }

    private void Start()
    {
        mainPanel.SetActive(true);
        UpdateRetriesHighScore();
    }

    private void OnDestroy()
    {
        GpsManager.OnCloudDataLoaded -= OnCloudDataLoaded;
        tapToStartButton.onClick.RemoveListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.RemoveListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.RemoveListener(OnAchievementsButtonClicked);
        achievementsTestButton.onClick.RemoveListener(OnAchievementsTestButtonClicked);
    }

    private void OnCloudDataLoaded(string obj)
    {
        UpdateRetriesHighScore();
    }

    public void UpdateRetriesHighScore()
    {
        retriesText.text = "Sessions: " + AppData.retries.ToString();
        highScoreText.text = "High Score: " + AppData.highScore.ToString();
    }

    private void OnTapToStartButtonClicked()
    {
        OnGameStart?.Invoke();
        mainPanel.gameObject.SetActive(false);
        AppData.retries++;
        retriesText.text = AppData.retries.ToString();
    }

    private void OnAchievementsTestButtonClicked()
    {
        GpsManager.Instance.DeleteGameData();
    }

    private void OnAchievementsButtonClicked()
    {
        GpsManager.Instance.ShowAchievementsUI();
    }

    private void OnLeaderBoardButtonClicked()
    {
        GpsManager.Instance.ShowLeaderboardUI();
    }
}