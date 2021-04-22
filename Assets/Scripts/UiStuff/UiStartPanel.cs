using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiStartPanel : MonoBehaviour
{
    public static Action<bool> OnToggleUiStartPanel;
    public static Action OnGameStart;
    public static Action OnHelpButtonPressed;
    public static Action OnSettingsButtonPressed;
    public static Action OnLeaderboardButtonPressed;
    public static Action OnAchievementsButtonPressed;
    public static Action OnReviewAppButtonPressed;
    public static Action OnSoundTogglePressed;
    public static Action OnVibrateTogglePressed;
    public static Action OnShopButtonPressed;
    public static Action OnDailyRewardsButtonPressed;
    public static Action OnFacebookButtonPressed;
    public static Action OnInstagramButtonPressed;
    public static Action OnTwitterButtonPressed;

    [SerializeField] private Button tapToStartButton;
    [Space(10)]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private RectTransform topPanelRect;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI retriesText;
    [Space(10)]
    //Left Buttons
    [SerializeField] private RectTransform leftButtonsRect;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button achievementsButton;
    [SerializeField] private Button reviewAppButton;
    [Space(10)]
    //Right Buttons
    [SerializeField] private RectTransform rightButtonsRect;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button dailyRewardsButton;

    private float topHideYPos;
    private float leftHideXPos;
    private float rightHideXPos;
    private const float animSpeed = 0.25f;

    private void Awake()
    {
        OnToggleUiStartPanel += ToggleUiStartPanel;
        GpsManager.OnCloudDataLoaded += OnCloudDataLoaded;
    }

    private void Start()
    {
        StartCoroutine(GetAllPanelSizes());
        UpdateRetriesHighScore();
        tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.AddListener(OnAchievementsButtonClicked);
        shopButton.onClick.AddListener(() => OnShopButtonPressed?.Invoke());
        settingsButton.onClick.AddListener(() => OnSettingsButtonPressed?.Invoke());
        reviewAppButton.onClick.AddListener(() => OnReviewAppButtonPressed?.Invoke());
    }

    private void OnDestroy()
    {
        OnToggleUiStartPanel -= ToggleUiStartPanel;
        GpsManager.OnCloudDataLoaded -= OnCloudDataLoaded;
        tapToStartButton.onClick.RemoveListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.RemoveListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.RemoveListener(OnAchievementsButtonClicked);
        shopButton.onClick.RemoveListener(() => OnShopButtonPressed?.Invoke());
        settingsButton.onClick.RemoveListener(() => OnSettingsButtonPressed?.Invoke());
        reviewAppButton.onClick.RemoveListener(() => OnReviewAppButtonPressed?.Invoke());
    }

    private IEnumerator GetAllPanelSizes()
    {
        yield return new WaitForEndOfFrame();
        topHideYPos = topPanelRect.rect.height + 100;
        leftHideXPos = leftButtonsRect.rect.width + 100;
        rightHideXPos = rightButtonsRect.rect.width + 100;
    }

    private void ToggleUiStartPanel(bool isVisible)
    {
        //mainPanel.SetActive(isVisible);
        if (tapToStartButton != null)
        {

        }
        tapToStartButton.gameObject.SetActive(isVisible);
        if (isVisible)
        {
            mainPanel.SetActive(isVisible);
            topPanelRect.DOAnchorPosY(0, animSpeed);
            leftButtonsRect.DOAnchorPosX(0, animSpeed);
            rightButtonsRect.DOAnchorPosX(0, animSpeed);
        }
        else
        {
            topPanelRect.DOAnchorPosY(topHideYPos, animSpeed);
            leftButtonsRect.DOAnchorPosX(-leftHideXPos, animSpeed);
            rightButtonsRect.DOAnchorPosX(rightHideXPos, animSpeed).OnComplete(() => mainPanel.SetActive(isVisible));
        }
    }

    private void OnCloudDataLoaded(string obj)
    {
        UpdateRetriesHighScore();
    }

    private void UpdateRetriesHighScore()
    {
        retriesText.text = "Sessions: " + AppData.retries.ToString();
        highScoreText.text = "High Score: " + AppData.highScore.ToString();
    }

    private void OnTapToStartButtonClicked()
    {
        ToggleUiStartPanel(false);
        OnGameStart?.Invoke();
        AppData.retries++;
        retriesText.text = AppData.retries.ToString();
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