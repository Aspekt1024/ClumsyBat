using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ClumsyBat.UI.DropdownMenuComponents
{
    public class DropdownStatsMenu : MenuScreenBase
    {
        #pragma warning disable 649
        [SerializeField] private CanvasGroup statsMask;
        [SerializeField] private RectTransform scrollView;
        [SerializeField] private RectTransform scrollViewContent;
        #pragma warning restore 649

        private const float TxtHeight = 30f;
        private const float DescTxtWidth = 300f;
        private const float ValTxtWidth = 100f;
        private const float TxtPad = 15f;
        
        private TextMeshProUGUI idleTimeTxt;
        
        private readonly List<Stat> _stats = new List<Stat>();
        
        private float _xScale = 1f;
        private float _yScale = 1f;
        private float _yTop;
        private float _xLeft;
        
        private struct Stat
        {
            public string Desc;
            public int Val;
            public string Unit;
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
            statsMask.alpha = 1f;
            CreateStats();
        }

        public override void HideScreen()
        {
            base.HideScreen();
            statsMask.alpha = 0f;
        }

        private void CreateStats()
        {
            InitialiseStatsList();

            // TODO figure out what this was trying to do
            _yScale = GameObject.Find("DropdownMenu").GetComponent<RectTransform>().localScale.y;
            _xScale = GameObject.Find("DropdownMenu").GetComponent<RectTransform>().localScale.x;
        
            SetContentSize();

            _yTop = scrollViewContent.position.y - 2*TxtHeight*_yScale;
            _xLeft = scrollViewContent.position.x - (scrollView.rect.width / 2  - DescTxtWidth ) * _xScale;

            for (int i = 0; i < _stats.Count; i++)
            {
                AddTextToCanvas(_stats[i], i);
            }
        }

        private void Update()
        {
            if (idleTimeTxt)
            {
                idleTimeTxt.text = GetValStr((int)GameStatics.Data.Stats.IdleTime, "s");
            }
        }

        private void InitialiseStatsList()
        {
            var stats = GameStatics.Data.Stats;
            NewStat("Moths Collected", stats.TotalMoths, "");
            NewStat("Distance Travelled", (int) stats.TotalDistance, "m");
            NewStat("Levels Completed", stats.LevelsCompleted, "");
            NewStat("Wing Flaps", stats.TotalJumps, "");
            NewStat("Hypersonic Count", stats.HypersonicCount, "");
            NewStat("Dash Count", stats.TimesDashed, "");
            NewStat("Shield Uses", stats.ShieldUses, "");
            NewStat("Distance Dashed", (int)stats.DashDistance, "m");
            NewStat("Stalactite Deaths", stats.ToothDeaths, "");
            NewStat("Spider Deaths", stats.SpiderDeaths, "");
            NewStat("Boss Deaths", stats.BossDeaths, "");
            NewStat("Other Deaths", stats.UnknownDeaths, "");
            NewStat("Total Deaths", stats.Deaths, "");
            NewStat("Play Time", (int)stats.PlayTime, "s");
            NewStat("Idle Time", (int)stats.IdleTime, "s");
            NewStat("Time in total darkness", (int)stats.DarknessTime, "s");
        }
        
        private void NewStat(string desc, int value, string unit)
        {
            Stat statItem;
            statItem.Desc = desc;
            statItem.Val = value;
            statItem.Unit = unit;
            _stats.Add(statItem);
        }

        private RectTransform CreateTxtObj(string objName, string text, float txtWidth)
        {
            GameObject txt = (GameObject)Instantiate(Resources.Load("UIElements/TxtPrefab"), scrollViewContent);
            txt.name = objName;
            txt.GetComponent<TextMeshProUGUI>().text = text;

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
                idleTimeTxt = valTxt.GetComponent<TextMeshProUGUI>();
            }

            float yPos = _yTop - itemNum * TxtHeight * _yScale;
            float xPosDesc = _xLeft;
            float xPosVal = xPosDesc + ((DescTxtWidth + ValTxtWidth)/2 + TxtPad) * _xScale;

            descTxt.position = new Vector3(xPosDesc, yPos, 0);
            valTxt.position = new Vector3(xPosVal, yPos, 0);
        }

        private void SetContentSize()
        {
            scrollViewContent.sizeDelta = new Vector2(0f, (_stats.Count + 3) * TxtHeight);
        }
        
        // TODO break out into static function in helper class
        private static string GetValStr(float val, string unit)
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
        
        private static string NextUnitTime(string unit)
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

        private static string NextUnitMeters(string unit)
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
    }
}