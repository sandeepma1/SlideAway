using UnityEngine;
using TMPro;
using System;

public class UiUserDataHud : MonoBehaviour
{
    [SerializeField] private GameObject scoreGo;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI gemsText;

    private void Awake()
    {
        UiStartPanel.OnGameStart += OnGameStart;
        BallController.OnGameOver += OnGameOver;
        BallController.OnUpdateScore += UpdateCurrentScore;
        UiGemsSpawnCanvas.OnUpdateGems += UpdateGems;
        GpsManager.OnCloudDataLoaded += OnCloudDataLoaded;
        scoreGo.SetActive(false);
        UpdateGems();
    }

    private void OnDestroy()
    {
        UiStartPanel.OnGameStart -= OnGameStart;
        BallController.OnGameOver -= OnGameOver;
        BallController.OnUpdateScore -= UpdateCurrentScore;
        UiGemsSpawnCanvas.OnUpdateGems -= UpdateGems;
        GpsManager.OnCloudDataLoaded -= OnCloudDataLoaded;
    }

    private void OnCloudDataLoaded(string obj)
    {
        UpdateGems();
    }

    private void OnGameStart()
    {
        //AppData.currentScore = 0;
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
        gemsText.text = AppData.gems.ToString();
    }
}