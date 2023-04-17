using System.Threading.Tasks;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdsManager : GenericSingleton<AdsManager>
{
    #region Unity Callback Methods

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(_ => { });
    }

    #endregion

    public static async Task InitializeAllManagers()
    {
        BannerAdsManager.Instance.Initialize();
        await Task.Delay(3000);
        
        InterstitialAdsManager.Instance.Initialize();
        await Task.Delay(3000);
        
        RewardedAdsManager.Instance.Initialize();
        await Task.Delay(3000);

        AdsInitialized = true;
    }

    public static bool AdsInitialized { get; private set; }
    
    public static bool HasInternet => Application.internetReachability != NetworkReachability.NotReachable;
}
