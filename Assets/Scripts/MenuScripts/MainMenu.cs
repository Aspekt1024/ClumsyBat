using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {
    
    public GameObject MenuButtons;
    private GameObject RuntimeScripts;
    public StatsHandler Stats;

    private MenuScroller Scroller;

    private struct LvButton
    {
        public LevelButton Script;
        public bool bClicked;
    }
    private const int NumLevels = 9;
    private LvButton[] Buttons = new LvButton[NumLevels];

    void Awake()
    {
        RuntimeScripts = new GameObject("Runtime Scripts");
        Stats = RuntimeScripts.AddComponent<StatsHandler>();
        Scroller = RuntimeScripts.AddComponent<MenuScroller>();
    }

    void Start()
    {
        //GetComponent<AudioSource>().Play();
        SetupLevelSelect();
        SetupStatsScreen();
    }

    private void SetupLevelSelect()
    {
        int HighestLevel = 1;
         // TODO force this to only be on the main path once we've decided how many levels will exist on it

        RectTransform LvlButtons = GameObject.Find("LevelButtons").GetComponent<RectTransform>();
        foreach (RectTransform LvlButton in LvlButtons)
        {
            int Level = int.Parse(LvlButton.name.Substring(2, LvlButton.name.Length - 2));
            if (Level == 1)
            {
                Buttons[Level].Script = LvlButton.GetComponent<LevelButton>();
                Buttons[Level].bClicked = false;
            }
            if (Stats.CompletionData.IsUnlocked(Level) || Level == 1)
            {
                if (Level > HighestLevel)
                {
                    HighestLevel = Level;
                }
                LvlButton.GetComponent<Image>().enabled = true;
                LvlButton.GetComponent<Button>().enabled = true;
            }
            else
            {
                LvlButton.GetComponent<Image>().enabled = false;
                LvlButton.GetComponent<Button>().enabled = false;
            }
        }

        // TODO confirm this after we decide how many levels there will be
        if (HighestLevel > 9)
        {
            HighestLevel = 1;
        }
        Scroller.SetCurrentLevel(HighestLevel);
    }

    private void SetupStatsScreen()
    {
        GameObject.Find("StatsPanel").GetComponent<StatsUI>().CreateStatText();
    }

    void Update()
    {
        Stats.IdleTime += Time.deltaTime;
    }

    public void PlayButtonClicked()
    {
        Stats.SaveStats();
        Scroller.LevelSelect();
    }

    public void ReturnToMainScreen()
    {
        Scroller.MainMenu();
    }

    public void QuitButtonClicked()
    {
        Stats.SaveStats();
        Application.Quit();
    }

    public void StatsButtonClicked()
    {
        Stats.SaveStats();
        Scroller.StatsScreen();
    }

    public void ClearDataButtonClicked()
    {
        // TODO setup menu to ask "Are you sure?"
        Stats.ClearPlayerPrefs();
    }

    // Level Selects
    private void LevelClick(int LevelNum)
    {
        Stats.SaveStats();
        bool bLoadLevel = Buttons[LevelNum].Script.Clicked();
        if (bLoadLevel)
        {
            Toolbox.Instance.Level = LevelNum;
            SceneManager.LoadScene("Levels");
        }
    }

    public void LvEndlessButtonClicked() { LevelClick(-1); }
    public void Lv1BtnClick() { LevelClick(1); }
    public void Lv2BtnClick() { LevelClick(2); }
    public void Lv3BtnClick() { LevelClick(3); }
    public void Lv4BtnClick() { LevelClick(4); }
    public void Lv5BtnClick() { LevelClick(5); }
    public void Lv6BtnClick() { LevelClick(6); }
    public void Lv7BtnClick() { LevelClick(7); }
    public void Lv8BtnClick() { LevelClick(8); }
    public void Lv9BtnClick() { LevelClick(9); }
}
