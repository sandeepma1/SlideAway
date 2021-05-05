using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDebugStuff : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Button unlockAllButton;
    [SerializeField] private Button addGemsButton;
    [SerializeField] private Button resetSaveButton;
    [SerializeField] private Button showSaveButton;
    [SerializeField] private Toggle showFpsToggle;
    [SerializeField] private Toggle showDebugToggle;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = addGemsButton.GetComponent<RectTransform>();
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        openButton.onClick.AddListener(OnOpenDebugButtonClicked);

        unlockAllButton.onClick.AddListener(OnUnlockAllButtonClicked);
        addGemsButton.onClick.AddListener(OnAddGemsClicked);
        resetSaveButton.onClick.AddListener(OnResetSaveButtonClicked);
        showSaveButton.onClick.AddListener(OnShowSaveButtonClicked);
        showFpsToggle.onValueChanged.AddListener(OnShowFpsToggle);
        showDebugToggle.onValueChanged.AddListener(OnShowDebugToggle);
        OnCloseButtonClicked();
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        openButton.onClick.RemoveListener(OnOpenDebugButtonClicked);

        unlockAllButton.onClick.AddListener(OnUnlockAllButtonClicked);
        addGemsButton.onClick.RemoveListener(OnAddGemsClicked);
        resetSaveButton.onClick.RemoveListener(OnResetSaveButtonClicked);
        showSaveButton.onClick.AddListener(OnShowSaveButtonClicked);
        showFpsToggle.onValueChanged.AddListener(OnShowFpsToggle);
        showDebugToggle.onValueChanged.AddListener(OnShowDebugToggle);
    }

    private void OnUnlockAllButtonClicked()
    {
        foreach (KeyValuePair<string, ShopItem> item in Shop.items)
        {
            Player.AddItemUnlockedId(item.Key);
        }
    }

    private void OnShowFpsToggle(bool showFps)
    {
        Hud.OnShowFpsText?.Invoke(showFps);
    }

    private void OnShowDebugToggle(bool showDebug)
    {
        Hud.OnShowDebugText?.Invoke(showDebug);
    }

    private void OnOpenDebugButtonClicked()
    {
        mainPanel.gameObject.SetActive(true);
    }

    private void OnCloseButtonClicked()
    {
        mainPanel.gameObject.SetActive(false);
    }

    private void OnShowSaveButtonClicked()
    {
        GpsManager.Instance.ShowSelectUI();
    }

    private void OnResetSaveButtonClicked()
    {
        PlayerPrefs.DeleteKey(AppData.oldSaveKey);
        PlayerPrefs.DeleteKey(AppData.localSaveKey);
        PlayerPrefs.DeleteAll();
        GpsManager.Instance.DeleteGameData();
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1);
        Application.Quit();
    }

    private void OnAddGemsClicked()
    {
        Player.IncrementGems(500);
        UiGemsSpawnCanvas.OnSpawnSingleGem2d(rectTransform.anchoredPosition);
    }
}