﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtonHandler : MonoBehaviour {

    public GameObject LoadingOverlay;
    public Text LevelText;
    public RectTransform LevelPlayButton;

    public LevelProgressionHandler.Levels ActiveLevel;

    private LevelButton[] buttons;
    
    public LevelButton[] LevelButtons()
    {
        return buttons;
    }

    private void Start ()
    {
        GetLevelButtons();
        SetupLevelSelect();
        LoadingOverlay = GameObject.Find("LoadScreen");
        LevelPlayButton.gameObject.SetActive(false);
        transform.parent.GetComponentInChildren<LevelPath>().CreateLevelPaths();
    }

    public void LevelClick()
    {
        ActiveLevel = EventSystem.current.currentSelectedGameObject.GetComponent<LevelButton>().Level;

        for (int index = 1; index < GameData.Instance.Data.LevelData.NumLevels; index++)
        {
            if (buttons[index] == null || !buttons[index].LevelAvailable()) continue;

            buttons[index].Click(ActiveLevel);
            LevelText.text = Toolbox.Instance.LevelNames[ActiveLevel];

            if (!LevelPlayButton.gameObject.activeSelf)
                LevelPlayButton.gameObject.SetActive(true); // TODO animate
        }
    }

    public void LevelPlayClicked()
    {
        GameData.Instance.Level = ActiveLevel;
        StartCoroutine(LoadLevel(ActiveLevel));
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

        LoadingOverlay.GetComponent<LoadScreen>().ShowLoadScreen();

        while (!levelLoader.isDone)
        {
            yield return null;
        }
    }

    private void GetLevelButtons()
    {
        buttons = new LevelButton[GameData.Instance.Data.LevelData.NumLevels + 1];
        foreach (LevelButton lvlButton in GetComponentsInChildren<LevelButton>())
        {
            int level = (int)lvlButton.Level;
            if (level >= 1 && level <= GameData.Instance.Data.LevelData.NumLevels)
            {
                buttons[level] = lvlButton;

                if (GameData.Instance.Data.LevelData.IsUnlocked(level) || level == 1)
                {
                    buttons[level].SetLevelState(GameData.Instance.Data.LevelData.IsCompleted(level)
                        ? LevelButton.LevelStates.Completed
                        : LevelButton.LevelStates.Enabled);
                }
                else
                {
                    buttons[level].SetLevelState(LevelButton.LevelStates.Disabled);
                }

                if (GameData.Instance.Data.LevelData.IsCompleted(level))
                {
                    lvlButton.Star1Complete = GameData.Instance.Data.LevelData.LevelCompletedAchievement(level);
                    lvlButton.Star2Complete = GameData.Instance.Data.LevelData.AllMothsGathered(level);
                    lvlButton.Star3Complete = GameData.Instance.Data.LevelData.NoDamageTaken(level);
                    lvlButton.StarsSet = true;
                }
            }
        }
    }

    private int GetHighestLevel()
    {
        int highestLevel = 1;
        for (int index = 1; index <= GameData.Instance.Data.LevelData.NumLevels; index++)
        {
            if (!buttons[index]) continue;
            if (!buttons[index].LevelAvailable()) continue;
            if (index < (int)LevelProgressionHandler.Levels.Boss5 && index > highestLevel)
                highestLevel = index;
        }

        return highestLevel;
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
        // TODO set the scroller
        //_scroller.SetCurrentLevel(level);
    }
}
