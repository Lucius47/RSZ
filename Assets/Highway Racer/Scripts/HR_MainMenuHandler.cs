using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Management of the main menu events. Creates and spawns vehicles, switches them, enables/disables menus.
/// </summary>
[AddComponentMenu("BoneCracker Games/Highway Racer/Main Menu/HR Main Menu Handler")]
public class HR_MainMenuHandler : MonoBehaviour
{

    #region SINGLETON PATTERN
    private static HR_MainMenuHandler _instance;
    public static HR_MainMenuHandler Instance
    {
        get {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<HR_MainMenuHandler>();
            }

            return _instance;
        }
    }
    #endregion
    
    public Transform carSpawnLocation;

    private GameObject[] _createdCars;
    public RCC_CarControllerV3 currentCar;
    public HR_ModApplier currentApplier;        //  Current mod applier.

    internal int CarIndex = 0;      //	Current car index.

    [Header("UI Menus")]
    public GameObject optionsMenu;
    public GameObject carSelectionMenu;
    public GameObject modsSelectionMenu;
    public GameObject cm_LevelSelectionMenu;
    public GameObject sceneSelectionMenu;

    [Header("UI Loading Section")]
    public GameObject loadingScreen;
    public Slider loadingBar;
    private AsyncOperation _async;

    [Header("Other UIs")]
    public Text currency;
    public GameObject buyCarButton;
    public GameObject selectCarButton;
    public GameObject modCarPanel;

    public Text vehicleNameText;
    public Text bestScoreOneWay;
    public Text bestScoreTwoWay;
    public Text bestScoreTimeLeft;
    public Text bestScoreBomb;

    internal AudioSource MainMenuSoundtrack;

    private void Awake()
    {
        // Setting time scale, volume, unpause, and target frame rate.
        Time.timeScale = 1f;
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.pause = false;
        Application.targetFrameRate = 60;

        //	Creating soundtracks for the main menu.
        if (HR_HighwayRacerProperties.Instance.mainMenuClips != null &&
            HR_HighwayRacerProperties.Instance.mainMenuClips.Length > 0)
        {

            MainMenuSoundtrack = HR_CreateAudioSource.NewAudioSource(gameObject, "Main Menu Soundtrack", 0f, 0f, PlayerPrefs.GetFloat("MusicVolume", .35f), HR_HighwayRacerProperties.Instance.mainMenuClips[UnityEngine.Random.Range(0, HR_HighwayRacerProperties.Instance.mainMenuClips.Length)], true, true, false);
            MainMenuSoundtrack.ignoreListenerPause = true;
        }

        //	If test mode enabled, add 1000000 coins to the balance.
        if (HR_HighwayRacerProperties.Instance._1MMoneyForTesting)
            PlayerPrefs.SetInt("Currency", 1000000);

        //	Getting last selected car index.
        // CarIndex = PlayerPrefs.GetInt("SelectedPlayerCarIndex", 0);
        CarIndex = GameState.SelectedPlayerCarIndex;

        CreateCars();
        SpawnCar();
        
        EnableMenu(carSelectionMenu);
    }

    private void Update()
    {
        //	Displaying currency.
        currency.text = HR_API.GetCurrency().ToString("F0");
        
        //	If loading, set value of the loading slider.
        if (_async is { isDone: false }) // _async != null && !_async.isDone
            loadingBar.value = _async.progress;
    }

    private void CreateCars()
    {
        //	Creating a new array.
        _createdCars = new GameObject[HR_PlayerCars.Instance.cars.Length];

        //	Setting array elements.
        for (int i = 0; i < _createdCars.Length; i++)
        {
            _createdCars[i] = (RCC.SpawnRCC(HR_PlayerCars.Instance.cars[i]
                    .playerCar.GetComponent<RCC_CarControllerV3>(),
                carSpawnLocation.position,
                carSpawnLocation.rotation,
                false,
                false,
                false)).gameObject;
            _createdCars[i].GetComponent<RCC_CarControllerV3>().lowBeamHeadLightsOn = true;
            _createdCars[i].SetActive(false);
        }
    }
    
    private void SpawnCar()
    {
        //	If price of the car is 0, or unlocked, save it as owned car.
        if (HR_PlayerCars.Instance.cars[CarIndex].price <= 0 || HR_PlayerCars.Instance.cars[CarIndex].unlocked)
            HR_API.UnlockVehice(CarIndex);

        //	If current spawned car is owned, enable buy button, disable select button. Do opposite otherwise.
        if (HR_API.OwnedVehicle(CarIndex))
        {
            //  Displaying price null.
            if (buyCarButton.GetComponentInChildren<Text>())
                buyCarButton.GetComponentInChildren<Text>().text = "";

            // Enabling select button, disabling buy button.
            buyCarButton.SetActive(false);
            selectCarButton.SetActive(true);
            modCarPanel.SetActive(true);

        }
        else
        {
            //  Displaying price.
            if (buyCarButton.GetComponentInChildren<Text>())
                buyCarButton.GetComponentInChildren<Text>().text =
                    "BUY FOR\n" + HR_PlayerCars.Instance.cars[CarIndex].price.ToString("F0");

            //  Enabling buy button, disabling select button.
            selectCarButton.SetActive(false);
            buyCarButton.SetActive(true);
            modCarPanel.SetActive(false);
        }

        //	Disabling all cars at once. And then enabling only target car (carIndex). And make sure spawned cars are always at spawn point.
        for (int i = 0; i < _createdCars.Length; i++)
        {
            if (_createdCars[i].activeInHierarchy)
            {
                _createdCars[i].SetActive(false);
                _createdCars[i].transform.position = carSpawnLocation.position;
                _createdCars[i].transform.rotation = carSpawnLocation.rotation;
            }
        }

        //	Enabling only target car (carIndex).
        _createdCars[CarIndex].SetActive(true);

        //	Setting current car.
        currentCar = _createdCars[CarIndex].GetComponent<RCC_CarControllerV3>();
        currentApplier = currentCar.GetComponent<HR_ModApplier>();

        //	Displaying car name text.
        if (vehicleNameText)
            vehicleNameText.text = HR_PlayerCars.Instance.cars[CarIndex].vehicleName;

        HR_ModHandler.Instance.ChooseClass(null);
    }
    
    public void BuyCar()
    {
        // If we own the car, don't consume currency.
        if (HR_API.OwnedVehicle(CarIndex))
        {
            Debug.LogError("Car is already owned!");
            return;
        }

        //	If currency is enough, save it and consume currency. Otherwise display the informer.
        if (HR_API.GetCurrency() >= HR_PlayerCars.Instance.cars[CarIndex].price)
        {
            HR_API.ConsumeCurrency(HR_PlayerCars.Instance.cars[CarIndex].price);

        }
        else
        {
            HR_UIInfoDisplayer.Instance.ShowInfo("Not Enough Coins",
                "You have to earn " +
                (HR_PlayerCars.Instance.cars[CarIndex]
                     .price -
                 HR_API.GetCurrency()).ToString() +
                " more coins to buy this wheel",
                HR_UIInfoDisplayer.InfoType.NotEnoughMoney);
            return;
        }

        //	Saving the car.
        HR_API.UnlockVehice(CarIndex);

        //	And spawning again to check modders of the car.
        SpawnCar();
    }
    
    public void SelectCar()
    {
        // PlayerPrefs.SetInt("SelectedPlayerCarIndex", CarIndex);
        GameState.SelectedPlayerCarIndex = CarIndex;
    }
    
    public void PositiveCarIndex()
    {
        CarIndex++;

        if (CarIndex >= _createdCars.Length)
            CarIndex = 0;

        SpawnCar();
    }
    
    public void NegativeCarIndex()
    {
        CarIndex--;

        if (CarIndex < 0)
            CarIndex = _createdCars.Length - 1;

        SpawnCar();
    }
    
    public void EnableMenu(GameObject activeMenu)
    {
        optionsMenu.SetActive(false);
        carSelectionMenu.SetActive(false);
        modsSelectionMenu.SetActive(false);
        cm_LevelSelectionMenu.SetActive(false);
        sceneSelectionMenu.SetActive(false);
        loadingScreen.SetActive(false);

        activeMenu.SetActive(true);

        if (activeMenu == modsSelectionMenu)
            BestScores();
    }
    
    public void SelectScene(int levelIndex)
    {
        SelectCar();
        EnableMenu(loadingScreen);
        _async = SceneManager.LoadSceneAsync(levelIndex);
    }

    /// <summary>
    /// Selects the mode with int.
    /// </summary>
    /// <param name="_modeIndex" />
    public void SelectMode(int _modeIndex)
    {
        //	Saving the selected mode, and enabling scene selection menu.
        // PlayerPrefs.SetInt("SelectedModeIndex", _modeIndex);
        // GameState.SelectedModeIndex = _modeIndex;
        
        switch (_modeIndex)
        {
            case 0:
                GameState.SelectedGameMode = GameState.GameMode.Endless;
                break;
            // case 1:
            //     mode = Mode.TwoWay;
            //     break;
            case 2:
                GameState.SelectedGameMode = GameState.GameMode.TimeAttack;
                break;
            case 3:
                GameState.SelectedGameMode = GameState.GameMode.Bomb;
                break;
            case 4:
                GameState.SelectedGameMode = GameState.GameMode.Challenges;
                break;
            case 5:
                GameState.SelectedGameMode = GameState.GameMode.Racing;
                break;
        }
        
        EnableMenu(sceneSelectionMenu);
    }

    public void SelectChallengeMode()
    {
        GameState.SelectedGameMode = GameState.GameMode.Challenges;
        EnableMenu(cm_LevelSelectionMenu);
    }

    public void SelectCMLevelAndPlay(int level)
    {
        GameState.CurrentCMLevel = level;
        SelectScene(1);
    }

    public void SelectTrafficMode(int mode)
    {
        GameState.SelectedTrafficMode = (mode == 1) ? 
            GameState.TrafficMode.OneWay : GameState.TrafficMode.TwoWay;
    }

    /// <summary>
    /// Displays best scores of all four modes.
    /// </summary>
    private void BestScores()
    {
        int[] scores = HR_API.GetHighScores();

        bestScoreOneWay.text = "BEST SCORE\n" + scores[0];
        bestScoreTwoWay.text = "BEST SCORE\n" + scores[1];
        bestScoreTimeLeft.text = "BEST SCORE\n" + scores[2];
        bestScoreBomb.text = "BEST SCORE\n" + scores[3];
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}