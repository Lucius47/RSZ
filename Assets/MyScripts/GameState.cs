using UnityEngine;

public static class GameState
{
    public static bool IsFirstSession
    {
        get => PlayerPrefs.GetInt("IsFirstSession", 1) == 1;
        set => PlayerPrefs.SetInt("IsFirstSession", value ? 1 : 0);
    }
    
    internal static GameMode SelectedGameMode;
    
    public static TrafficMode SelectedTrafficMode = TrafficMode.OneWay;

    public static int SelectedPlayerCarIndex
    {
        get => PlayerPrefs.GetInt("SelectedPlayerCarIndex", 0);
        set => PlayerPrefs.SetInt("SelectedPlayerCarIndex", value);
    }

    public static int CurrentCMLevel;
    
    public static int LastCMLevelPlayed
    {
        get => PlayerPrefs.GetInt("LastCMLevelPlayed", 0);
        set => PlayerPrefs.SetInt("LastCMLevelPlayed", value);
    }

    public enum GameMode { Endless, TimeAttack, Bomb, Challenges, Racing}
    public enum TrafficMode {OneWay, TwoWay}
    public enum DayOrNight { Day, Night }
    
    public enum Environment
    {
        ForestHighway, 
    }
}