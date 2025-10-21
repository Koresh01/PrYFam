using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

public class InterstitialAdController : MonoBehaviour
{
    [SerializeField] private string _adUnitId = "demo-interstitial-yandex";

    private InterstitialAdLoader _interstitialAdLoader;
    private Interstitial _interstitial;

    private void Awake()
    {
        _interstitialAdLoader = new InterstitialAdLoader();
        _interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
        _interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
    }

    public void ShowInterstitialAd()
    {
        if (_interstitial != null)
        {
            _interstitial.Destroy();
        }

        MobileAds.SetAgeRestrictedUser(true);

        var adRequest = new AdRequestConfiguration.Builder(_adUnitId).Build();
        _interstitialAdLoader.LoadAd(adRequest);
    }

    private void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
    {
        _interstitial = args.Interstitial;

        _interstitial.OnAdShown += HandleAdShown;
        _interstitial.OnAdDismissed += HandleAdDismissed;
        _interstitial.OnAdFailedToShow += HandleAdFailedToShow;

        _interstitial.Show();
    }

    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.LogError($"Interstitial ad failed to load: {args.Message}");
    }

    private void HandleAdShown(object sender, EventArgs args)
    {
        Debug.Log("Interstitial ad shown");
    }

    private void HandleAdDismissed(object sender, EventArgs args)
    {
        _interstitial.Destroy();
        _interstitial = null;
    }

    private void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
    {
        Debug.LogError($"Interstitial ad failed to show: {args.Message}");
        _interstitial.Destroy();
        _interstitial = null;
    }

    private void OnDestroy()
    {
        if (_interstitial != null)
        {
            _interstitial.Destroy();
        }
    }
}