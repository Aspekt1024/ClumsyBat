using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {
    
    public GameObject MenuButtons;
    public GameObject LoadingOverlay;

    private GameObject _runtimeScripts;
    private MenuScroller _scroller;
    
    private LevelButton[] _btnScripts;

    private void Awake()
    {
        GameData.Instance.Data.LoadDataObjects();
        _btnScripts = new LevelButton[GameData.Instance.Data.LevelData.NumLevels + 1];
        _runtimeScripts = new GameObject("Runtime Scripts");
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
        GameData.Instance.Data.Stats.IdleTime += Time.deltaTime;
    }

    private void GetLevelButtons()
    {
        var lvlButtons = GameObject.Find("LevelButtons").GetComponent<RectTransform>();
        foreach (RectTransform lvlButton in lvlButtons)
        {
            LevelProgressionHandler.Levels levelId = lvlButton.GetComponent<LevelButton>().Level;
            int level = (int)levelId;
            if (level >= 1 && level <= GameData.Instance.Data.LevelData.NumLevels)
            {
                _btnScripts[level] = lvlButton.GetComponent<LevelButton>();
                
                if (GameData.Instance.Data.LevelData.IsUnlocked(level) || level == 1)
                {
                    _btnScripts[level].SetLevelState(GameData.Instance.Data.LevelData.IsCompleted(level)
                        ? LevelButton.LevelStates.Completed
                        : LevelButton.LevelStates.Enabled);
                }
                else
                {
                    _btnScripts[level].SetLevelState(LevelButton.LevelStates.Disabled);
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
        for (int index = 1; index <= GameData.Instance.Data.LevelData.NumLevels; index++)
        {
            if (!_btnScripts[index]) continue;
            if (!_btnScripts[index].LevelAvailable()) continue;
            if (index > highestLevel)
                highestLevel = index;
        }

        // TODO force this to only be on the main path once we've decided how many levels will exist on it
        // TODO confirm this after we decide how many levels there will be
        if (highestLevel > GameData.Instance.Data.LevelData.NumLevels)
        {
            highestLevel = 1;
        }

        return highestLevel;
    }

    public void PlayButtonClicked()
    {
        SaveData();
        _scroller.LevelSelect();
    }

    public void ReturnToMainScreen()
    {
        _scroller.MainMenu();
    }

    public void QuitButtonClicked()
    {
        SaveData();
        Application.Quit();
    }

    public void StatsButtonClicked()
    {
        SaveData();
        _scroller.StatsScreen();
    }

    public void TrainingButtonClicked()
    {
        SaveData();
        SceneManager.LoadScene("Training");
    }
    
    public void LevelClick()
    {
        var levelId = EventSystem.current.currentSelectedGameObject.GetComponent<LevelButton>().Level;
        SaveData();

        for (int index = 1; index < GameData.Instance.Data.LevelData.NumLevels; index++)
        {
            if (_btnScripts[index] == null || !_btnScripts[index].LevelAvailable()) continue;
            
            _btnScripts[index].Click(levelId);
            if (!_btnScripts[index].IsDoubleClicked(levelId)) continue;

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

    private void SaveData() { GameData.Instance.Data.SaveData(); }
}
