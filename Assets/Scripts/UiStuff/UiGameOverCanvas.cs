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
    [SerializeField] private Button shareAdButton;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI highScore;
    private const float hidePosX = -1500;
    private const float animSpeed = 0.25f;

    private void Awake()
    {
        PlayerController.OnGameOver += GameOver;
        restartButton.onClick.AddListener(RestartLevelButtonClicked);
        watchAdButton.onClick.AddListener(WatchAdButtonClicked);
        shareAdButton.onClick.AddListener(ShareAdButtonClicked);
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
        shareAdButton.onClick.RemoveListener(ShareAdButtonClicked);
        PlayerController.OnGameOver -= GameOver;
    }

    private void WatchAdButtonClicked()
    {
        GameAdManager.OnWatchAd?.Invoke(AdRewardType.FreeGems, "");
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
        StartCoroutine(ShowPanelAnimate());
        InvokeRepeating("HighlightWatchAdButton", 1, 1);
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

    private Vector3 punchRotate = new Vector3(10, 0, 10);
    //Do not delete used by invoke
    private void HighlightWatchAdButton()
    {
        watchAdButton.transform.DOPunchRotation(punchRotate, 0.5f);
    }

    private void RestartLevelButtonClicked()
    {
        mainPanel.SetActive(false);
        OnRestartLevel?.Invoke();
        SceneManager.LoadScene(0);
    }
}