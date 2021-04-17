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
    [SerializeField] private GameObject continueMenuPanel;
    [SerializeField] private GameObject resumePanel;
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
    private bool isContinueUsed = false;

    private void Awake()
    {
        BallController.OnGameOver += GameOver;
        BallController.OnGameStart += GameStart;
        BallController.OnScoreUpdated += OnScoreUpdated;
        restartButton.onClick.AddListener(RestartLevelButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        noThanksButton.onClick.AddListener(NoThanksButtonClicked);
        resumePanel.gameObject.SetActive(false);
        continueMenuPanel.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        currentScore.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(RestartLevelButtonClicked);
        continueButton.onClick.RemoveListener(OnContinueButtonClicked);
        noThanksButton.onClick.RemoveListener(NoThanksButtonClicked);
        BallController.OnGameOver -= GameOver;
        BallController.OnGameStart -= GameStart;
        BallController.OnScoreUpdated -= OnScoreUpdated;
    }

    private void OnScoreUpdated(int score)
    {
        Score = score;
        currentScore.text = "Score: " + score;
    }

    #region Resume Panel 3, 2, 1
    private void OnContinueButtonClicked()
    {
        isContinueUsed = true;
        //Show Ads..
        OnResumeStart?.Invoke();
        continueMenuPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        StartCoroutine(StartResumePanel());
    }

    private IEnumerator StartResumePanel()
    {
        resumePanel.gameObject.SetActive(true);
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
        resumePanel.gameObject.SetActive(false);
        OnResumed?.Invoke();
    }
    #endregion


    #region Continue Panel
    private void GameStart()
    {
        currentScore.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        currentScore.gameObject.SetActive(false);
        score.text = "Score: " + Score.ToString();
        highScore.text = "High Score: " + PlayerPrefs.GetInt("highScore").ToString();
        continueMenuPanel.gameObject.SetActive(true);
        if (Score > 1 && !isContinueUsed)
        {
            ShowContinueCountdownMenu();
        }
        else
        {
            ShowRestartMenu();
        }
    }

    private void NoThanksButtonClicked()
    {
        ShowRestartMenu();
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

    private void RestartLevelButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}