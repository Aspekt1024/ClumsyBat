using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// TODO display time in hh:mm:ss

public class StatsUI : MonoBehaviour
{
    private StatsHandler _statsData;
    private readonly List<Stat> _stats = new List<Stat>();

    private const float TxtHeight = 30f;
    private const float DescTxtWidth = 300f;
    private const float ValTxtWidth = 100f;
    private const float TxtPad = 15f;

    private float _xScale;
    private float _yScale;
    private float _yTop;
    private float _xLeft;
    
    private RectTransform _idleTimeTxt;
    private RectTransform _scrollView;
    private RectTransform _scrollViewContent;

    private struct Stat
    {
        public string Desc;
        public int Val;
        public string Unit;
    }

	private void Awake ()
    {
        _statsData = GameData.Instance.Data.Stats;
    }

    private void Update()
    {
        if (_idleTimeTxt)
        {
            _idleTimeTxt.GetComponent<Text>().text = GetValStr((int)_statsData.IdleTime, "s");
        }
    }

    public void CreateStatText()
    {
        InitialiseStatsList();

        _scrollView = GetScrollView();
        _scrollViewContent = GetScrollViewContent();

        _yScale = GameObject.Find("GameMenuOverlay").GetComponent<RectTransform>().localScale.y;
        _xScale = GameObject.Find("GameMenuOverlay").GetComponent<RectTransform>().localScale.x;
        
        SetContentSize();

        _yTop = _scrollViewContent.position.y - 2*TxtHeight*_yScale;
        _xLeft = _scrollViewContent.position.x - ((_scrollView.rect.width - DescTxtWidth) / 2 - TxtPad) * _xScale;

        for (int i = 0; i < _stats.Count; i++)
        {
            AddTextToCanvas(_stats[i], i);
        }
    }

    private void InitialiseStatsList()
    {
        //NewStat("Best Distance", ((int)MS.Stats.BestDistance), "m");
        NewStat("Total Distance", (int)_statsData.TotalDistance, "m");
        NewStat("Currency", _statsData.Currency, "");
        NewStat("TotalCurrency", _statsData.TotalCurrency, "");
        NewStat("Levels Completed", _statsData.LevelsCompleted, "");
        NewStat("Most Moths Eaten", _statsData.MostMoths, "");
        NewStat("Total Moths Eaten", _statsData.TotalMoths, "");
        NewStat("Total Deaths", _statsData.Deaths, "");
        NewStat("Deaths to Rocks", _statsData.RockDeaths, "");
        NewStat("Death to Stalactites", _statsData.ToothDeaths, "");
        NewStat("Total times Jumped", _statsData.TotalJumps, "");
        NewStat("Times Dash Used", _statsData.TimesDashed, "");
        NewStat("Distance Dashed", (int)_statsData.DashDistance, "m");
        NewStat("Time in total darkness", (int)_statsData.DarknessTime, "s");
        NewStat("Play Time", (int)_statsData.PlayTime, "s");
        NewStat("Idle Time", (int)_statsData.IdleTime, "s");
    }

    private RectTransform CreateTxtObj(string objName, string text, float txtWidth)
    {
        GameObject txt = (GameObject)Instantiate(Resources.Load("TxtPrefab"), _scrollViewContent);
        txt.name = objName;
        txt.GetComponent<Text>().text = text;

        RectTransform txtRt = txt.GetComponent<RectTransform>();

        txtRt.localScale = Vector3.one;
        txtRt.sizeDelta = new Vector2(txtWidth, TxtHeight);

        return txtRt;
    }

    private void AddTextToCanvas(Stat stat, int itemNum)
    {
        RectTransform descTxt = CreateTxtObj(stat.Desc + " Desc", stat.Desc + ": ", DescTxtWidth);
        RectTransform valTxt = CreateTxtObj(stat.Desc + " Val", GetValStr(stat.Val, stat.Unit), ValTxtWidth);
        
        if (valTxt.name == "Idle Time Val")
        {
            _idleTimeTxt = valTxt;
        }

        float yPos = _yTop - itemNum * TxtHeight * _yScale;
        float xPosDesc = _xLeft;
        float xPosVal = xPosDesc + ((DescTxtWidth + ValTxtWidth)/2 + TxtPad) * _xScale;

        descTxt.position = new Vector3(xPosDesc, yPos, 0);
        valTxt.position = new Vector3(xPosVal, yPos, 0);
    }

    private string GetValStr(float val, string unit)
    {
        string valStr;
        switch (unit)
        {
            case "m":
                while (val / 1000 >= 1)
                {
                    val /= 1000;
                    unit = NextUnitMeters(unit);
                    if (unit == "Mm") { break; }
                }
                if (unit == "m")
                {
                    valStr = val + unit;
                }
                else
                {
                    valStr = val.ToString("0.00") + unit;
                }
                break;
            case "s":
                while (val / 60 > 1)
                {
                    val /= 60;
                    unit = NextUnitTime(unit);
                    if (unit == "d") {
                        val *= 60f / 24;
                        break;
                    }
                }
                switch (unit)
                {
                    case "s":
                        valStr = val + unit;
                        break;
                    case "m":
                        valStr = val.ToString("0") + ":" + ((val - Mathf.Floor(val))*60).ToString("00") + " min";
                        break;
                    case "h":
                        valStr = val.ToString("0") + ":" + ((val - Mathf.Floor(val)) * 60).ToString("00") + " hrs";
                        break;
                    case "d":
                        valStr = val.ToString("0.00") + " days";
                        break;
                    default:
                        valStr = val + unit;
                        break;
                }
                break;
            default:
                valStr = val + unit;
                break;
        }
        return valStr;
    }

    private string NextUnitTime(string unit)
    {
        string newUnit = unit;
        switch (unit)
        {
            case "s":
                newUnit = "m";
                break;
            case "m":
                newUnit = "h";
                break;
            case "h":
                newUnit = "d";
                break;
        }
        return newUnit;
    }

    private string NextUnitMeters(string unit)
    {
        string newUnit = unit;
        switch (unit)
        {
            case "m":
                newUnit = "km";
                break;
            case "km":
                newUnit = "Mm";
                break;
        }
        return newUnit;
    }

    private void NewStat(string desc, int value, string unit)
    {
        Stat statItem;
        statItem.Desc = desc;
        statItem.Val = value;
        statItem.Unit = unit;
        _stats.Add(statItem);
    }

    private RectTransform GetScrollViewContent()
    {
        foreach (RectTransform rt in _scrollView)
        {
            if (rt.name == "StatViewport")
            {
                foreach (RectTransform rt2 in rt.GetComponent<RectTransform>())
                {
                    if (rt2.name == "StatContent")
                    {
                        return rt2;
                    }
                }
            }
        }
        return null;
    }

    private RectTransform GetScrollView()
    {
        foreach (RectTransform rt in GetComponent<RectTransform>())
        {
            if (rt.name == "StatScrollView")
            {
                return rt;
            }
        }
        return null;
    }

    private void SetContentSize()
    {
        _scrollViewContent.sizeDelta = new Vector2(0f, (_stats.Count + 3) * TxtHeight);
    }
}
