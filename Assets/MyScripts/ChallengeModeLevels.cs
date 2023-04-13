using UnityEngine;
// using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(menuName = "Levels", fileName = "Challenge Mode Levels")]
public class ChallengeModeLevels : ScriptableObject
{
    private static ChallengeModeLevels _instance;
    public static ChallengeModeLevels Instance
    {
        get
        {
            if(_instance == null)
                _instance = Resources.Load("Levels/Challenge Mode Levels") as ChallengeModeLevels;
            return _instance;
        }
		
    }

    [System.Serializable]
    public class Level
    {
        public string levelObjective = "";
        public float levelWinCoins;
        public ChallengeType challengeType;
        public GameState.TrafficMode trafficMode;
        public GameState.Environment environment;

        // [ShowIf(nameof(ShowDistance))]
        public float distance;
        
        // [ShowIf(nameof(ShowTime))]
        public float time;
        
        // [ShowIf(nameof(ShowScore))]
        public float score;

        // [ShowIf(nameof(ShowSpeed))]
        public float speedToMaintain;

        // [ShowIf(nameof(ShowNearMisses))]
        public int nearMisses;

        // [ShowIf(nameof(ShowTimeToComplete))]
        public float timeToCompleteChallenge;

        public bool HasTimeLimit => ShowTimeToComplete();

        
        // The following methods are used to customize the inspector.
        private bool ShowDistance()
        {
            return challengeType == ChallengeType.Distance ||
                   challengeType == ChallengeType.DistanceInTime ||
                   challengeType == ChallengeType.DistanceInOppositeDirection;
        }
        
        private bool ShowTime()
        {
            return challengeType == ChallengeType.Time ||
                   challengeType == ChallengeType.SustainSpeedForTime ||
                   challengeType == ChallengeType.TimeInOppositeDirection;
        }
        private bool ShowScore()
        {
            return challengeType == ChallengeType.Score ||
                   challengeType == ChallengeType.ScoreInTime;
        }
        private bool ShowSpeed()
        {
            return challengeType == ChallengeType.SustainSpeedForTime;
        }

        private bool ShowNearMisses()
        {
            return challengeType == ChallengeType.NearMisses || 
                   challengeType == ChallengeType.NearMissesInTime;
        }

        private bool ShowTimeToComplete()
        {
            return challengeType == ChallengeType.DistanceInTime ||
                   challengeType == ChallengeType.ScoreInTime ||
                   challengeType == ChallengeType.NearMissesInTime;
        }
    }

    public Level[] levels;

}

public enum ChallengeType
{
    Distance,
    DistanceInTime,
    Time,
    Score,
    ScoreInTime,
    NearMisses,
    NearMissesInTime,
    DistanceInOppositeDirection,
    TimeInOppositeDirection,
    SustainSpeedForTime,
}