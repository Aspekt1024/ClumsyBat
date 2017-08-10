using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtonHandler : MonoBehaviour {

    public GameObject LoadingOverlay;
    public RectTransform LevelTextRT;
    public RectTransform LevelPlayButton;
    
    public LevelProgressionHandler.Levels ActiveLevel;

    private LevelButton[] buttons;
    private RectTransform levelContentRect;
    private ScrollRect levelScrollRect;
    private Text levelText;
    private int CurrentLevel;

    public LevelButton[] LevelButtons()
    {
        return buttons;
    }

    private void Start ()
    {
        levelContentRect = GameObject.Find("Content").GetComponent<RectTransform>();
        levelScrollRect = GameObject.Find("LevelScrollRect").GetComponent<ScrollRect>();
        levelText = LevelTextRT.GetComponent<Text>();

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
            
            if (!LevelPlayButton.gameObject.activeSelf)
                Toolbox.UIAnimator.PopInObject(LevelPlayButton);
            
            buttons[index].Click(ActiveLevel);

            if (index == (int)ActiveLevel)
                SetLevelText(Toolbox.Instance.LevelNames[ActiveLevel]);
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


    private float GetButtonPosX()
    {
        GameObject lvlButton = GameObject.Find("Lv" + CurrentLevel);
        if (!lvlButton) { return 0; }
        RectTransform lvlButtonRt = lvlButton.GetComponent<RectTransform>();
        float ButtonPosX = lvlButtonRt.position.x - levelContentRect.position.x;
        return ButtonPosX;
    }

    public void SetCurrentLevel(int Level)
    {
        CurrentLevel = Level;
        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            JumpToCurrentLevel();
        }
    }
    private void JumpToCurrentLevel()
    {
        GotoCurrentLevel(Instantly: true);
    }

    private void GotoCurrentLevel(bool Instantly = false)
    {
        const float MaxPosX = 7f;
        float LvlButtonPosX = GetButtonPosX();
        if (LvlButtonPosX > MaxPosX)
        {
            float XShift = LvlButtonPosX - MaxPosX;
            float ContentScale = GameObject.Find("ScrollOverlay").GetComponent<RectTransform>().localScale.x;
            float NormalisedPosition = XShift / levelContentRect.rect.width / ContentScale;
            if (!Instantly)
            {
                StartCoroutine("GotoLevelAnim", NormalisedPosition);
            }
            else
            {
                levelScrollRect.horizontalNormalizedPosition = NormalisedPosition;
            }
        }
    }

    private void SetLevelText(string text)
    {
        const float normalWidth = 621f;
        const float normalScale = 0.31f;
        const float extraWidth = 800f;
        const float extraScale = 0.25f;

        Toolbox.UIAnimator.PopInObject(LevelTextRT);
        levelText.text = text;

        return;
        if (text.Length > 14)
        {
            LevelTextRT.sizeDelta = new Vector2(extraWidth, 80f);
            LevelTextRT.localScale = Vector2.one * extraScale;
        }
        else
        {
            LevelTextRT.sizeDelta = new Vector2(normalWidth, 80f);
            LevelTextRT.localScale = Vector2.one * normalScale;
        }
    }
}
