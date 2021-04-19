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
    [SerializeField] private TextMeshProUGUI highScore;

    private void Awake()
    {
        tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.AddListener(OnAchievementsButtonClicked);
        achievementsTestButton.onClick.AddListener(OnAchievementsTestButtonClicked);
        UiStartPanel.OnGameStart += GameStart;
    }

    private void Start()
    {
        highScore.text = "High Score: " + ZPlayerPrefs.GetInt(AppData.keyHighScore);
        mainPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        UiStartPanel.OnGameStart -= GameStart;
        tapToStartButton.onClick.RemoveListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.RemoveListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.RemoveListener(OnAchievementsButtonClicked);
        achievementsTestButton.onClick.RemoveListener(OnAchievementsTestButtonClicked);
    }

    private void OnTapToStartButtonClicked()
    {
        OnGameStart?.Invoke();
    }

    private void OnAchievementsTestButtonClicked()
    {
        Social.ReportProgress("CgkIyqv0lcUPEAIQAA", 100.0f, (bool success) =>
        {
            Hud.SetHudText?.Invoke("Ach ulocked test");
        });
    }

    private void GameStart()
    {
        mainPanel.gameObject.SetActive(false);
    }

    private void OnAchievementsButtonClicked()
    {
        Social.ShowAchievementsUI();
    }

    private void OnLeaderBoardButtonClicked()
    {
        Social.ShowLeaderboardUI();
    }
}