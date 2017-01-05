using UnityEngine;
using System.Collections.Generic;

public class StatsHandler : MonoBehaviour {

    // Stat keeping (active in-game only)
    public int Score;
    public int MothsEaten;
    public int CollectedCurrency;
    public float Distance = 0;

    // Stat data (persistent)
    public float TotalDistance = 0;
    public float BestDistance = 0;
    public float DashDistance = 0;
    public float DarknessTime = 0;
    public float PlayTime = 0;
    public float IdleTime = 0;
    public float TotalTime = 0;
    public int TotalJumps = 0;
    public int TimesDashed = 0;
    public int ToothDeaths = 0;
    public int RockDeaths = 0;
    public int Deaths = 0;
    public int MostMoths = 0;
    public int TotalMoths = 0;
    public int Highscore = 0;       // Not currently in use
    public int LevelsCompleted = 0;
    public int Currency = 0;
    public int TotalCurrency = 0;
    
    public LevelCompletionDataControl CompletionData;
    public AbilityControl AbilityData;
    public StoryEventControl Story;

    public struct UserSettings
    {
        public bool Music;
        public bool SFX;
        public bool Tooltips;
    }
    public UserSettings Settings;
    
    private List<Pref> PrefList = new List<Pref>();

    private struct Pref
    {
        public string pref;
        public string varType;
    }

    void Awake()
    {
        SetupPrefList();
        SetupPlayerPrefs();
        GetPersistentStats();
        CreateDataObjects();
        LoadUserSettings();
    }

    void Update ()
    {
        // TODO achievements

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

    }

    private void CreateDataObjects()
    {
        GameObject DataObject = new GameObject("DataObjects");
        CompletionData = DataObject.AddComponent<LevelCompletionDataControl>();
        AbilityData = DataObject.AddComponent<AbilityControl>();
        Story = DataObject.AddComponent<StoryEventControl>();
        CompletionData.Load();
        AbilityData.Load();
        Story.Load();
    }

    private void SaveDataObjects()
    {
        CompletionData.Save();
        AbilityData.Save();
        Story.Save();
    }

    private void LoadUserSettings()
    {
        Settings.Music = PlayerPrefs.GetInt("MusicON") == 1 ? true : false;
        Settings.SFX = PlayerPrefs.GetInt("SFXON") == 1 ? true : false;
        Settings.Tooltips = PlayerPrefs.GetInt("TooltipsON") == 1 ? true : false;
    }

    private void SaveUserSettings()
    {
        PlayerPrefs.SetInt("MusicON", Settings.Music ? 1 : 0);
        PlayerPrefs.SetInt("SFXON", Settings.SFX ? 1 : 0);
        PlayerPrefs.SetInt("TooltipsON", Settings.Tooltips ? 1 : 0);
    }

    public void LevelWon(int Level)
    {
        Currency += CollectedCurrency;
        TotalCurrency += CollectedCurrency;
        CollectedCurrency = 0;

        LevelsCompleted++;
        CompletionData.SetCompleted(Level, true, false, false);
        CompletionData.UnlockLevels(Level, true, false, false);
        SaveStats();
    }

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

        SaveDataObjects();
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
        PrefList.Clear();
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
        foreach (Pref pref in PrefList)
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

    private void AddPref(string Pref, string PrefType)
    {
        Pref NewPref = new Pref();
        NewPref.pref = Pref;
        NewPref.varType = PrefType;
        PrefList.Add(NewPref);
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
