using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Gameplay management. Spawns player vehicle, sets volume, set mods, listens player events.
/// </summary>
[AddComponentMenu("BoneCracker Games/Highway Racer/Gameplay/HR Gameplay Handler")]
public class HR_GamePlayHandler : MonoBehaviour
{
    #region SINGLETON PATTERN

    private static HR_GamePlayHandler _instance;
    public static HR_GamePlayHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HR_GamePlayHandler>();
            }
            return _instance;
        }
    }
    #endregion
    
    public GameState.DayOrNight dayOrNight;
    // internal GameState.GameMode SelectedGameMode;
    public Transform spawnLocation;

    private HR_PlayerHandler player;

    private int selectedCarIndex = 0;
    private int selectedModeIndex = 0;

    internal bool gameStarted = false;
    internal bool paused = false;
    private readonly float minimumSpeed = 20f;

    public delegate void onPaused();
    public static event onPaused OnPaused;

    public delegate void onResumed();
    public static event onResumed OnResumed;

    internal AudioSource gameplaySoundtrack;

    private void Awake()
    {
        //  Make sure time scale is 1. We are setting volume to 0, we'll be increase it smoothly in update method.
        Time.timeScale = 1f;
        AudioListener.volume = 0f;
        AudioListener.pause = false;

        //  Creating soundtrack.
        if (HR_HighwayRacerProperties.Instance.gameplayClips != null &&
            HR_HighwayRacerProperties.Instance.gameplayClips.Length > 0)
        {
            gameplaySoundtrack = HR_CreateAudioSource.NewAudioSource(gameObject, "GamePlay Soundtrack", 0f, 0f, .35f, HR_HighwayRacerProperties.Instance.gameplayClips[UnityEngine.Random.Range(0, HR_HighwayRacerProperties.Instance.gameplayClips.Length)], true, true, false);
            gameplaySoundtrack.volume = PlayerPrefs.GetFloat("MusicVolume", .35f);
            gameplaySoundtrack.ignoreListenerPause = true;
        }

        //  Getting selected player car index and mode index.
        selectedCarIndex = GameState.SelectedPlayerCarIndex;
    }

    private void Start()
    {
        SpawnCar();
        StartCoroutine(WaitForGameStart());     // wait for the countdown.
    }

    private void OnEnable()
    {
        //  Listening events of the player.
        HR_PlayerHandler.OnPlayerSpawned += HR_PlayerHandler_OnPlayerSpawned;
        HR_PlayerHandler.OnNearMiss += HR_PlayerHandler_OnNearMiss;
        HR_PlayerHandler.OnGameOver += HR_PlayerHandler_OnGameOver;
    }
    
    private void HR_PlayerHandler_OnPlayerSpawned(HR_PlayerHandler player)
    {
        gameStarted = false;
        RCC.SetControl(player.GetComponent<RCC_CarControllerV3>(), false);
        StartCoroutine(WaitForGameStart());
    }

    private void HR_PlayerHandler_OnNearMiss(HR_PlayerHandler player, 
        int score, HR_UIDynamicScoreDisplayer.Side side)
    {
        
    }

    private void HR_PlayerHandler_OnGameOver(HR_PlayerHandler player, int[] scores, bool didWin)
    {
        StartCoroutine(OnGameOver(1f));
    }

    /// <summary>
    /// Countdown before the game.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForGameStart()
    {
        yield return new WaitForSeconds(4);

        RCC.SetControl(player.GetComponent<RCC_CarControllerV3>(), true);
        gameStarted = true;
    }

    private void Update()
    {
        //  Adjusting volume smoothly.
        float targetVolume = 1f;

        if (AudioListener.volume < targetVolume && !paused && Time.timeSinceLevelLoad > .5f) {

            if (AudioListener.volume < targetVolume) {

                targetVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
                AudioListener.volume = Mathf.MoveTowards(AudioListener.volume, targetVolume, Time.deltaTime);

            }

        }
    }

    private void SpawnCar()
    {
        player = (RCC.SpawnRCC(HR_PlayerCars.Instance.cars[selectedCarIndex]
                .playerCar.GetComponent<RCC_CarControllerV3>(),
            spawnLocation.position,
            spawnLocation.rotation,
            true,
            false,
            true)).GetComponent<HR_PlayerHandler>();
        player.transform.position = spawnLocation.transform.position;
        player.transform.rotation = Quaternion.identity;

        RCC_Customization.LoadStats(player.GetComponent<RCC_CarControllerV3>());

        player.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, minimumSpeed / 1.75f);

        StartCoroutine(CheckDayTime());
    }
    
    private IEnumerator CheckDayTime()
    {
        yield return new WaitForFixedUpdate();

        if (dayOrNight == GameState.DayOrNight.Night)
            player.GetComponent<RCC_CarControllerV3>().lowBeamHeadLightsOn = true;
        else
            player.GetComponent<RCC_CarControllerV3>().lowBeamHeadLightsOn = false;
    }

    /// <summary>
    /// Pauses the game after the crash and saves the highscore.
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    public IEnumerator OnGameOver(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        OnPaused();

        switch (GameState.SelectedGameMode)
        {
            case GameState.GameMode.Endless:
                PlayerPrefs.SetInt("bestScoreOneWay", (int)player.GetComponent<HR_PlayerHandler>().Score);
                break;
            // case GameMode.TwoWay:
            //     PlayerPrefs.SetInt("bestScoreTwoWay", (int)player.GetComponent<HR_PlayerHandler>().score);
            //     break;
            case GameState.GameMode.TimeAttack:
                PlayerPrefs.SetInt("bestScoreTimeAttack", (int)player.GetComponent<HR_PlayerHandler>().Score);
                break;
            case GameState.GameMode.Bomb:
                PlayerPrefs.SetInt("bestScoreBomb", (int)player.GetComponent<HR_PlayerHandler>().Score);
                break;
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Pause or resume the game.
    /// </summary>
    public void Paused()
    {
        paused = !paused;

        if (paused)
            OnPaused();
        else
            OnResumed();
    }

    private void OnDisable()
    {
        HR_PlayerHandler.OnPlayerSpawned -= HR_PlayerHandler_OnPlayerSpawned;
        HR_PlayerHandler.OnNearMiss -= HR_PlayerHandler_OnNearMiss;
        HR_PlayerHandler.OnGameOver -= HR_PlayerHandler_OnGameOver;
    }
}