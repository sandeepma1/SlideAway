using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class UiPlayerDataHud : MonoBehaviour
{
    public static Action OnUpdateGemsValue;
    public static Action OnUpdateScoreValue;
    public static Action<RectTransform> OnGemIconPosition;
    [SerializeField] private RectTransform gemImageRect;
    [SerializeField] private Button openBuyGemsButton;
    [SerializeField] private GameObject scoreGo;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI gemsText;
    private Vector3 scale = new Vector3(1.25f, 1.25f, 1.25f);
    private const float animSpeed = 0.15f;

    private void Awake()
    {
        OnUpdateGemsValue += UpdateGems;
        OnUpdateScoreValue += UpdateCurrentScore;
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart += OnGameStart;
        PlayerController.OnGameOver += OnGameOver;
        scoreGo.SetActive(false);
        UpdateGems();
        openBuyGemsButton.onClick.AddListener(OpenBuyGemMenu);
    }

    private void Start()
    {
        OnGemIconPosition?.Invoke(gemImageRect);
    }

    private void OnDestroy()
    {
        OnUpdateGemsValue -= UpdateGems;
        OnUpdateScoreValue -= UpdateCurrentScore;
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart -= OnGameStart;
        PlayerController.OnGameOver -= OnGameOver;
        openBuyGemsButton.onClick.RemoveListener(OpenBuyGemMenu);
    }

    private void OpenBuyGemMenu()
    {
        UiGemsShopCanvas.OnShowBuyGemsMenu?.Invoke();
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
        gemImageRect.transform.DOScale(scale, animSpeed)
                .OnComplete(() => gemImageRect.transform.DOScale(Vector3.one, animSpeed));
    }
}