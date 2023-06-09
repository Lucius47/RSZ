using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAds : MonoBehaviour
{
    private void Start()
    {
        BannerAdsManager.Instance?.ShowSmallBannerAd();
    }

    public void ShowBanner()
    {
        BannerAdsManager.Instance?.ShowSmallBannerAd();
    }
    
    public void HideBanner()
    {
        BannerAdsManager.Instance?.HideSmallBannerAd();
    }

    public void ShowInt()
    {
        InterstitialAdsManager.Instance?.ShowInterstitialAd();
    }

    public void ShowRwd()
    {
        RewardedAdsManager.Instance?.ShowAdmobRewardedAd(RewardUser);
    }

    private void RewardUser()
    {
        Debug.LogError("User Rewarded");
    }

    private void OnDisable()
    {
        BannerAdsManager.Instance?.HideAllAds();
    }
}
