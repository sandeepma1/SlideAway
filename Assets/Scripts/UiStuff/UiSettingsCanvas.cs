using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UiSettingsCanvas : MonoBehaviour
{
    public static Action<bool> isSoundEnabled;
    public static Action<bool> isVibrateEnabled;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button closeButtonBG;
    [SerializeField] private RectTransform mainPanelRect;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vibrateToggle;
    private float rectHeight;
    private const float animSpeed = 0.25f;

    private void Awake()
    {
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnSettingsButtonPressed += OnSettingsButtonPressed;
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        closeButtonBG.onClick.AddListener(OnCloseButtonClicked);
        soundToggle.onValueChanged.AddListener(OnSoundToggle);
        vibrateToggle.onValueChanged.AddListener(OnVibrateToggle);
        StartCoroutine(GetRectHeight());
    }

    private void Start()
    {
        if (!PlayerDataManager.Instance.IsPlayerDataNull())
        {
            OnPlayerDataLoaded();
        }
    }

    private void OnDestroy()
    {
        PlayerDataManager.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnSettingsButtonPressed -= OnSettingsButtonPressed;
        closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        closeButtonBG.onClick.RemoveListener(OnCloseButtonClicked);
        soundToggle.onValueChanged.RemoveListener(OnSoundToggle);
        vibrateToggle.onValueChanged.RemoveListener(OnVibrateToggle);
    }

    private void OnPlayerDataLoaded()
    {
        soundToggle.isOn = !PlayerDataManager.Instance.IsSoundEnabled;
        vibrateToggle.isOn = !PlayerDataManager.Instance.IsVibrateEnabled;
    }

    private void OnSoundToggle(bool isOn)
    {
        PlayerDataManager.Instance.IsSoundEnabled = !isOn;
    }

    private void OnVibrateToggle(bool isOn)
    {
        PlayerDataManager.Instance.IsVibrateEnabled = !isOn;
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
    }

    private void HideSettingsMenu()
    {
        UiStartCanvas.OnToggleUiStartPanel?.Invoke(true);
        closeButtonBG.gameObject.SetActive(false);
        mainPanelRect.DOAnchorPosY(-rectHeight, animSpeed)
            .OnComplete(() => mainPanelRect.gameObject.SetActive(false));
    }
}