using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class BannerAdsManager : GenericSingleton<BannerAdsManager>
{
    #region Fields

    #region Admob Top Right Banner Ad

    [Header("Admob Top Right Banner Ad")] [SerializeField]
    private string topRightBannerAdAndroidID;

    private BannerView _topRightMainMenuBannerAd;

    private bool _isMainMenuBannerAdLoaded;

    #endregion
    

    #region Admob Top Right Medium Rectangle Ad

    [Header("Top Right Medium Rectangle Ad")] [SerializeField]
    private string topRightMediumRectangleAdAndroidID;

    private BannerView _topRightRectBannerAd;

    private bool _isMediumRectangleAdLoaded;

    #endregion

    #endregion

    #region Unity Callback Methods

    public void Initialize()
    {
        InitializeAdmobTopRightBannerAd();
        InitializeAdmobTopRightMediumRectangleAd();

        HideMainMenuBannerAd();
        HideMediumRectangleAd();
    }

    #endregion

    #region Private Methods

    #region Initializing

    private void InitializeAdmobTopRightBannerAd()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        string adUnitID = topRightBannerAdAndroidID;
#endif

        _topRightMainMenuBannerAd = new BannerView(adUnitID, AdSize.Banner, AdPosition.TopRight);
        
        _topRightMainMenuBannerAd.OnBannerAdLoaded += HandleMainMenuBannerOnAdLoaded;
        _topRightMainMenuBannerAd.OnBannerAdLoadFailed += HandleMainMenuBannerOnAdFailedToLoad;

        // _topRightMainMenuBannerAd.OnBannerAdLoaded += () => { };

        RequestMainMenuBannerAd();
    }

    private void InitializeAdmobTopRightMediumRectangleAd()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        string adUnitID = topRightMediumRectangleAdAndroidID;
#endif

        _topRightRectBannerAd = new BannerView(adUnitID, AdSize.MediumRectangle, AdPosition.TopRight);

        _topRightRectBannerAd.OnBannerAdLoaded += HandleRectBannerOnAdLoaded;
        _topRightRectBannerAd.OnBannerAdLoadFailed += HandleRectBannerOnAdFailedToLoad;

        RequestRectBannerAd();
    }

    #endregion

    #region Requesting

    private void RequestMainMenuBannerAd()
    {
        if (!AdsManager.HasInternet)
        {
            _isMainMenuBannerAdLoaded = false;
            Debug.Log("Admob MainMenu Banner Ad failed to load: No Internet");
            return;
        }

        AdRequest request = new AdRequest.Builder().Build();
        _topRightMainMenuBannerAd.LoadAd(request);
        Debug.Log("Admob MainMenu Banner Ad requested");
    }
    

    private void RequestRectBannerAd()
    {
        if (!AdsManager.HasInternet)
        {
            _isMediumRectangleAdLoaded = false;
            Debug.Log("Admob Rectangle Ad failed to load: No Internet");
            return;
        }

        AdRequest request = new AdRequest.Builder().Build();
        _topRightRectBannerAd.LoadAd(request);
        Debug.Log("Admob Rectangle Ad requested");
    }

    #endregion

    #region Admob Banner Ad Callbacks

    private void HandleMainMenuBannerOnAdLoaded()
    {
        _isMainMenuBannerAdLoaded = true;
        Debug.Log("Admob MainMenu Banner Ad Loaded");
    }

    private void HandleMainMenuBannerOnAdFailedToLoad(LoadAdError error)
    {
        _isMainMenuBannerAdLoaded = false;
        Debug.LogError("Small Banner view failed to load an ad with error : "
                       + error);
    }

    #endregion

    #region Admob Gameplay Top Right Banner Ad Callbacks

    private void HandleGameplayBannerOnAdLoaded(object sender, EventArgs args)
    {
        IsGameplayBannerAdLoaded = true;
        Debug.Log("Admob Gameplay Banner Ad Loaded");
    }

    private void HandleGameplayBannerOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        IsGameplayBannerAdLoaded = false;
        Debug.Log("Admob Gameplay Banner Ad Failed To Load with message: " + args.LoadAdError.GetMessage());
    }

    #endregion

    #region Admob Medium Rectangle Ad Callbacks

    private void HandleRectBannerOnAdLoaded()
    {
        _isMediumRectangleAdLoaded = true;
        Debug.Log("Admob Rectangle Ad Loaded");
    }

    private void HandleRectBannerOnAdFailedToLoad(LoadAdError error)
    {
        _isMediumRectangleAdLoaded = false;
        Debug.LogError("Medium Rect Banner view failed to load an ad with error : "
                       + error);
    }

    #endregion

    #endregion

    #region Public Methods

    public void ShowMainMenuBannerAd()
    {
        if (!_isMainMenuBannerAdLoaded)
        {
            RequestMainMenuBannerAd();
        }

        HideMediumRectangleAd();
        _topRightMainMenuBannerAd.Show();
        
        _isMainMenuBannerAdShown = true;
    }

    public void HideMainMenuBannerAd()
    {
        _topRightMainMenuBannerAd?.Hide();
        
        _isMainMenuBannerAdShown = false;
    }

    public void ShowRectBannerAd()
    {
        if (!_isMediumRectangleAdLoaded)
        {
            RequestRectBannerAd();
        }

        HideMainMenuBannerAd();
        _topRightRectBannerAd.Show();

        _isRectBannerAdShown = true;
    }

    public void HideMediumRectangleAd()
    {
        _topRightRectBannerAd?.Hide();
        
        _isRectBannerAdShown = false;
    }

    public void HideAllAds()
    {
        _topRightMainMenuBannerAd?.Hide();
        _topRightRectBannerAd?.Hide();
        
        _isMainMenuBannerAdShown = false;
        _isRectBannerAdShown = false;
    }
    
    
    // Function to hide all ads.
    // Function to show all ads based on if they were previously shown or not.
    // Generic functions.
    // Example usage: Hiding all banners while displaying video ads and then restoring the banners
    // after the video ad is complete.
    private bool _isMainMenuBannerAdShown = false;
    private bool _isRectBannerAdShown = false;

    private bool _wasMainMenuBannerAdPreviouslyShown;
    private bool _wasRectBannerAdPreviouslyShown;
    
    public void HideAllWithBackup()
    {
        _wasMainMenuBannerAdPreviouslyShown = _isMainMenuBannerAdShown;
        _wasRectBannerAdPreviouslyShown = _isRectBannerAdShown;

        HideAllAds();
    }

    public void RestoreAllAds()
    {
        if (_wasMainMenuBannerAdPreviouslyShown) ShowMainMenuBannerAd();
        if (_wasRectBannerAdPreviouslyShown) ShowRectBannerAd();
    }

    #endregion

    #region Properties

    public bool IsGameplayBannerAdLoaded { get; private set; }

    #endregion
}
