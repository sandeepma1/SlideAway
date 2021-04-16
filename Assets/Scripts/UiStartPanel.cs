using TMPro;
using UnityEngine;
using DG.Tweening;

public class UiStartPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScore;
    private CanvasGroup startPanelCanvasGroup;

    private void Awake()
    {
        BallController.OnGameStart += GameStart;
    }

    private void Start()
    {
        startPanelCanvasGroup = GetComponent<CanvasGroup>();
        startPanelCanvasGroup.alpha = 1;
        highScore.text = "High Score: " + PlayerPrefs.GetInt("highScore");
    }

    private void OnDestroy()
    {
        BallController.OnGameStart -= GameStart;
    }

    private void GameStart()
    {
        startPanelCanvasGroup.DOFade(0, 0.25f);
    }
}