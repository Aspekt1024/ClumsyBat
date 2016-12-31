using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {
    
    public GameObject MenuButtons;
    private GameObject RuntimeScripts;
    public StatsHandler Stats;
    public GameObject LoadingOverlay;

    private MenuScroller Scroller;

    private struct LvButton
    {
        public LevelButton Script;
        public bool bClicked;
    }
    private const int NumLevels = 9;
    private LvButton[] Buttons = new LvButton[NumLevels + 1];

    void Awake()
    {
        RuntimeScripts = new GameObject("Runtime Scripts");
        Stats = RuntimeScripts.AddComponent<StatsHandler>();
        Scroller = RuntimeScripts.AddComponent<MenuScroller>();
        LoadingOverlay = GameObject.Find("LoadScreen");
    }

    void Start()
    {
        //GetComponent<AudioSource>().Play();
        GetLevelButtons();
        SetupLevelSelect();
        SetupStatsScreen();
    }

    private void GetLevelButtons()
    {
        RectTransform LvlButtons = GameObject.Find("LevelButtons").GetComponent<RectTransform>();
        foreach (RectTransform LvlButton in LvlButtons)
        {
            int Level = int.Parse(LvlButton.name.Substring(2, LvlButton.name.Length - 2));
            if (Level >= 1 && Level <= NumLevels)   // TODO confirm what to do with index 0
            {
                Buttons[Level].Script = LvlButton.GetComponent<LevelButton>();
                Buttons[Level].bClicked = false;

                // TODO setup button states (images) here
                if (Stats.CompletionData.IsUnlocked(Level) || Level == 1)
                {
                    LvlButton.GetComponent<Image>().enabled = true;
                    LvlButton.GetComponent<Button>().enabled = true;
                    if (Stats.CompletionData.IsCompleted(Level))
                    {
                        Buttons[Level].Script.LevelState = LevelButton.LevelStates.Completed;
                    }
                    else
                    {
                        Buttons[Level].Script.LevelState = LevelButton.LevelStates.Enabled;
                    }
                }
                else
                {
                    LvlButton.GetComponent<Image>().enabled = false;
                    LvlButton.GetComponent<Button>().enabled = false;
                    Buttons[Level].Script.LevelState = LevelButton.LevelStates.Disabled;
                }
            }
        }
    }

    private void SetupLevelSelect()
    {
        int Level = 1;

        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            Level = Toolbox.Instance.Level;
        }
        else
        {
            Level = GetHighestLevel();
        }
        Scroller.SetCurrentLevel(Level);
    }

    private int GetHighestLevel()
    {
        int HighestLevel = 1;

        for (int index = 1; index <= NumLevels; index++)
        {
            if (Buttons[index].Script)
            {
                if (Buttons[index].Script.LevelAvailable())
                {
                    if (index > HighestLevel)
                    {
                        HighestLevel = index;
                    }
                }
            }
        }

        // TODO force this to only be on the main path once we've decided how many levels will exist on it
        // TODO confirm this after we decide how many levels there will be
        if (HighestLevel > NumLevels)
        {
            HighestLevel = 1;
        }

        return HighestLevel;
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
            StartCoroutine("LoadLevel", LevelNum);
        }
        else
        {
            for (int index = 1; index <= NumLevels; index++)
            {
                if (index != LevelNum && Buttons[index].Script != null)
                {
                    Buttons[index].Script.Unclick();
                }
            }
        }
    }

    private IEnumerator LoadLevel(int LevelNum)
    {
        Toolbox.Instance.Level = LevelNum;
        AsyncOperation LevelLoader = SceneManager.LoadSceneAsync("Levels");
        LoadingOverlay.GetComponent<LoadScreen>().ShowLoadScreen();

        while (!LevelLoader.isDone)
        {
            yield return null;
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
