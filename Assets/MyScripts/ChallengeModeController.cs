using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;

/// <summary>
/// Gameplay controller for challenge mode.
/// </summary>
/// <remarks>Muneeb</remarks>
public class ChallengeModeController : MonoBehaviour
{
    #region Fields

    private bool _isTimerRunning;
    private bool _isGameRunning;

    private float _targetDistance;
    private float _targetTime;
    private float _targetScore;
    private float _targetSpeed;
    
    private int _targetNearMisses;
    private int _currentNearMisses;

    private int _targetWheelies;

    private HR_PlayerHandler _playerHandler;

   [SerializeField] private bool winConditionsMet;
   [SerializeField] private bool failConditionsMet;

    public static UnityAction OnLevelStart;
    public static UnityAction<HR_PlayerHandler> OnLevelWin;
    public static UnityAction<HR_PlayerHandler> OnLevelFail;

    // private int _currentLevel;
    private ChallengeType _challengeType;

    #region Challenge Mode UI

    [Space] [Header("UI")]
    [SerializeField] private TextMeshProUGUI levelObjective;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI timeLeftText;

    #endregion

    [Space] [Header("For Testing")]
    [SerializeField] private bool testMode;
    [ShowIf("testMode")]
    [Range(1, 10)]
    [SerializeField] private int testLevelNumber;
    
    #endregion

    #region Unity Callback Methods

    private void Awake()
    {
    #if !UNITY_EDITOR
            testMode = false;
    #endif
        
        if (testMode)
        {
            GameState.SelectedGameMode = GameState.GameMode.Challenges;
            GameState.CurrentCMLevel = testLevelNumber;
        }

        // _currentLevel = GameState.CurrentCMLevel;
    }

    private void OnEnable()
    {
        HR_PlayerHandler.OnPlayerSpawned += HR_PlayerHandler_OnPlayerSpawned;
        HR_PlayerHandler.OnNearMiss += HR_PlayerHandler_OnNearMiss;
    }
    
    private void OnDisable()
    {
        HR_PlayerHandler.OnPlayerSpawned -= HR_PlayerHandler_OnPlayerSpawned;
        HR_PlayerHandler.OnNearMiss -= HR_PlayerHandler_OnNearMiss;
    }
    
    private void HR_PlayerHandler_OnPlayerSpawned (HR_PlayerHandler _playerHandler)
    {
        this._playerHandler = _playerHandler;
        TimeElapsed = 0;
        _isTimerRunning = true;
    }

    private void HR_PlayerHandler_OnNearMiss(HR_PlayerHandler player, int combo, HR_UIDynamicScoreDisplayer.Side side)
    {
        _currentNearMisses++;
    }

    private void Start()
    {
        GameState.SelectedTrafficMode = CurrentLevel.trafficMode;
        
        _challengeType = CurrentLevel.challengeType;
        _isGameRunning = true;
        
        switch (_challengeType)
        {
            case ChallengeType.Distance:
            {
                _targetDistance = CurrentLevel.distance;
                TargetValue = _targetDistance * 1000; // conversion to meters.
                break;
            }
            case ChallengeType.DistanceInTime:
            {
                _targetDistance = CurrentLevel.distance;
                _targetTime = CurrentLevel.timeToCompleteChallenge;
                TargetValue = _targetDistance * 1000; // conversion to meters.
                break;
            }
            case ChallengeType.Time:
            {
                _targetTime = CurrentLevel.time;
                TargetValue = _targetTime;
                break;
            }
            case ChallengeType.Score:
            {
                _targetScore = CurrentLevel.score;
                TargetValue = _targetScore;
                break;
            }
            case ChallengeType.ScoreInTime:
            {
                _targetScore = CurrentLevel.score;
                _targetTime = CurrentLevel.timeToCompleteChallenge;
                TargetValue = _targetScore;
                break;
            }
            case ChallengeType.NearMisses:
            {
                _targetNearMisses = CurrentLevel.nearMisses;
                TargetValue = _targetNearMisses;
                break;
            }
            case ChallengeType.NearMissesInTime:
            {
                _targetNearMisses = CurrentLevel.nearMisses;
                _targetTime = CurrentLevel.timeToCompleteChallenge;
                TargetValue = _targetNearMisses;
                break;
            }
            case ChallengeType.DistanceInOppositeDirection:
            {
                _targetDistance = CurrentLevel.distance;
                TargetValue = _targetDistance * 1000;
                break;
            }
            case ChallengeType.TimeInOppositeDirection:
            {
                _targetTime = CurrentLevel.time;
                TargetValue = _targetTime;
                break;
            }
            case ChallengeType.SustainSpeedForTime:
            {
                _targetTime = CurrentLevel.time;
                _targetSpeed = CurrentLevel.speedToMaintain;
                TargetValue = _targetTime;
                break;
            }
        }

        levelObjective.text = CurrentLevel.levelObjective;
        progressText.text = "0/" + TargetValue.ToString("F0");
        timeLeftText.gameObject.SetActive(CurrentLevel.HasTimeLimit);
        
        OnLevelStart?.Invoke();
    }

    private void Update()
    {
        if (!_isGameRunning) return;
        
        if (_isTimerRunning)
        {
            TimeElapsed += Time.deltaTime;
        }
        
        switch (_challengeType)
        {
            case ChallengeType.Distance:
            {
                if (_playerHandler.Distance > _targetDistance)
                {
                    winConditionsMet = true;
                }
                else
                {
                    CurrentValue = _playerHandler.Distance * 1000; // conversion to meters.
                }
                break;
            }
            case ChallengeType.DistanceInTime:
            {
                if (_playerHandler.Distance > _targetDistance && TimeElapsed <= _targetTime)
                {
                    winConditionsMet = true;
                }
                else
                {
                    CurrentValue = (float)(_playerHandler.Distance * 1000); // conversion to meters.
                }

                if (TimeElapsed > _targetTime)
                {
                    failConditionsMet = true;
                }

                break;
            }
            case ChallengeType.Time:
            {
                if (TimeElapsed > _targetTime)
                {
                    winConditionsMet = true;
                }
                else
                {
                    CurrentValue = TimeElapsed;
                }
                break;
            }
            case ChallengeType.Score:
            {
                if (_playerHandler.Score > _targetScore)
                {
                    winConditionsMet = true;
                }
                else
                {
                    CurrentValue = _playerHandler.Score;
                }

                break;
            }
            case ChallengeType.ScoreInTime:
            {
                if (_playerHandler.Score > _targetScore && TimeElapsed <= _targetTime)
                {
                    winConditionsMet = true;
                }
                else
                {
                    CurrentValue = _playerHandler.Score;
                }

                if (TimeElapsed > _targetTime)
                {
                    failConditionsMet = true;
                }
                
                break;
            }
            case ChallengeType.NearMisses:
            {
                CurrentValue = _currentNearMisses;
                if (_currentNearMisses >= _targetNearMisses)
                {
                    winConditionsMet = true;
                }

                break;
            }
            case ChallengeType.NearMissesInTime:
            {
                CurrentValue = _currentNearMisses;
                if (TimeElapsed > _targetTime)
                {
                    failConditionsMet = true;
                }
                else if (_currentNearMisses >= _targetNearMisses)
                {
                    winConditionsMet = true;
                }

                break;
            }
            case ChallengeType.DistanceInOppositeDirection:
            {
                if (_playerHandler.DistanceInOppositeDirection > _targetDistance)
                {
                    winConditionsMet = true;
                }
                else
                {
                    CurrentValue = _playerHandler.DistanceInOppositeDirection * 1000;
                }
                break;
            }
            case ChallengeType.TimeInOppositeDirection:
            {
                _isTimerRunning = _playerHandler.IsInOppositeSide;
                
                if (TimeElapsed > _targetTime)
                {
                    winConditionsMet = true;
                }
                else
                {
                    CurrentValue = TimeElapsed;
                }
                break;
            }
            case ChallengeType.SustainSpeedForTime:
            {
                _isTimerRunning = _playerHandler.Speed > _targetSpeed;
                
                if (TimeElapsed > _targetTime)
                {
                    winConditionsMet = true;
                }
                else
                {
                    CurrentValue = TimeElapsed;
                }
                break;
            }
            
        }
        
        progressText.text = CurrentValue.ToString("F1") + "/" + TargetValue;
        if (timeLeftText.gameObject.activeInHierarchy)
        {
            timeLeftText.text = (CurrentLevel.timeToCompleteChallenge - TimeElapsed).ToString("F1");
        }

        if (winConditionsMet)
        {
            LevelWin();
        }

        if (failConditionsMet)
        {
            LevelFail();
        }
    }

    #endregion

    #region Private Methods

    private void LevelWin()
    {
        if (GameState.CurrentCMLevel > GameState.LastCMLevelPlayed)
        {
            GameState.LastCMLevelPlayed = GameState.CurrentCMLevel;
        }
        
        OnLevelWin?.Invoke(_playerHandler);
        winConditionsMet = false;
        _isGameRunning = false;
        _playerHandler.GameOver(true);
        StartCoroutine(HR_GamePlayHandler.Instance.OnGameOver(1));
    }

    private void LevelFail()
    {
        OnLevelFail?.Invoke(_playerHandler);
        failConditionsMet = false;
        _isGameRunning = false;
        _playerHandler.GameOver(false);
        StartCoroutine(HR_GamePlayHandler.Instance.OnGameOver(1));
    }
    
    #endregion

    #region Public Properties

    public ChallengeModeLevels.Level CurrentLevel => ChallengeModeLevels.Instance.levels[GameState.CurrentCMLevel - 1];

    public float TimeElapsed
    {
        get;
        private set;
    }

    public float TargetValue
    {
        get;
        private set;
    }
    public float CurrentValue
    {
        get;
        private set;
    }

    #endregion
    
    #region Public Methods
    
    public void HandleNextButton()
    {
        if (GameState.CurrentCMLevel < ChallengeModeLevels.Instance.levels.Length)
        {
            GameState.CurrentCMLevel++;
        }
        HR_GamePlayHandler.Instance.RestartGame();
    }

    // public void HandleGoToModeSelectionButton()
    // {
    //     GameState.HasRecentlyUnlockedTimeTrialMode = true;
    //     UnityEngine.SceneManagement.SceneManager.LoadScene(1); // Load Main Menu Scene.
    // }
    
    #endregion
}