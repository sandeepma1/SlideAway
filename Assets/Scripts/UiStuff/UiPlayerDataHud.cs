using UnityEngine;
using TMPro;
using System;

public class UiPlayerDataHud : MonoBehaviour
{
    [SerializeField] private GameObject scoreGo;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI gemsText;

    private void Awake()
    {
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart += OnGameStart;
        BallController.OnGameOver += OnGameOver;
        BallController.OnUpdateScore += UpdateCurrentScore;
        UiGemsSpawnCanvas.OnUpdateGems += UpdateGems;
        scoreGo.SetActive(false);
        UpdateGems();
    }

    private void OnDestroy()
    {
        PlayerDataManager.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart -= OnGameStart;
        BallController.OnGameOver -= OnGameOver;
        BallController.OnUpdateScore -= UpdateCurrentScore;
        UiGemsSpawnCanvas.OnUpdateGems -= UpdateGems;
    }

    private void OnPlayerDataLoaded()
    {
        UpdateGems();
    }

    private void OnGameStart()
    {
        UpdateCurrentScore();
        scoreGo.SetActive(true);
    }

    private void OnGameOver()
    {
        scoreGo.SetActive(false);
    }

    private void UpdateCurrentScore()
    {
        currentScore.text = "Score: " + AppData.currentScore;
    }

    private void UpdateGems()
    {
        gemsText.text = PlayerDataManager.Instance.GetGems().ToString();
    }
}