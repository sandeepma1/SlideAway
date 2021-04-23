using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource clickAS;
    [SerializeField] private AudioSource gemAS;
    [SerializeField] private AudioSource gameOverAS;

    private void Awake()
    {
        BallController.OnBallSwitchDirection += OnBallSwitchDirection;
        BallController.OnBallGemTouched += OnBallGemTouched;
        BallController.OnGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        BallController.OnBallSwitchDirection -= OnBallSwitchDirection;
        BallController.OnBallGemTouched -= OnBallGemTouched;
        BallController.OnGameOver -= OnGameOver;
    }

    private void OnBallSwitchDirection()
    {
        if (PlayerDataManager.Instance.IsSoundEnabled)
        {
            clickAS.Play();
        }
    }

    private void OnBallGemTouched()
    {
        if (PlayerDataManager.Instance.IsSoundEnabled)
        {
            gemAS.Play();
        }
    }

    private void OnGameOver()
    {
        if (PlayerDataManager.Instance.IsSoundEnabled)
        {
            gameOverAS.Play();
        }
        if (PlayerDataManager.Instance.IsVibrateEnabled)
        {
            Handheld.Vibrate();
        }
    }
}