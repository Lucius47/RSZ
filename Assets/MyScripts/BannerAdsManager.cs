using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class BannerAdsManager : GenericSingleton<BannerAdsManager>
{
    #region Admob Top Right Banner Ad

    [Header("Admob Top Right Banner Ad")] [SerializeField]
    private string smallBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";

    private BannerView _smallBannerView;

    private bool _isMainMenuBannerAdLoaded;

    #endregion

    #region Unity Callback Methods

    public void Initialize()
    {
        CreateSmallBannerView();

        HideSmallBannerAd();
    }

    #endregion

    #region Private Methods

    #region Initializing

    private void CreateSmallBannerView()
    {
        Debug.Log("Creating small banner view");

        // If we already have a banner, destroy the old one.
        if (_smallBannerView != null)
        {
            DestroySmallBannerAd();
        }

        // Create a 320x50 banner at top of the screen
        _smallBannerView = new BannerView(smallBannerAdUnitId, AdSize.Banner, AdPosition.Top);

        ListenToSmallBannerAdEvents();
    }

    #endregion

    #region Requesting

    private void LoadSmallBannerAd()
    {
        if (!AdsManager.HasInternet)
        {
            _isMainMenuBannerAdLoaded = false;
            Debug.Log("Admob MainMenu Banner Ad failed to load: No Internet");
            return;
        }

        
        if(_smallBannerView == null)
        {
            CreateSmallBannerView();
        }
        
        var adRequest = new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();
        
        Debug.Log("Loading banner ad.");
        _smallBannerView.LoadAd(adRequest);
    }

    private void DestroySmallBannerAd()
    {
        if (_smallBannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            _smallBannerView.Destroy();
            _smallBannerView = null;
        }
    }

    #endregion

    #region Admob Banner Ad Callbacks

    private void ListenToSmallBannerAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _smallBannerView.OnBannerAdLoaded += () =>
        {
            _isMainMenuBannerAdLoaded = true;
            
            Debug.Log("Banner view loaded an ad with response : "
                      + _smallBannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _smallBannerView.OnBannerAdLoadFailed += error =>
        {
            _isMainMenuBannerAdLoaded = false;
            
            Debug.LogError("Banner view failed to load an ad with error : "
                           + error);
        };
        // Raised when the ad is estimated to have earned money.
        _smallBannerView.OnAdPaid += adValue =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
    }

    #endregion

    #endregion

    #region Public Methods

    public void ShowSmallBannerAd()
    {
        if (!_isMainMenuBannerAdLoaded)
        {
            LoadSmallBannerAd();
        }

        // HideMediumRectangleAd();
        _smallBannerView.Show();
        
        _isMainMenuBannerAdShown = true;
    }

    public void HideSmallBannerAd()
    {
        _smallBannerView?.Hide();
        
        _isMainMenuBannerAdShown = false;
    }

    public void HideAllAds()
    {
        _smallBannerView?.Hide();
        // _topRightRectBannerAd?.Hide();
        
        _isMainMenuBannerAdShown = false;
        // _isRectBannerAdShown = false;
    }
    
    
    // Function to hide all ads.
    // Function to show all ads based on if they were previously shown or not.
    // Generic functions.
    // Example usage: Hiding all banners while displaying video ads and then restoring the banners
    // after the video ad is complete.
    private bool _isMainMenuBannerAdShown = false;
    // private bool _isRectBannerAdShown = false;

    private bool _wasMainMenuBannerAdPreviouslyShown;
    // private bool _wasRectBannerAdPreviouslyShown;
    
    public void HideAllWithBackup()
    {
        _wasMainMenuBannerAdPreviouslyShown = _isMainMenuBannerAdShown;
        // _wasRectBannerAdPreviouslyShown = _isRectBannerAdShown;

        HideAllAds();
    }

    public void RestoreAllAds()
    {
        if (_wasMainMenuBannerAdPreviouslyShown) ShowSmallBannerAd();
        // if (_wasRectBannerAdPreviouslyShown) ShowRectBannerAd();
    }

    #endregion
}
