using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UiSettingsCanvas : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button closeButtonBG;
    [SerializeField] private RectTransform mainPanelRect;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vibrateToggle;
    private float rectHeight;
    private const float animSpeed = 0.25f;

    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        closeButtonBG.onClick.AddListener(OnCloseButtonClicked);
        soundToggle.onValueChanged.AddListener(OnSoundToggle);
        vibrateToggle.onValueChanged.AddListener(OnVibrateToggle);
        UiStartPanel.OnSettingsButtonPressed += OnSettingsButtonPressed;
        StartCoroutine(GetRectHeight());
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        closeButtonBG.onClick.RemoveListener(OnCloseButtonClicked);
        soundToggle.onValueChanged.RemoveListener(OnSoundToggle);
        vibrateToggle.onValueChanged.RemoveListener(OnVibrateToggle);
        UiStartPanel.OnSettingsButtonPressed -= OnSettingsButtonPressed;
    }

    private void OnSoundToggle(bool isOn)
    {
        print("sound " + !isOn);
    }

    private void OnVibrateToggle(bool isOn)
    {
        print("Vibrate " + !isOn);
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
        UiStartPanel.OnToggleUiStartPanel?.Invoke(false);
        mainPanelRect.gameObject.SetActive(true);
        mainPanelRect.DOAnchorPosY(0, animSpeed).OnComplete(() => closeButtonBG.gameObject.SetActive(true));
    }

    private void HideSettingsMenu()
    {
        UiStartPanel.OnToggleUiStartPanel?.Invoke(true);
        closeButtonBG.gameObject.SetActive(false);
        mainPanelRect.DOAnchorPosY(-rectHeight, animSpeed)
            .OnComplete(() => mainPanelRect.gameObject.SetActive(false));
    }
}