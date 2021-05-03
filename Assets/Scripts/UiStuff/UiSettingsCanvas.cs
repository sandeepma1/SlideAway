using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UiSettingsCanvas : MonoBehaviour
{
    public static Action<bool> IsSoundEnabled;
    public static Action<bool> IsVibrateEnabled;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button closeButtonBG;
    [SerializeField] private Button gpsLoginButton;
    [SerializeField] private Button facebookButton;
    [SerializeField] private Button instagramButton;
    [SerializeField] private RectTransform mainPanelRect;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vibrateToggle;
    private float rectHeight;
    private const float animSpeed = 0.25f;

    private void Awake()
    {
        GpsManager.OnCloudDataLoaded += OnCloudDataLoaded;
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnSettingsButtonPressed += OnSettingsButtonPressed;
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        closeButtonBG.onClick.AddListener(OnCloseButtonClicked);
        gpsLoginButton.onClick.AddListener(OnGpsLoginButton);
        soundToggle.onValueChanged.AddListener(OnSoundToggle);
        vibrateToggle.onValueChanged.AddListener(OnVibrateToggle);

        facebookButton.onClick.AddListener(OnFacebookButtonClicked);
        instagramButton.onClick.AddListener(OnInstagramButtonClicked);
        //twitterButton.onClick.AddListener(OnTwitterButtonButtonClicked);
        StartCoroutine(GetRectHeight());
        gpsLoginButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (!Player.IsPlayerDataNull())
        {
            OnPlayerDataLoaded();
        }
    }

    private void OnDestroy()
    {
        GpsManager.OnCloudDataLoaded -= OnCloudDataLoaded;
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnSettingsButtonPressed -= OnSettingsButtonPressed;
        closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        closeButtonBG.onClick.RemoveListener(OnCloseButtonClicked);
        gpsLoginButton.onClick.RemoveListener(OnGpsLoginButton);
        soundToggle.onValueChanged.RemoveListener(OnSoundToggle);
        vibrateToggle.onValueChanged.RemoveListener(OnVibrateToggle);
        facebookButton.onClick.RemoveListener(OnFacebookButtonClicked);
        instagramButton.onClick.RemoveListener(OnInstagramButtonClicked);
        //twitterButton.onClick.RemoveListener(OnTwitterButtonButtonClicked);
    }

    private void OnFacebookButtonClicked()
    {
        Application.OpenURL("https://www.facebook.com/bronzdev");
        AnalyticsManager.ButtonPressed(GameButtons.Facebook);
    }

    private void OnInstagramButtonClicked()
    {
        Application.OpenURL("https://instagram.com/slideawaygame?igshid=cjast8794i6k");
        AnalyticsManager.ButtonPressed(GameButtons.Instagram);
    }

    #region GPS login
    private void OnCloudDataLoaded(bool isCloudSaveLoaded, string arg2)
    {
        gpsLoginButton.gameObject.SetActive(!isCloudSaveLoaded);
    }

    private void OnGpsLoginButton()
    {
        GpsManager.Instance.GpsSignIn();
        AnalyticsManager.ButtonPressed(GameButtons.GpsLogin);
    }
    #endregion

    private void OnPlayerDataLoaded()
    {
        soundToggle.isOn = !Player.save.isSoundEnabled;
        vibrateToggle.isOn = !Player.save.isVibrateEnabled;
    }

    private void OnSoundToggle(bool isOn)
    {
        AnalyticsManager.ButtonPressed(GameButtons.Sound);
        Player.save.isSoundEnabled = !isOn;
        IsSoundEnabled?.Invoke(!isOn);
    }

    private void OnVibrateToggle(bool isOn)
    {
        AnalyticsManager.ButtonPressed(GameButtons.Vibrate);
        Player.save.isVibrateEnabled = !isOn;
        IsVibrateEnabled?.Invoke(!isOn);
    }

    private IEnumerator GetRectHeight()
    {
        yield return new WaitForEndOfFrame();
        rectHeight = mainPanelRect.rect.height + 300;
        HideSettingsMenu();
        mainPanelRect.gameObject.SetActive(false);
    }

    private void OnCloseButtonClicked()
    {
        HideSettingsMenu();
    }

    private void OnSettingsButtonPressed()
    {
        ShowSettingsMenu();
    }

    private void ShowSettingsMenu()
    {
        UiStartCanvas.OnToggleUiStartPanel?.Invoke(false);
        mainPanelRect.gameObject.SetActive(true);
        mainPanelRect.DOAnchorPosY(0, animSpeed).OnComplete(() => closeButtonBG.gameObject.SetActive(true));
        AnalyticsManager.ScreenVisit(GameScreens.SettingsMenu);
    }

    private void HideSettingsMenu()
    {
        UiStartCanvas.OnToggleUiStartPanel?.Invoke(true);
        closeButtonBG.gameObject.SetActive(false);
        mainPanelRect.DOAnchorPosY(-rectHeight, animSpeed)
            .OnComplete(() => mainPanelRect.gameObject.SetActive(false));
    }
}