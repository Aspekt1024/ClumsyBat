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
    public RectTransform LevelScoreTextRt;
    
    public LevelProgressionHandler.Levels ActiveLevel;

    private LevelButton[] buttons;
    private RectTransform levelContentRect;
    private ScrollRect levelScrollRect;
    private Text levelText;
    private Text levelScoreText;
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
        levelScoreText = LevelScoreTextRt.GetComponent<Text>();

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

            if (index == (int)ActiveLevel && buttons[index].LevelAvailable())
            {
                StartCoroutine(SetLevelText(Toolbox.Instance.LevelNames[ActiveLevel], GameData.Instance.Data.LevelData.GetBestScore((int)ActiveLevel)));
                if (!LevelPlayButton.gameObject.activeSelf)
                    Toolbox.UIAnimator.PopInObject(LevelPlayButton);
            }
        }
    }

    public void LevelPlayClicked()
    {
        GameData.Instance.Level = ActiveLevel;
        UIObjectAnimator.Instance.PopOutObject(LevelPlayButton);
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
    
    private void SetupLevelSelect()
    {
        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
            CurrentLevel = (int)GameData.Instance.Level;
        else
            CurrentLevel = GetHighestLevel();

        levelScoreText.text = string.Empty;
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
    
    public IEnumerator MoveLevelMapToStart()
    {
        while (levelScrollRect.horizontalNormalizedPosition > 0.1f)
        {
            levelScrollRect.horizontalNormalizedPosition = Mathf.Lerp(levelScrollRect.horizontalNormalizedPosition, 0, Time.deltaTime * 4);
            yield return null;
        }
        levelScrollRect.horizontalNormalizedPosition = 0;
    }

    public void SetCurrentLevel(int Level)
    {
        CurrentLevel = Level;
        GotoCurrentLevel(Instantly: true);
    }

    private void GotoCurrentLevel(bool Instantly = false)
    {
        const float maxPosX = 7f;
        float LvlButtonPosX = GetButtonPosX();
        if (LvlButtonPosX > maxPosX)
        {
            float xShift = LvlButtonPosX - maxPosX;
            float contentScale = GameObject.Find("ScrollOverlay").GetComponent<RectTransform>().localScale.x;
            float normalisedPosition = xShift / levelContentRect.rect.width / contentScale;
            if (!Instantly)
            {
                StartCoroutine(MoveToLevel(normalisedPosition));
            }
            else
            {
                levelScrollRect.horizontalNormalizedPosition = normalisedPosition;
            }
        }
    }

    private IEnumerator MoveToLevel(float normalisedPosition)
    {
        while (Mathf.Abs(levelScrollRect.horizontalNormalizedPosition - normalisedPosition) > 0.1f)
        {
            levelScrollRect.horizontalNormalizedPosition = Mathf.Lerp(levelScrollRect.horizontalNormalizedPosition, normalisedPosition, Time.deltaTime * 2);
            yield return null;
        }
        levelScrollRect.horizontalNormalizedPosition = normalisedPosition;
    }

    private float GetButtonPosX()
    {
        GameObject lvlButton = GameObject.Find("Lv" + CurrentLevel);
        if (!lvlButton) { return 0; }
        RectTransform lvlButtonRt = lvlButton.GetComponent<RectTransform>();
        float ButtonPosX = lvlButtonRt.position.x - levelContentRect.position.x;
        return ButtonPosX;
    }

    private IEnumerator SetLevelText(string text, int score)
    {
        Toolbox.UIAnimator.PopObject(LevelTextRT);
        yield return new WaitForSeconds(0.08f);

        levelScoreText.text = "Best Score: " + score.ToString();
        Toolbox.UIAnimator.PopObject(LevelScoreTextRt);
        levelText.text = text;
    }
}
