using System;
using System.Threading.Tasks;
using UnityEngine;
using GoogleMobileAds.Api;

public class RewardedAdsManager : GenericSingleton<RewardedAdsManager>
{
    [SerializeField] private string _adUnitId;
    private RewardedAd rewardedAd;
    
    private TextDotsAnimation _loadingAdText;


    #region Unity Callback Methods

    public override void Awake()
    {
        base.Awake();
        _loadingAdText = transform.GetChild(0).GetChild(0).GetComponentInChildren<TextDotsAnimation>();
    }

    public void Initialize()
    {
        LoadRewardedAd();
    }
    
    private void OnApplicationPause(bool pause)
    {
        try
        {
            if (!pause)
            {
                if (_loadingAdText != null)
                {
                    if (_loadingAdText.transform.parent.gameObject.activeSelf)
                    {
                        _loadingAdText.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
        }
        catch(Exception e) { Debug.Log(e); }
    }

    #endregion

    #region Private Methods

    #region Initializing

    #region Creating and Requesting

    private void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");
        
        var adRequest = new AdRequest.Builder().Build();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (ad, error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
            });
    }

    #endregion

    #region Admob Video Rewarded Ad Callbacks

    private void HandleAdmobVideoRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("Admob Video Rewarded Ad Loaded");
    }

    private void HandleAdmobVideoRewardedAdFailedToLoad(object sender, /*AdErrorEventArgs*/AdFailedToLoadEventArgs args)
    {
        Debug.Log("Admob Video Rewarded Ad failed to load with message: " + args.LoadAdError.GetMessage());
    }

    private void HandleAdmobVideoRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log("Admob Video Rewarded Ad opening...");
    }

    private void HandleAdmobVideoRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        Debug.Log("Admob Video Rewarded Ad failed to show with message: " + args.AdError.GetMessage());
    }

    private void HandleAdmobVideoRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("Admob Video Rewarded Ad closed");
        
        LoadRewardedAd();
    }

    private void HandleAdmobVideoUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;

        Debug.Log("Admob Video Rewarded Ad user rewarded with "
                  + amount + " " + type);
    }

    #endregion

    #endregion
    
    #endregion

    #region Public Methods

    public async Task ShowAdmobRewardedAd(Action callback)
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";
        
        BannerAdsManager.Instance.HideAllWithBackup();

        _loadingAdText.message = "Loading Ad";
        _loadingAdText.transform.parent.gameObject.SetActive(true);
        
        await Task.Delay(100);
        
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show(reward =>
            {
                // TODO: Reward the user.
                callback.Invoke();
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                BannerAdsManager.Instance.RestoreAllAds();
                return;
            });
        }
        
        Debug.Log("Rewarded Ad not loaded yet");

        if (AdsManager.HasInternet)
        {
            LoadRewardedAd();
            await Task.Delay(5000);

            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show(reward =>
                {
                    // TODO: Reward the user.
                    callback.Invoke();
                    Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                    BannerAdsManager.Instance.RestoreAllAds();
                    return;
                });
            }

            Debug.Log("Admob Video Rewarded Ad not shown: Video ad not loaded");
            _loadingAdText.message = "Ad not available";
            await Task.Delay(1000);
            _loadingAdText.transform.parent.gameObject.SetActive(false);
            BannerAdsManager.Instance.RestoreAllAds();
            _loadingAdText.message = "Loading Ad";
            return;
        }

        Debug.Log("Admob Video Rewarded Ad not shown: No internet");
        _loadingAdText.message = "Ad not available";
        await Task.Delay(1000);
        _loadingAdText.transform.parent.gameObject.SetActive(false);
        BannerAdsManager.Instance.RestoreAllAds();
        _loadingAdText.message = "Loading Ad";
    }

    #endregion
}
