using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// TODO display time in hh:mm:ss

public class StatsUI : MonoBehaviour {

    private List<Stat> Stats = new List<Stat>();

    private const float TxtHeight = 30f;
    private const float DescTxtWidth = 300f;
    private const float ValTxtWidth = 100f;
    private const float TxtPad = 15f;

    private float XScale;
    private float YScale;
    private float YTop;
    private float XLeft;
    
    private MainMenu MS = null;
    private RectTransform IdleTimeTxt = null;
    private RectTransform ScrollView = null;
    private RectTransform ScrollViewContent = null;

    struct Stat
    {
        public string desc;
        public int val;
        public string unit;
    }

	void Awake ()
    {
        MS = GameObject.Find("Scripts").GetComponent<MainMenu>();
    }

    void Update()
    {
        if (IdleTimeTxt)
        {
            IdleTimeTxt.GetComponent<Text>().text = GetValStr((int)MS.Stats.IdleTime, "s");
        }
    }

    public void CreateStatText()
    {
        InitialiseStatsList();

        ScrollView = GetScrollView();
        ScrollViewContent = GetScrollViewContent();
        
        YScale = GameObject.Find("ScrollOverlay").GetComponent<RectTransform>().localScale.y;
        XScale = GameObject.Find("ScrollOverlay").GetComponent<RectTransform>().localScale.x;
        
        SetContentSize();

        YTop = ScrollViewContent.position.y - TxtHeight/2*YScale;
        XLeft = ScrollViewContent.position.x - ((ScrollView.rect.width - DescTxtWidth) / 2 - TxtPad) * XScale;

        for (int i = 0; i < Stats.Count; i++)
        {
            AddTextToCanvas(Stats[i], i);
        }
    }
    void InitialiseStatsList()
    {
        NewStat("Best Distance", ((int)MS.Stats.BestDistance), "m");
        NewStat("Total Distance", ((int)MS.Stats.TotalDistance), "m");
        NewStat("Levels Completed", (MS.Stats.LevelsCompleted), "");
        NewStat("Most Moths Eaten", MS.Stats.MostMoths, "");
        NewStat("Total Moths Eaten", MS.Stats.TotalMoths, "");
        NewStat("Total Deaths", MS.Stats.Deaths, "");
        NewStat("Deaths to Rocks", MS.Stats.RockDeaths, "");
        NewStat("Death to Stalactites", MS.Stats.ToothDeaths, "");
        NewStat("Total times Jumped", MS.Stats.TotalJumps, "");
        NewStat("Times Dash Used", MS.Stats.TimesDashed, "");
        NewStat("Distance Dashed", (int)MS.Stats.DashDistance, "m");
        NewStat("Time in total darkness", (int)MS.Stats.DarknessTime, "s");
        NewStat("Play Time", (int)MS.Stats.PlayTime, "s");
        NewStat("Idle Time", (int)MS.Stats.IdleTime, "s");
    }

    RectTransform CreateTxtObj(string name, string text, float txtWidth)
    {
        GameObject Txt = (GameObject)Instantiate(Resources.Load("TxtPrefab"), ScrollViewContent);
        Txt.name = name;
        Txt.GetComponent<Text>().text = text;

        RectTransform TxtRT = Txt.GetComponent<RectTransform>();

        TxtRT.localScale = Vector3.one;
        TxtRT.sizeDelta = new Vector2(txtWidth, TxtHeight);

        return TxtRT;
    }

    void AddTextToCanvas(Stat Stat, int itemNum)
    {
        RectTransform DescTxt = CreateTxtObj(Stat.desc + " Desc", Stat.desc + ": ", DescTxtWidth);
        RectTransform ValTxt = CreateTxtObj(Stat.desc + " Val", GetValStr(Stat.val, Stat.unit), ValTxtWidth);
        
        if (ValTxt.name == "Idle Time Val")
        {
            IdleTimeTxt = ValTxt;
        }

        float YPos = YTop - itemNum * TxtHeight * YScale;
        float XPosDesc = XLeft;
        float XPosVal = XPosDesc + ((DescTxtWidth + ValTxtWidth)/2 + TxtPad) * XScale;

        DescTxt.position = new Vector3(XPosDesc, YPos, 0);
        ValTxt.position = new Vector3(XPosVal, YPos, 0);
    }

    private string GetValStr(float Val, string Unit)
    {
        string ValStr;
        switch (Unit)
        {
            case "m":
                while (Val / 1000 >= 1)
                {
                    Val /= 1000;
                    Unit = NextUnitMeters(Unit);
                    if (Unit == "Mm") { break; }
                }
                if (Unit == "m")
                {
                    ValStr = Val + Unit;
                }
                else
                {
                    ValStr = Val.ToString("0.00") + Unit;
                }
                break;
            case "s":
                while (Val / 60 > 1)
                {
                    Val /= 60;
                    Unit = NextUnitTime(Unit);
                    if (Unit == "d") {
                        Val *= 60 / 24;
                        break;
                    }
                }
                switch (Unit)
                {
                    case "s":
                        ValStr = Val.ToString() + Unit;
                        break;
                    case "m":
                        ValStr = Val.ToString("0") + ":" + ((Val - Mathf.Floor(Val))*60).ToString("00") + " min";
                        break;
                    case "h":
                        ValStr = Val.ToString("0") + ":" + ((Val - Mathf.Floor(Val)) * 60).ToString("00") + " hrs";
                        break;
                    case "d":
                        ValStr = Val.ToString("0.00") + " days";
                        break;
                    default:
                        ValStr = Val.ToString() + Unit;
                        break;
                }
                break;
            default:
                ValStr = Val.ToString() + Unit;
                break;
        }
        return ValStr;
    }

    string NextUnitTime(string Unit)
    {
        string NewUnit = Unit;
        switch (Unit)
        {
            case "s":
                NewUnit = "m";
                break;
            case "m":
                NewUnit = "h";
                break;
            case "h":
                NewUnit = "d";
                break;
        }
        return NewUnit;
    }

    string NextUnitMeters(string Unit)
    {
        string NewUnit = Unit;
        switch (Unit)
        {
            case "m":
                NewUnit = "km";
                break;
            case "km":
                NewUnit = "Mm";
                break;
        }
        return NewUnit;
    }

    void NewStat(string desc, int value, string unit)
    {
        Stat StatItem;
        StatItem.desc = desc;
        StatItem.val = value;
        StatItem.unit = unit;
        Stats.Add(StatItem);
    }

    RectTransform GetScrollViewContent()
    {
        foreach (RectTransform RT in ScrollView)
        {
            if (RT.name == "StatViewport")
            {
                foreach (RectTransform RT2 in RT.GetComponent<RectTransform>())
                {
                    if (RT2.name == "StatContent")
                    {
                        return RT2;
                    }
                }
            }
        }
        return null;
    }
    RectTransform GetScrollView()
    {
        foreach (RectTransform RT in GetComponent<RectTransform>())
        {
            if (RT.name == "StatScrollView")
            {
                return RT;
            }
        }
        return null;
    }

    void SetContentSize()
    {
        ScrollViewContent.sizeDelta = new Vector2(0f, Stats.Count * TxtHeight);
    }
}
