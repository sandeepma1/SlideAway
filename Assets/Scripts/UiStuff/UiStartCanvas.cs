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

    [SerializeField] private Button tapToStartButton;
    [Space(10)]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private RectTransform topPanelRect;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI retriesText;
    [Space(10)]    //Left Buttons
    [SerializeField] private RectTransform leftButtonsRect;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button achievementsButton;
    [SerializeField] private Button reviewAppButton;
    [Space(10)]    //Right Buttons
    [SerializeField] private RectTransform rightButtonsRect;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button rewardsButton;
    [SerializeField] private TextMeshProUGUI dailyRewardsText;

    private float topHideYPos;
    private float leftHideXPos;
    private float rightHideXPos;
    private const float animSpeed = 0.25f;

    private void Awake()
    {
        OnToggleUiStartPanel += ToggleUiStartPanel;
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
        PlayerDataManager.OnUpdateRewardTimer += OnUpdateRewardTimer;
        PlayerDataManager.OnRewardAvailable += OnRewardAvailable;
    }

    private void Start()
    {
        StartCoroutine(GetAllPanelSizes());
        tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.AddListener(OnAchievementsButtonClicked);
        rewardsButton.onClick.AddListener(OnRewardsButtonPressed);
        shopButton.onClick.AddListener(() => OnShopButtonPressed?.Invoke());
        settingsButton.onClick.AddListener(() => OnSettingsButtonPressed?.Invoke());
        reviewAppButton.onClick.AddListener(OnReviewAppButtonPressed);
        UpdateAllSavedValues();
    }

    private void OnDestroy()
    {
        PlayerDataManager.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        OnToggleUiStartPanel -= ToggleUiStartPanel;
        tapToStartButton.onClick.RemoveListener(OnTapToStartButtonClicked);
        leaderboardButton.onClick.RemoveListener(OnLeaderBoardButtonClicked);
        achievementsButton.onClick.RemoveListener(OnAchievementsButtonClicked);
        rewardsButton.onClick.RemoveListener(OnRewardsButtonPressed);
        shopButton.onClick.RemoveListener(() => OnShopButtonPressed?.Invoke());
        settingsButton.onClick.RemoveListener(() => OnSettingsButtonPressed?.Invoke());
        reviewAppButton.onClick.RemoveListener(OnReviewAppButtonPressed);
        PlayerDataManager.OnUpdateRewardTimer -= OnUpdateRewardTimer;
        PlayerDataManager.OnRewardAvailable -= OnRewardAvailable;
    }

    private void OnPlayerDataLoaded()
    {
        UpdateAllSavedValues();
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


    #region Rewards stuff
    private void OnRewardAvailable()
    {
        rewardsButton.interactable = true;
        dailyRewardsText.text = "Ready";
    }

    private void OnUpdateRewardTimer(string rewardText)
    {
        rewardsButton.interactable = false;
        dailyRewardsText.text = rewardText;
    }

    private void OnRewardsButtonPressed()
    {
        rewardsButton.interactable = false;
        PlayerDataManager.Instance.RewardDateTime = DateTime.UtcNow.AddHours(AppData.nextRewardInHours);
        //PlayerDataManager.Instance.RewardDateTime = DateTime.UtcNow.AddSeconds(60);
        PlayerDataManager.Instance.SaveGameUserDataOnCloud();
        StartCoroutine(RewardPlayer());
    }

    private IEnumerator RewardPlayer()
    {
        for (int i = 0; i < AppData.gemsRewards; i++)
        {
            UiGemsSpawnCanvas.OnSpawnGem2d?.Invoke(topPanelRect.GetRandomPointInRectTransform());
            yield return new WaitForEndOfFrame();
        }
    }

    private void UpdateAllSavedValues()
    {
        if (PlayerDataManager.Instance.IsPlayerDataNull())
        {
            Debug.LogError("PlayerData is null");
            return;
        }
        retriesText.text = "Sessions: " + PlayerDataManager.Instance.GetRetries();
        highScoreText.text = "High Score: " + PlayerDataManager.Instance.GetHighScore();
    }
    #endregion


    private void OnTapToStartButtonClicked()
    {
        ToggleUiStartPanel(false);
        OnGameStart?.Invoke();
        PlayerDataManager.Instance.IncrementRetries();
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