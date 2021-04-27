using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameAdManager : Singleton<GameAdManager>
{
    public static Action OnWatchAdClicked;
    [SerializeField] private bool showBanner;
    private string gameIdAndroid = "4094845";
    private string gameIdIos = "4094844";

    protected override void Awake()
    {
        base.Awake();
        OnWatchAdClicked += ShowRewardVideo;
        AdsManager.OnAdStatus += OnAdStatus;
    }

    private void Start()
    {
#if UNITY_IOS
        AdsManager.InitAdManager?.Invoke(gameIdIos, true);
#endif
#if UNITY_ANDROID
        AdsManager.InitAdManager?.Invoke(gameIdAndroid, showBanner);
#endif
    }

    private void OnDestroy()
    {
        AdsManager.OnAdStatus -= OnAdStatus;
        OnWatchAdClicked -= ShowRewardVideo;
    }

    private void ShowRewardVideo()
    {
        AdsManager.ShowRewardedAd?.Invoke();
    }

    private void OnAdStatus(AdStatus adStatus)
    {
        if (adStatus != AdStatus.IsReady)
        {
            //ads ready
        }
        switch (adStatus)
        {
            case AdStatus.Finished:
                StartCoroutine(Add50Gems());
                break;
            case AdStatus.Skipped:
                break;
            case AdStatus.Error:
                break;
            case AdStatus.Failed:
                break;
            case AdStatus.Started:
                break;
            case AdStatus.IsReady:
                break;
            default:
                break;
        }
    }

    private IEnumerator Add50Gems()
    {
        for (int i = 0; i < 50; i++)
        {
            UiGemsSpawnCanvas.OnSpawnGem2d(Vector2.zero);
            yield return new WaitForEndOfFrame();
        }
    }
}
