using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource clickAS;
    [SerializeField] private AudioSource gemAS;
    [SerializeField] private AudioSource gameOverAS;

    private void Awake()
    {
        PlayerController.OnBallSwitchDirection += OnBallSwitchDirection;
        PlayerController.OnBallGemTouched += OnBallGemTouched;
        PlayerController.OnGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        PlayerController.OnBallSwitchDirection -= OnBallSwitchDirection;
        PlayerController.OnBallGemTouched -= OnBallGemTouched;
        PlayerController.OnGameOver -= OnGameOver;
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