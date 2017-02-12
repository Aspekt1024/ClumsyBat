using UnityEngine;
using System.Collections.Generic;

public class StatsHandler {

    // Stat keeping (active in-game only)
    public int Score;
    public int MothsEaten;
    public int CollectedCurrency;
    public float Distance;

    // Stat data (persistent)
    public float TotalDistance;
    public float BestDistance;
    public float DashDistance;
    public float DarknessTime;
    public float PlayTime;
    public float IdleTime;
    public float TotalTime;
    public int TotalJumps;
    public int TimesDashed;
    public int ToothDeaths;
    public int RockDeaths;
    public int Deaths;
    public int MostMoths;
    public int TotalMoths;
    public int Highscore; // Not currently in use
    public int LevelsCompleted;
    public int Currency;
    public int TotalCurrency;
    

    public struct UserSettings
    {
        public bool Music;
        public bool Sfx;
        public bool Tooltips;
    }
    public UserSettings Settings;
    
    private readonly List<Pref> _prefList = new List<Pref>();

    private struct Pref
    {
        public string pref;
        public string varType;
    }

    public StatsHandler()
    {
        SetupPrefList();
    }

    public void LoadStats()
    {
        SetupPlayerPrefs();
        GetPersistentStats();
        LoadUserSettings();
    }
    
    // TODO achievements
    // TODO Things i've learned - use events, not update to check if we've reached an achievement.
    // 500 Miles
    // Blind as a bat
    // Blind as a bat 2
    // First death
    // 1000 jumps
    // 10000 jumps (etc)
    // Play time!!
    // "Somebody's a bunny"
    // Dash 
    // Exterminator (destroy objects)
    // bash through objects
    // I love lamp
    //Bat - astrophe! : clumsy fell 1000 times
    //Batzilla : clumsy used hypersonic 50 times
    //Seeing - eye - bat : clumsy used echo location 100 times
    //Bat in Black: clumsy completed a level without consuming a single moth

    private void LoadUserSettings()
    {
        Settings.Music = PlayerPrefs.GetInt("MusicON") == 1;
        Settings.Sfx = PlayerPrefs.GetInt("SFXON") == 1;
        Settings.Tooltips = PlayerPrefs.GetInt("TooltipsON") == 1;
    }

    private void SaveUserSettings()
    {
        PlayerPrefs.SetInt("MusicON", Settings.Music ? 1 : 0);
        PlayerPrefs.SetInt("SFXON", Settings.Sfx ? 1 : 0);
        PlayerPrefs.SetInt("TooltipsON", Settings.Tooltips ? 1 : 0);
    }
    
    public void ResetCurrency()
    {
        CollectedCurrency = 0;
        Currency = 0;
        SaveStats();
    }

    /// <summary>
    /// Saves all game data
    /// </summary>
    public void SaveStats()
    {
        PlayerPrefs.SetInt("Highscore", Highscore);
        PlayerPrefs.SetFloat("TotalDistance", TotalDistance);
        PlayerPrefs.SetFloat("BestDistance", BestDistance);
        PlayerPrefs.SetFloat("DashDistance", DashDistance);
        PlayerPrefs.SetFloat("DarknessTime", DarknessTime);
        PlayerPrefs.SetFloat("PlayTime", PlayTime);
        PlayerPrefs.SetFloat("TotalTime", TotalTime);
        PlayerPrefs.SetFloat("IdleTime", IdleTime);
        PlayerPrefs.SetInt("TotalJumps", TotalJumps);
        PlayerPrefs.SetInt("TimesDashed", TimesDashed);
        PlayerPrefs.SetInt("ToothDeaths", ToothDeaths);
        PlayerPrefs.SetInt("RockDeaths", RockDeaths);
        PlayerPrefs.SetInt("Deaths", Deaths);
        PlayerPrefs.SetInt("MostMoths", MostMoths);
        PlayerPrefs.SetInt("TotalMoths", TotalMoths);
        PlayerPrefs.SetInt("LevelsCompleted", LevelsCompleted);
        PlayerPrefs.SetInt("Currency", Currency);
        PlayerPrefs.SetInt("TotalCurrency", TotalCurrency);
        
        SaveUserSettings();
        PlayerPrefs.Save();
    }

    private void GetPersistentStats()
    {
        Highscore = PlayerPrefs.GetInt("Highscore");
        TotalDistance = PlayerPrefs.GetFloat("TotalDistance");
        BestDistance = PlayerPrefs.GetFloat("BestDistance");
        DashDistance = PlayerPrefs.GetFloat("DashDistance");
        DarknessTime = PlayerPrefs.GetFloat("DarknessTime");
        PlayTime = PlayerPrefs.GetFloat("PlayTime");
        IdleTime = PlayerPrefs.GetFloat("IdleTime");
        TotalTime = PlayerPrefs.GetFloat("TotalTime");
        TotalJumps = PlayerPrefs.GetInt("TotalJumps");
        TimesDashed = PlayerPrefs.GetInt("TimesDashed");
        ToothDeaths = PlayerPrefs.GetInt("ToothDeaths");
        RockDeaths = PlayerPrefs.GetInt("RockDeaths");
        Deaths = PlayerPrefs.GetInt("Deaths");
        MostMoths = PlayerPrefs.GetInt("MostMoths");
        TotalMoths = PlayerPrefs.GetInt("TotalMoths");
        LevelsCompleted = PlayerPrefs.GetInt("LevelsCompleted");
        Currency = PlayerPrefs.GetInt("Currency");
        TotalCurrency = PlayerPrefs.GetInt("TotalCurrency");
    }

    private void SetupPrefList()
    {
        _prefList.Clear();
        AddPref("Highscore", "Int");
        AddPref("BestDistance", "Float");
        AddPref("TotalDistance", "Float");
        AddPref("DashDistance", "Float");
        AddPref("DarknessTime", "Float");
        AddPref("PlayTime", "Float");
        AddPref("IdleTime", "Float");
        AddPref("TotalTime", "Float");
        AddPref("Deaths", "Int");
        AddPref("MostMoths", "Int");
        AddPref("TotalMoths", "Int");
        AddPref("RockDeaths", "Int");
        AddPref("ToothDeaths", "Int");
        AddPref("TimesDashed", "Int");
        AddPref("TotalJumps", "Int");
        AddPref("LevelsCompleted", "Int");
        AddPref("Currency", "Int");
        AddPref("TotalCurrency", "Int");
    }

    private void SetupPlayerPrefs()
    {
        foreach (Pref pref in _prefList)
        {
            if (!PlayerPrefs.HasKey(pref.pref))
            {
                SetupPref(pref);
            }
        }
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        SetupPlayerPrefs();
        GetPersistentStats();
    }

    private void AddPref(string pref, string prefType)
    {
        Pref newPref = new Pref
        {
            pref = pref,
            varType = prefType
        };
        _prefList.Add(newPref);
    }

    private void SetupPref(Pref pref)
    {
        switch (pref.varType)
        {
            case "Int":
                PlayerPrefs.SetInt(pref.pref, 0);
                break;
            case "Float":
                PlayerPrefs.SetFloat(pref.pref, 0f);
                break;
            case "String":
                PlayerPrefs.SetString(pref.pref, string.Empty);
                break;
        }
    }
}
