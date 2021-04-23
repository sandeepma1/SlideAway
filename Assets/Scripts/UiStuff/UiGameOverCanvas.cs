using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UiGameOverCanvas : MonoBehaviour
{
    public static Action OnResumeStart;
    public static Action OnResumed;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI highScore;

    private void Awake()
    {
        BallController.OnGameOver += GameOver;
        restartButton.onClick.AddListener(RestartLevelButtonClicked);
        mainPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(RestartLevelButtonClicked);
        BallController.OnGameOver -= GameOver;
    }

    private void GameOver()
    {
        score.text = "Score: " + AppData.currentScore.ToString();
        highScore.text = "High Score: " + PlayerDataManager.Instance.GetHighScore().ToString();
        mainPanel.SetActive(true);
    }

    private void RestartLevelButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
}