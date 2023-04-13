using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAds : MonoBehaviour
{
    public void ShowBanner()
    {
        BannerAdsManager.Instance.ShowMainMenuBannerAd();
    }
    
    public void HideBanner()
    {
        BannerAdsManager.Instance.HideMainMenuBannerAd();
    }

    public void ShowInt()
    {
        InterstitialAdsManager.Instance.ShowInterstitialAd();
    }

    public void ShowRwd()
    {
        RewardedAdsManager.Instance.ShowAdmobRewardedAd(RewardUser);
    }

    private void RewardUser()
    {
        Debug.LogError("User Rewarded");
    }
}
