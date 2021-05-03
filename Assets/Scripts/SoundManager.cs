using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource clickAS;
    [SerializeField] private AudioSource gemAS;
    [SerializeField] private AudioSource gameOverAS;
    [SerializeField] private bool isSound;
    [SerializeField] private bool isVibrate;

    protected override void Awake()
    {
        base.Awake();
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
        PlayerController.OnBallSwitchDirection += OnBallSwitchDirection;
        PlayerController.OnBallGemTouched += OnBallGemTouched;
        PlayerController.OnGameOver += OnGameOver;
        UiSettingsCanvas.IsSoundEnabled += IsSoundEnabledToggle;
        UiSettingsCanvas.IsVibrateEnabled += IsVibrateEnabledToggle;
    }

    private void OnDestroy()
    {
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        PlayerController.OnBallSwitchDirection -= OnBallSwitchDirection;
        PlayerController.OnBallGemTouched -= OnBallGemTouched;
        PlayerController.OnGameOver -= OnGameOver;
        UiSettingsCanvas.IsSoundEnabled -= IsSoundEnabledToggle;
        UiSettingsCanvas.IsVibrateEnabled -= IsVibrateEnabledToggle;
    }

    private void IsSoundEnabledToggle(bool isSound)
    {
        this.isSound = isSound;
    }

    private void IsVibrateEnabledToggle(bool isVibrate)
    {
        this.isVibrate = isVibrate;
    }

    private void OnPlayerDataLoaded()
    {
        isSound = Player.save.isSoundEnabled;
        isVibrate = Player.save.isVibrateEnabled;
    }

    private void OnBallSwitchDirection()
    {
        if (isSound)
        {
            clickAS.Play();
        }
    }

    private void OnBallGemTouched()
    {
        if (isSound)
        {
            gemAS.Play();
        }
    }

    private void OnGameOver()
    {
        if (isSound)
        {
            gameOverAS.Play();
        }
        if (isVibrate)
        {
            Handheld.Vibrate();
        }
    }
}