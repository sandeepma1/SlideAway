using TMPro;
using UnityEngine;

public class UiStartPanel : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TextMeshProUGUI highScore;

    private void Awake()
    {
        BallController.OnGameStart += GameStart;
    }

    private void Start()
    {
        highScore.text = "High Score: " + ZPlayerPrefs.GetInt(AppData.keyHighScore);
        mainPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        BallController.OnGameStart -= GameStart;
    }

    private void GameStart()
    {
        mainPanel.gameObject.SetActive(false);
    }
}