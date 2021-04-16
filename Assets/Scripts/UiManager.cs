using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using System.Collections;

public class UiManager : MonoBehaviour
{
    public static Action OnResumeStart;
    public static Action OnResumed;
    [SerializeField] private CanvasGroup titlePanelCanvasGroup;
    [SerializeField] private CanvasGroup gameOverPanelCanvasGroup;
    [SerializeField] private CanvasGroup resumePanelCanvasGroup;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI highScore1; // of the title panel
    [SerializeField] private TextMeshProUGUI highScore2; // of the game over panel

    private void Awake()
    {
        resumePanelCanvasGroup.alpha = 0;
        gameOverPanelCanvasGroup.alpha = 0;
        titlePanelCanvasGroup.alpha = 1;
        restartButton.onClick.AddListener(ResetLevel);
        resumeButton.onClick.AddListener(ResumeLevel);
        BallController.OnGameOver += GameOver;
        BallController.OnGameStart += GameStart;
        BallController.OnScoreUpdated += OnScoreUpdated;
    }

    private void Start()
    {
        highScore1.text = "High Score: " + PlayerPrefs.GetInt("highScore");
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(ResetLevel);
        resumeButton.onClick.RemoveListener(ResumeLevel);
        BallController.OnGameOver -= GameOver;
        BallController.OnGameStart -= GameStart;
        BallController.OnScoreUpdated -= OnScoreUpdated;
    }

    private void ResumeLevel()
    {
        OnResumeStart?.Invoke();
        gameOverPanelCanvasGroup.alpha = 0;
        gameOverPanelCanvasGroup.interactable = false;
        gameOverPanelCanvasGroup.blocksRaycasts = false;
        resumeButton.gameObject.SetActive(false);
        StartCoroutine(StartCountdownAndResume());
    }

    private IEnumerator StartCountdownAndResume()
    {
        resumePanelCanvasGroup.alpha = 1;
        countdownText.text = "3";
        countdownText.transform.DOScale(2, 1);
        yield return new WaitForSeconds(1);

        countdownText.transform.localScale = Vector3.one;
        countdownText.text = "2";
        countdownText.transform.DOScale(2, 1);
        yield return new WaitForSeconds(1);

        countdownText.transform.localScale = Vector3.one;
        countdownText.text = "1";
        countdownText.transform.DOScale(2, 1);
        yield return new WaitForSeconds(1);
        resumePanelCanvasGroup.alpha = 0;
        OnResumed?.Invoke();
    }

    private void OnScoreUpdated(int score)
    {
        currentScore.text = "Score: " + score;
    }

    private void GameStart()
    {
        titlePanelCanvasGroup.DOFade(0, 0.25f);
        currentScore.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        currentScore.gameObject.SetActive(false);
        score.text = "Score: " + PlayerPrefs.GetInt("score").ToString();
        highScore2.text = "High Score: " + PlayerPrefs.GetInt("highScore").ToString();
        gameOverPanelCanvasGroup.alpha = 1;
        gameOverPanelCanvasGroup.interactable = true;
        gameOverPanelCanvasGroup.blocksRaycasts = true;
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(0);
    }
}