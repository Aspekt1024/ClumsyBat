using UnityEngine;
using System.Collections.Generic;

public class StatsHandler : MonoBehaviour {

    // Stat keeping (active in-game only)
    public int Score;
    public int MothsEaten;
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
