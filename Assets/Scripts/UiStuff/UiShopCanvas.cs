using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiShopCanvas : MonoBehaviour
{
    public static Action<bool> OnIsShopMenuVisible;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button closeButtonBG;
    [SerializeField] private RectTransform mainPanel;
    private float panelHeight;
    private const float animDuration = 0.25f;

    private void Awake()
    {
        UiStartPanel.OnShopButtonPressed += OnShopButtonPressed;
        closeButton.onClick.AddListener(HideShopMenu);
        closeButtonBG.onClick.AddListener(HideShopMenu);
        StartCoroutine(GetMainPanelHeight());
    }

    private void OnDestroy()
    {
        UiStartPanel.OnShopButtonPressed -= OnShopButtonPressed;
        closeButton.onClick.RemoveListener(HideShopMenu);
        closeButtonBG.onClick.RemoveListener(HideShopMenu);
    }

    private IEnumerator GetMainPanelHeight()
    {
        yield return new WaitForEndOfFrame();
        panelHeight = mainPanel.rect.height;
        panelHeight += 20;
        HideShopMenu();
    }

    private void OnShopButtonPressed()
    {
        print("OnShopButtonPressed");
        ShowShopMenu();
    }

    private void ShowShopMenu()
    {
        OnIsShopMenuVisible?.Invoke(true);
        mainPanel.gameObject.SetActive(true);
        mainPanel.DOAnchorPosY(0, animDuration);
        closeButtonBG.gameObject.SetActive(true);
        UiStartPanel.OnToggleUiStartPanel?.Invoke(false);
    }

    private void HideShopMenu()
    {
        OnIsShopMenuVisible?.Invoke(false);
        mainPanel.DOAnchorPosY(-panelHeight, animDuration)
            .OnComplete(() => mainPanel.gameObject.SetActive(false));
        closeButtonBG.gameObject.SetActive(false);
        UiStartPanel.OnToggleUiStartPanel?.Invoke(true);
    }
}