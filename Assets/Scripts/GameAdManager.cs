using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameAdManager : Singleton<GameAdManager>
{
    public static Action<AdRewardType, string> OnWatchAd;
    public static Action<string> OnAdWatchRewardPlayer;
    public static Action OnAdFailed;
    [SerializeField] private bool showBanner;
    [SerializeField] private RectTransform gemsRandomSpawnRect;
    private string gameIdAndroid = "4094845";
    private string gameIdIos = "4094844";
    private string itemId = "";
    private AdRewardType adRewardType;

    protected override void Awake()
    {
        base.Awake();
        OnWatchAd += ShowRewardVideo;
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
        OnWatchAd -= ShowRewardVideo;
    }

    private void ShowRewardVideo(AdRewardType adRewardType, string itemId)
    {
        this.adRewardType = adRewardType;
        this.itemId = itemId;
        AdsManager.ShowRewardedAd?.Invoke();
    }

    private void OnAdStatus(AdStatus adStatus)
    {
        switch (adStatus)
        {
            case AdStatus.Finished:
                switch (adRewardType)
                {
                    case AdRewardType.FreeGems:
                        StartCoroutine(Add50Gems());
                        break;
                    case AdRewardType.SingleReward:
                        OnAdWatchRewardPlayer?.Invoke(itemId);
                        break;
                    default:
                        break;
                }
                break;
            case AdStatus.Skipped:
            case AdStatus.Error:
            case AdStatus.Failed:
                OnAdFailed?.Invoke();
                Hud.SetHudText("Ad Skipped, Error or Failed");
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

public enum AdRewardType
{
    FreeGems,
    SingleReward
}