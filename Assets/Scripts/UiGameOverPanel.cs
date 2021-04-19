using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UiGameOverPanel : MonoBehaviour
{
    public static Action OnResumeStart;
    public static Action OnResumed;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI highScore;
    private int Score;

    private void Awake()
    {
        BallController.OnGameOver += GameOver;
        UiStartPanel.OnGameStart += GameStart;
        BallController.OnScoreUpdated += OnScoreUpdated;
    }

    private void Start()
    {
        restartButton.onClick.AddListener(RestartLevelButtonClicked);
        currentScore.gameObject.SetActive(false);
        mainPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(RestartLevelButtonClicked);
        BallController.OnGameOver -= GameOver;
        UiStartPanel.OnGameStart -= GameStart;
        BallController.OnScoreUpdated -= OnScoreUpdated;
    }

    private void OnScoreUpdated(int score)
    {
        Score = score;
        currentScore.text = "Score: " + score;
    }

    #region Continue Panel
    private void GameStart()
    {
        currentScore.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        currentScore.gameObject.SetActive(false);
        score.text = "Score: " + Score.ToString();
        highScore.text = "High Score: " + ZPlayerPrefs.GetInt(AppData.keyHighScore).ToString();
        mainPanel.SetActive(true);
    }

    private void RestartLevelButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}