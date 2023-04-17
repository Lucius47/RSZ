using System.Threading.Tasks;
using UnityEngine;

public class GameplayAds : MonoBehaviour
{
    private void OnEnable()
    {
        HR_PlayerHandler.OnGameOver += HR_PlayerHandler_OnGameOver;
    }

    private async void HR_PlayerHandler_OnGameOver(HR_PlayerHandler player, int[] scores, bool didWin)
    {
        await Task.Delay(2000);     // wait for game over animations 
        await InterstitialAdsManager.Instance.ShowInterstitialAd();
    }
}
