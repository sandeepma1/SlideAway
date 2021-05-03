using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Collections;

public class UiPlayerDataHud : MonoBehaviour
{
    public static Action OnUpdateGemsValue;
    public static Action OnUpdateScoreValue;
    [SerializeField] private GameObject scoreGo;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI gemsText;

    private void Awake()
    {
        OnUpdateGemsValue += UpdateGems;
        OnUpdateScoreValue += UpdateCurrentScore;
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart += OnGameStart;
        PlayerController.OnGameOver += OnGameOver;
        scoreGo.SetActive(false);
        UpdateGems();
    }

    private void OnDestroy()
    {
        OnUpdateGemsValue -= UpdateGems;
        OnUpdateScoreValue -= UpdateCurrentScore;
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart -= OnGameStart;
        PlayerController.OnGameOver -= OnGameOver;
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
        gemsText.text = Player.GetGems().ToString();
    }
}