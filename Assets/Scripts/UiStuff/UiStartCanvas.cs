using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiStartCanvas : MonoBehaviour
{
    public static Action<bool> OnToggleUiStartPanel;
    public static Action OnGameStart;
    public static Action OnHelpButtonPressed;
    public static Action OnSettingsButtonPressed;
    public static Action OnLeaderboardButtonPressed;
    public static Action OnAchievementsButtonPressed;
    public static Action OnSoundTogglePressed;
    public static Action OnVibrateTogglePressed;
    public static Action OnShopButtonPressed;
    public static Action OnDailyRewardsButtonPressed;
    public static Action OnFacebookButtonPressed;
    public static Action OnInstagramButtonPressed;
    public static Action OnTwitterButtonPressed;

    [SerializeField] private BoxCollider rewardsBoxCollider;
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
    [SerializeField] private TextMeshProUGUI dailyRewardsText;


    [SerializeField] private bool updateRewardTimer = false;
    [SerializeField] private TimeSpan rewardTimeSpan;
    private float topHideYPos;
    private float leftHideXPos;
    private float rightHideXPos;
    private const float animSpeed = 0.25f;

    private void Awake()
    {
        OnToggleUiStartPanel += ToggleUiStartPanel;
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
    }

    private void Start()
    {
        StartCoroutine(GetAllPanelSizes());
        tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.AddListener(OnAchievementsButtonClicked);
        dailyRewardsButton.onClick.AddListener(OnRewardsButtonPressed);
        shopButton.onClick.AddListener(() => OnShopButtonPressed?.Invoke());
        settingsButton.onClick.AddListener(() => OnSettingsButtonPressed?.Invoke());
        reviewAppButton.onClick.AddListener(OnReviewAppButtonPressed);
        UpdateRewardSystem();
    }

    private void OnDestroy()
    {
        PlayerDataManager.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        OnToggleUiStartPanel -= ToggleUiStartPanel;
        tapToStartButton.onClick.RemoveListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.RemoveListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.RemoveListener(OnAchievementsButtonClicked);
        dailyRewardsButton.onClick.RemoveListener(OnRewardsButtonPressed);
        shopButton.onClick.RemoveListener(() => OnShopButtonPressed?.Invoke());
        settingsButton.onClick.RemoveListener(() => OnSettingsButtonPressed?.Invoke());
        reviewAppButton.onClick.RemoveListener(OnReviewAppButtonPressed);
    }

    private void OnPlayerDataLoaded()
    {
        UpdateRetriesHighScore();
        UpdateRewardSystem();
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

    private void UpdateRetriesHighScore()
    {
        retriesText.text = "Sessions: " + PlayerDataManager.Instance.GetRetries();
        highScoreText.text = "High Score: " + PlayerDataManager.Instance.GetHighScore();
    }


    #region Rewards stuff
    private bool IsRewardReady()
    {
        int yieldTotalSeconds = (int)PlayerDataManager.Instance.GetRewardsDateTime().Subtract(DateTime.UtcNow).TotalSeconds;
        return yieldTotalSeconds <= 0;
    }

    private void UpdateRewardSystem()
    {
        if (IsRewardReady())
        {
            dailyRewardsText.text = "Ready";
            updateRewardTimer = false;
            dailyRewardsButton.interactable = true;
        }
        else
        {
            updateRewardTimer = true;
            dailyRewardsButton.interactable = false;
        }
    }

    private void OnRewardsButtonPressed()
    {
        if (IsRewardReady())
        {
            PlayerDataManager.Instance.SetNextRewardTime();
            updateRewardTimer = true;
            StartCoroutine(RewardPlayer());
        }
    }

    private IEnumerator RewardPlayer()
    {
        for (int i = 0; i < AppData.gemsRewards; i++)
        {
            UiGemsSpawnCanvas.OnSpawnGem2d?.Invoke(topPanelRect.GetRandomPointInRectTransform());
            yield return new WaitForEndOfFrame();
        }
    }

    private void Update()
    {
        if (updateRewardTimer)
        {
            rewardTimeSpan = PlayerDataManager.Instance.GetRewardsDateTime().Subtract(DateTime.UtcNow);
            dailyRewardsText.text = rewardTimeSpan.ToFormattedDuration();
            if (rewardTimeSpan.TotalSeconds <= 0)
            {
                updateRewardTimer = false;
                UpdateRewardSystem();
            }
        }
    }
    #endregion


    private void OnTapToStartButtonClicked()
    {
        ToggleUiStartPanel(false);
        OnGameStart?.Invoke();
        PlayerDataManager.Instance.IncrementRetries();
        retriesText.text = PlayerDataManager.Instance.GetRetries().ToString();
        updateRewardTimer = false;
    }

    private void OnAchievementsButtonClicked()
    {
        GpsManager.Instance.ShowAchievementsUI();
    }

    private void OnLeaderBoardButtonClicked()
    {
        GpsManager.Instance.ShowLeaderboardUI();
    }

    private void OnReviewAppButtonPressed()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.bronz.slideway");
    }
}