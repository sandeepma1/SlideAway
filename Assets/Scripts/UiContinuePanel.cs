using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using System.Collections;

public class UiContinuePanel : MonoBehaviour
{
    public static Action OnResumeStart;
    public static Action OnResumed;
    [SerializeField] private CanvasGroup continueMenuCanvasGroup;
    [SerializeField] private CanvasGroup resumePanelCanvasGroup;
    [SerializeField] private GameObject continuePanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button noThanksButton;
    [SerializeField] private Image continueCountdownRingImage;
    [SerializeField] private TextMeshProUGUI continueCountdownText;
    [SerializeField] private TextMeshProUGUI resumeCountdownText;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI highScore; // of the game over panel
    private const int continueTime = 8;//seconds
    private int Score;

    private void Awake()
    {
        BallController.OnGameOver += GameOver;
        BallController.OnGameStart += GameStart;
        BallController.OnScoreUpdated += OnScoreUpdated;
        restartButton.onClick.AddListener(ResetLevel);
        continueButton.onClick.AddListener(ResumeLevel);
        noThanksButton.onClick.AddListener(NoThanks);
        resumePanelCanvasGroup.alpha = 0;
        continueMenuCanvasGroup.alpha = 0;
        restartButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(ResetLevel);
        continueButton.onClick.RemoveListener(ResumeLevel);
        noThanksButton.onClick.RemoveListener(NoThanks);
        BallController.OnGameOver -= GameOver;
        BallController.OnGameStart -= GameStart;
        BallController.OnScoreUpdated -= OnScoreUpdated;
    }

    private void OnScoreUpdated(int score)
    {
        Score = score;
        currentScore.text = "Score: " + score;
    }

    private void GameStart()
    {
        currentScore.gameObject.SetActive(true);
    }


    #region Resume Panel 3, 2, 1
    private void ResumeLevel()
    {
        OnResumeStart?.Invoke();
        continueMenuCanvasGroup.alpha = 0;
        continueMenuCanvasGroup.interactable = false;
        continueMenuCanvasGroup.blocksRaycasts = false;
        continueButton.gameObject.SetActive(false);
        StartCoroutine(StartResumePanel());
    }

    private IEnumerator StartResumePanel()
    {
        resumePanelCanvasGroup.alpha = 1;
        resumeCountdownText.text = "3";
        resumeCountdownText.transform.DOScale(2, 1);
        yield return new WaitForSeconds(1);

        resumeCountdownText.transform.localScale = Vector3.one;
        resumeCountdownText.text = "2";
        resumeCountdownText.transform.DOScale(2, 1);
        yield return new WaitForSeconds(1);

        resumeCountdownText.transform.localScale = Vector3.one;
        resumeCountdownText.text = "1";
        resumeCountdownText.transform.DOScale(2, 1);
        yield return new WaitForSeconds(1);
        resumePanelCanvasGroup.alpha = 0;
        OnResumed?.Invoke();
    }
    #endregion


    #region Continue Panel
    private void NoThanks()
    {
        ShowRestartMenu();
    }

    private void GameOver()
    {
        currentScore.gameObject.SetActive(false);
        score.text = "Score: " + PlayerPrefs.GetInt("score").ToString();
        highScore.text = "High Score: " + PlayerPrefs.GetInt("highScore").ToString();
        continueMenuCanvasGroup.alpha = 1;
        continueMenuCanvasGroup.interactable = true;
        continueMenuCanvasGroup.blocksRaycasts = true;
        if (Score > 1)
        {
            ShowContinueCountdownMenu();
        }
        else
        {
            ShowRestartMenu();
        }
    }

    private void ShowContinueCountdownMenu()
    {
        noThanksButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        continuePanel.SetActive(true);
        continueCountdownRingImage.DOFillAmount(0, continueTime);
        StartCoroutine(StartContinuePanel());
        Invoke("ShowNoThanksButton", 2);
    }

    private IEnumerator StartContinuePanel()
    {
        for (int i = continueTime; i > 0; i--)
        {
            continueCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        continuePanel.gameObject.SetActive(false);
        ShowRestartMenu();
    }

    //Used in Invoke, don't delete
    private void ShowNoThanksButton()
    {
        noThanksButton.gameObject.SetActive(true);
    }

    private void ShowRestartMenu()
    {
        continuePanel.SetActive(false);
        restartButton.gameObject.SetActive(true);
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}