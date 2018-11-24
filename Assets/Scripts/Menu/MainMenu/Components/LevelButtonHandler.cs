using ClumsyBat;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static LevelProgressionHandler;

public class LevelButtonHandler : MonoBehaviour {
    
    public RectTransform LevelTextRT;
    public RectTransform LevelPlayButton;
    public RectTransform LevelScoreTextRt;
    public ScrollRect LevelScrollRect;
    public RectTransform LevelContentRect;
    
    public Levels ActiveLevel;

    private LevelButton[] buttons;
    private Text levelText;
    private Text levelScoreText;
    private Levels CurrentLevel;

    public LevelButton[] LevelButtons()
    {
        return buttons;
    }

    private void Start ()
    {
        levelText = LevelTextRT.GetComponent<Text>();
        levelScoreText = LevelScoreTextRt.GetComponent<Text>();
    }

    public void Init()
    {
        GetLevelButtons();
        SetupLevelSelect();
        LevelPlayButton.gameObject.SetActive(false);
        transform.parent.GetComponentInChildren<LevelPath>().CreateLevelPaths();
    }

    public void LevelClick()
    {
        ActiveLevel = EventSystem.current.currentSelectedGameObject.GetComponent<LevelButton>().Level;

        for (int index = 1; index < GameStatics.Data.LevelDataHandler.NumLevels; index++)
        {
            if (buttons[index] == null || !buttons[index].LevelAvailable()) continue;
            
            buttons[index].Click(ActiveLevel);

            if (index == (int)ActiveLevel && buttons[index].LevelAvailable())
            {
                StartCoroutine(SetLevelText(Toolbox.Instance.LevelNames[ActiveLevel], GameStatics.Data.LevelDataHandler.GetBestScore(ActiveLevel)));
                if (!LevelPlayButton.gameObject.activeSelf)
                    UIObjectAnimator.Instance.PopInObject(LevelPlayButton);
            }
        }
    }

    /// <summary>
    /// Event that loads a level
    /// </summary>
    public void LevelPlayClicked()
    {
        UIObjectAnimator.Instance.PopOutObject(LevelPlayButton);
        GameStatics.GameManager.LoadLevel(ActiveLevel);
    }

    private void GetLevelButtons()
    {
        buttons = new LevelButton[GameStatics.Data.LevelDataHandler.NumLevels + 1];
        foreach (LevelButton lvlButton in GetComponentsInChildren<LevelButton>())
        {
            int level = (int)lvlButton.Level;
            if (level >= 1 && level <= GameStatics.Data.LevelDataHandler.NumLevels - 1)
            {
                buttons[level] = lvlButton;

                if (GameStatics.Data.LevelDataHandler.IsUnlocked(lvlButton.Level) || level == 1)
                {
                    buttons[level].SetLevelState(GameStatics.Data.LevelDataHandler.IsCompleted(lvlButton.Level)
                        ? LevelButton.LevelStates.Completed
                        : LevelButton.LevelStates.Enabled);
                }
                else
                {
                    buttons[level].SetLevelState(LevelButton.LevelStates.Disabled);
                }

                if (GameStatics.Data.LevelDataHandler.IsCompleted(lvlButton.Level))
                {
                    lvlButton.Star1Complete = GameStatics.Data.LevelDataHandler.LevelCompletedAchievement(lvlButton.Level);
                    lvlButton.Star2Complete = GameStatics.Data.LevelDataHandler.AllMothsGathered(lvlButton.Level);
                    lvlButton.Star3Complete = GameStatics.Data.LevelDataHandler.NoDamageTaken(lvlButton.Level);
                    lvlButton.StarsSet = true;
                }
            }
        }
    }
    
    private void SetupLevelSelect()
    {
        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
            CurrentLevel = GameStatics.LevelManager.Level;
        else
            CurrentLevel = GetHighestLevel();

        levelScoreText.text = string.Empty;
    }
    
    private Levels GetHighestLevel()
    {
        var highestLevel = Levels.Main1;
        var nextLevel = Levels.Main1;
        var levelData = GameStatics.Data.LevelDataHandler.GetLevelData(highestLevel);

        while (levelData.LevelCompleted)
        {
            highestLevel = nextLevel;
            nextLevel = GetNextLevel(highestLevel);
            if (nextLevel == highestLevel) break;

            levelData = GameStatics.Data.LevelDataHandler.GetLevelData(highestLevel);
        }
        
        return highestLevel;
    }
    
    public IEnumerator MoveLevelMapToStart()
    {
        while (LevelScrollRect.horizontalNormalizedPosition > 0.1f)
        {
            LevelScrollRect.horizontalNormalizedPosition = Mathf.Lerp(LevelScrollRect.horizontalNormalizedPosition, 0, Time.deltaTime * 4);
            yield return null;
        }
        LevelScrollRect.horizontalNormalizedPosition = 0;
    }

    public void SetCurrentLevel(Levels Level)
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
            float normalisedPosition = xShift / LevelContentRect.rect.width / contentScale;
            if (!Instantly)
            {
                StartCoroutine(MoveToLevel(normalisedPosition));
            }
            else
            {
                LevelScrollRect.horizontalNormalizedPosition = normalisedPosition;
            }
        }
    }

    private IEnumerator MoveToLevel(float normalisedPosition)
    {
        while (Mathf.Abs(LevelScrollRect.horizontalNormalizedPosition - normalisedPosition) > 0.1f)
        {
            LevelScrollRect.horizontalNormalizedPosition = Mathf.Lerp(LevelScrollRect.horizontalNormalizedPosition, normalisedPosition, Time.deltaTime * 2);
            yield return null;
        }
        LevelScrollRect.horizontalNormalizedPosition = normalisedPosition;
    }

    private float GetButtonPosX()
    {
        GameObject lvlButton = GameObject.Find("Lv" + CurrentLevel);
        if (!lvlButton) { return 0; }
        RectTransform lvlButtonRt = lvlButton.GetComponent<RectTransform>();
        float ButtonPosX = lvlButtonRt.position.x - LevelContentRect.position.x;
        return ButtonPosX;
    }

    private IEnumerator SetLevelText(string text, int score)
    {
        UIObjectAnimator.Instance.PopObject(LevelTextRT);
        yield return new WaitForSeconds(0.08f);

        levelScoreText.text = "Best Score: " + score.ToString();
        UIObjectAnimator.Instance.PopObject(LevelScoreTextRt);
        levelText.text = text;
    }
}
