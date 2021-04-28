using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UiGameOverCanvas : MonoBehaviour
{
    public static Action OnRestartLevel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button shareAdButton;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI highScore;

    private void Awake()
    {
        PlayerController.OnGameOver += GameOver;
        restartButton.onClick.AddListener(RestartLevelButtonClicked);
        watchAdButton.onClick.AddListener(WatchAdButtonClicked);
        shareAdButton.onClick.AddListener(ShareAdButtonClicked);
        mainPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(RestartLevelButtonClicked);
        watchAdButton.onClick.RemoveListener(WatchAdButtonClicked);
        shareAdButton.onClick.RemoveListener(ShareAdButtonClicked);
        PlayerController.OnGameOver -= GameOver;
    }

    private void WatchAdButtonClicked()
    {
        GameAdManager.OnWatchAdClicked?.Invoke();
    }

    private void ShareAdButtonClicked()
    {
        ShareManager.OnShareAction?.Invoke();
    }

    private void GameOver()
    {
        score.text = "Score: " + AppData.currentScore.ToString();
        highScore.text = "High Score: " + PlayerDataManager.Instance.GetHighScore().ToString();
        mainPanel.SetActive(true);
    }

    private void RestartLevelButtonClicked()
    {
        mainPanel.SetActive(false);
        OnRestartLevel?.Invoke();
        SceneManager.LoadScene(0);
    }
}