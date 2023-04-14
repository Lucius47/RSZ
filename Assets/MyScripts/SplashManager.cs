using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    private async void Start()
    {
        await AdsManager.InitializeAllManagers();

        // init firebase
        await Task.Delay(1000);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
