using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using DG.Tweening;

public class UiGameOverCanvas : MonoBehaviour
{
    public static Action OnRestartLevel;
    [SerializeField] private RectTransform[] panels;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button shareButton;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI highScore;
    private const float hidePosX = -1500;
    private const float animSpeed = 0.25f;
    private Vector3 punchRotate = new Vector3(10, 0, 10);

    private void Awake()
    {
        PlayerController.OnGameOver += GameOver;
        restartButton.onClick.AddListener(RestartLevelButtonClicked);
        watchAdButton.onClick.AddListener(WatchAdButtonClicked);
        shareButton.onClick.AddListener(ShareButtonClicked);
        mainPanel.SetActive(false);
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].anchoredPosition = new Vector2(hidePosX, panels[i].anchoredPosition.y);
        }
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(RestartLevelButtonClicked);
        watchAdButton.onClick.RemoveListener(WatchAdButtonClicked);
        shareButton.onClick.RemoveListener(ShareButtonClicked);
        PlayerController.OnGameOver -= GameOver;
    }

    private void WatchAdButtonClicked()
    {
        GameAdManager.OnWatchAd?.Invoke(AdRewardType.FreeGems, "");
        AnalyticsManager.ButtonPressed(GameButtons.FreeGemsByAd);
    }

    private void ShareButtonClicked()
    {
        ShareManager.OnShareAction?.Invoke();
        AnalyticsManager.ButtonPressed(GameButtons.Share);
        AnalyticsManager.SocialShare(UnityEngine.Analytics.ShareType.Image, UnityEngine.Analytics.SocialNetwork.None);
    }

    private void GameOver()
    {
        score.text = "Score: " + AppData.currentScore.ToString();
        highScore.text = "High Score: " + Player.GetHighScore().ToString();
        mainPanel.SetActive(true);
        StartCoroutine(ShowPanelAnimate());
        InvokeRepeating("HighlightWatchAdButton", 1, 1);
        AnalyticsManager.GameOverCurrentScore();
        AnalyticsManager.ScreenVisit(GameScreens.GameOver);
    }
    //Do not delete used by invoke
    private void HighlightWatchAdButton()
    {
        watchAdButton.transform.DOPunchRotation(punchRotate, 0.5f);
    }

    private IEnumerator ShowPanelAnimate()
    {
        yield return new WaitForSeconds(animSpeed);
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].DOAnchorPosX(0, animSpeed);
            yield return new WaitForSeconds(animSpeed);
        }
    }

    private void RestartLevelButtonClicked()
    {
        mainPanel.SetActive(false);
        OnRestartLevel?.Invoke();
        SceneManager.LoadScene(0);
        AnalyticsManager.ButtonPressed(GameButtons.Restart);
    }
}