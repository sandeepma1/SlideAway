using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDebugStuff : MonoBehaviour
{
    [SerializeField] private Button addGemsButton;
    [SerializeField] private Button resetSaveButton;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = addGemsButton.GetComponent<RectTransform>();
        addGemsButton.onClick.AddListener(OnAddGemsClicked);
        resetSaveButton.onClick.AddListener(OnResetSaveButtonClicked);
    }

    private void OnDestroy()
    {
        addGemsButton.onClick.RemoveListener(OnAddGemsClicked);
        resetSaveButton.onClick.RemoveListener(OnResetSaveButtonClicked);
    }

    private void OnResetSaveButtonClicked()
    {
        PlayerDataManager.Instance.playerData = new PlayerData();
        PlayerDataManager.Instance.SaveGameUserDataOnCloud();
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForEndOfFrame();
        Application.Quit();
    }

    private void OnAddGemsClicked()
    {
        PlayerDataManager.Instance.IncrementGems(100);
        UiGemsSpawnCanvas.OnSpawnGem2d(rectTransform.anchoredPosition);
    }
}
