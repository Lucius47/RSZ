using System;
using System.Threading.Tasks;
using UnityEngine;
using GoogleMobileAds.Api;

public class InterstitialAdsManager : GenericSingleton<InterstitialAdsManager>
{
    #region Fields

    [SerializeField] private string adUnitId;
    private InterstitialAd interstitialAd;

    private TextDotsAnimation _loadingAdText;
    
    #endregion

    #region Unity Callback Methods
    
    public override void Awake()
    {
        base.Awake();
        _loadingAdText = transform.GetChild(0).GetChild(0).GetComponentInChildren<TextDotsAnimation>();
    }

    public void Initialize()
    {
        LoadInterstitialAd();
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

    #region Initializing Admob and Unity Ads
    

    private void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        
        Debug.Log("Loading the interstitial ad.");

        // if (AdsManager.IsLowOnMemory(MemoryLimitForAdmobAd))
        // {
        //     Debug.Log("Admob interstitial ad not initialized: RAM < " + MemoryLimitForAdmobAd);
        //     return;
        // }
        
        var adRequest = new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();
        
        InterstitialAd.Load(adUnitId, adRequest,
            (ad, error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                RegisterReloadHandler(ad);
                interstitialAd = ad;
            });
    }
    
    #endregion

    private void RegisterReloadHandler(InterstitialAd ad)
    {
        
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad full screen content closed.");
            LoadInterstitialAd();
        };
        
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            LoadInterstitialAd();
        };
    }
    

    #region Public Methods
    
    public async Task ShowInterstitialAd()
    {
        BannerAdsManager.Instance.HideAllWithBackup();
        
        _loadingAdText.message = "Loading Ad";
        _loadingAdText.transform.parent.gameObject.SetActive(true);

        await Task.Delay(1000);

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
            
            await Task.Delay(1000); // 1000
            BannerAdsManager.Instance.RestoreAllAds();
            return;
        }
        
        Debug.LogError("Interstitial ad is not ready yet.");
        LoadInterstitialAd();

        _loadingAdText.message = "Ad not available";
        await Task.Delay(1000);
        _loadingAdText.transform.parent.gameObject.SetActive(false);
        BannerAdsManager.Instance.RestoreAllAds();
        _loadingAdText.message = "Loading Ad";
    }
    
    #endregion
}
