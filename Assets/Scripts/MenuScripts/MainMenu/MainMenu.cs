using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {
    
    public GameObject MenuButtons;
    public StatsHandler Stats;
    public GameObject LoadingOverlay;

    private GameObject _runtimeScripts;
    private MenuScroller _scroller;
    
    private LevelButton[] BtnScripts = new LevelButton[GameData.NumLevels + 1];

    private void Awake()
    {
        _runtimeScripts = new GameObject("Runtime Scripts");
        Stats = _runtimeScripts.AddComponent<StatsHandler>();
        _scroller = _runtimeScripts.AddComponent<MenuScroller>();
        LoadingOverlay = GameObject.Find("LoadScreen");
    }

    private void Start()
    {
        //GetComponent<AudioSource>().Play();
        GetLevelButtons();
        SetupLevelSelect();
    }
    
    private void Update()
    {
        Stats.IdleTime += Time.deltaTime;
    }

    private void GetLevelButtons()
    {
        RectTransform lvlButtons = GameObject.Find("LevelButtons").GetComponent<RectTransform>();
        foreach (RectTransform lvlButton in lvlButtons)
        {
            int level = int.Parse(lvlButton.name.Substring(2, lvlButton.name.Length - 2));
            if (level >= 1 && level <= GameData.NumLevels)
            {
                BtnScripts[level] = lvlButton.GetComponent<LevelButton>();
                
                if (Stats.LevelData.IsUnlocked(level) || level == 1)
                {
                    BtnScripts[level].SetLevelState(Stats.LevelData.IsCompleted(level)
                        ? LevelButton.LevelStates.Completed
                        : LevelButton.LevelStates.Enabled);
                }
                else
                {
                    BtnScripts[level].SetLevelState(LevelButton.LevelStates.Disabled);
                }
            }
        }
    }

    private void SetupLevelSelect()
    {
        int level;
        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            level = (int)GameData.Instance.Level;
        }
        else
        {
            level = GetHighestLevel();
        }
        _scroller.SetCurrentLevel(level);
    }

    private int GetHighestLevel()
    {
        int highestLevel = 1;
        for (int index = 1; index <= GameData.NumLevels; index++)
        {
            if (!BtnScripts[index]) continue;
            if (!BtnScripts[index].LevelAvailable()) continue;
            if (index > highestLevel)
                highestLevel = index;
        }

        // TODO force this to only be on the main path once we've decided how many levels will exist on it
        // TODO confirm this after we decide how many levels there will be
        if (highestLevel > GameData.NumLevels)
        {
            highestLevel = 1;
        }

        return highestLevel;
    }

    public void PlayButtonClicked()
    {
        Stats.SaveStats();
        _scroller.LevelSelect();
    }

    public void ReturnToMainScreen()
    {
        _scroller.MainMenu();
    }

    public void QuitButtonClicked()
    {
        Stats.SaveStats();
        Application.Quit();
    }

    public void StatsButtonClicked()
    {
        Stats.SaveStats();
        _scroller.StatsScreen();
    }

    public void TrainingButtonClicked()
    {
        Stats.SaveStats();
        SceneManager.LoadScene("Training");
    }
    
    public void LevelClick()
    {
        var levelId = EventSystem.current.currentSelectedGameObject.GetComponent<LevelButton>().Level;
        Stats.SaveStats();

        for (int index = 1; index < GameData.NumLevels; index++)
        {
            if (BtnScripts[index] == null) continue;

            BtnScripts[index].Click(levelId);
            if (!BtnScripts[index].IsDoubleClicked(levelId)) continue;

            GameData.Instance.Level = levelId;
            StartCoroutine("LoadLevel", levelId);
        }
    }

    private IEnumerator LoadLevel(LevelProgressionHandler.Levels levelId)
    {
        GameData.Instance.Level = levelId;
        Toolbox.Instance.Debug = false;
        AsyncOperation levelLoader;

        if (levelId.ToString().Contains("Boss"))
            levelLoader = SceneManager.LoadSceneAsync("Boss");
        else
            levelLoader = SceneManager.LoadSceneAsync("Levels");
        // TODO training

        LoadingOverlay.GetComponent<LoadScreen>().ShowLoadScreen();

        while (!levelLoader.isDone)
        {
            yield return null;
        }
    }

    public void LvEndlessButtonClicked() { StartCoroutine("LoadLevel", LevelProgressionHandler.Levels.Endless); }
}
