using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {
    
    private Image pauseImage;
    private Button pauseButton;
    private Text scoreText;
    private Text currencyText;
    private Text bigScoreText;
    private Text levelText;
    private Text resumeTimerText;
    private Text timeText;
    private RectTransform cooldownBar;
    private Image cooldownImage;
    private CanvasGroup gameUICanvas;
    private Image mothImage;
    
    private Vector3 currencyScale;
    private Vector3 collectedCurrencyScale;
    private RectTransform currencyRt;
    private RectTransform bigScoreTextRt;

    private RectTransform _resumeTimerRt;
    private int _resumeTime;
    private bool _bCooldownReady;

    private StatsHandler _stats;

    private bool _bGamePaused;
    
    private void Awake()
    {
        GetTextObjects();
    }

	private void Start ()
	{
	    _stats = GameData.Instance.Data.Stats;
        SetupUI();
	}

    private void HideBossUIElements()
    {
        scoreText.GetComponent<Text>().enabled = false;
        mothImage.enabled = false;
        currencyRt.GetComponent<Text>().enabled = false;
    }
	
    private void SetScore()
    {
        if (_bGamePaused) { return; }
        scoreText.text = _stats.Score.ToString();
    }

    private void SetupUI()
    {
        currencyText.text = _stats.Currency.ToString();
        bigScoreText.text = string.Empty;
        gameUICanvas.alpha = 0f;
        gameUICanvas.blocksRaycasts = false;
    }

    public void UpdateTimer(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        if (minutes > 0)
        {
            seconds -= 60 * minutes;
            timeText.text = string.Format("{0}:{1}", minutes, seconds.ToString("n2"));
        }
        else
        {
            timeText.text = seconds.ToString("n2");
        }
    }

    public void SetCurrencyText(string text)
    {
        if (currencyText == null) return;
        if (currencyText.text == text) return;
        
        currencyText.gameObject.SetActive(true);
        StartCoroutine(PulseObject(currencyRt));
        currencyText.text = text;
    }
    
    private void GetTextObjects()
    {
        foreach (RectTransform rt in GetComponent<RectTransform>())
        {
            if (rt.name == "UIToggleElements")
            {
                gameUICanvas = rt.GetComponent<CanvasGroup>();
                foreach (RectTransform r in rt)
                {
                    switch (r.name)
                    {
                        case "PauseButton":
                            pauseButton = r.GetComponent<Button>();
                            pauseImage = r.GetComponent<Image>();
                            break;
                        case "LevelText":
                            levelText = r.GetComponent<Text>();
                            break;
                        case "ScoreText":
                            scoreText = r.GetComponent<Text>();
                            break;
                        case "CurrencyText":
                            currencyText = r.GetComponent<Text>();
                            currencyRt = r;
                            break;
                        case "BigPointsText":
                            bigScoreText = r.GetComponent<Text>();
                            bigScoreTextRt = r;
                            break;
                        case "TimeText":
                            timeText = r.GetComponent<Text>();
                            break;
                        case "MothImage":
                            mothImage = r.GetComponent<Image>();
                            break;
                        case "CooldownPanel":
                            foreach (RectTransform childRt in r.GetComponentsInChildren<RectTransform>())
                            {
                                if (childRt.name == "CooldownBar")
                                {
                                    cooldownBar = childRt;
                                    cooldownImage = childRt.GetComponent<Image>();
                                }
                            }
                            break;
                    }
                }
            }
            else if (rt.name == "ResumeTimerText")
            {
                resumeTimerText = rt.GetComponent<Text>();
                _resumeTimerRt = rt;
            }
        }
        // TODO remove these if not needed
        bigScoreTextRt.gameObject.SetActive(false);
        currencyRt.gameObject.SetActive(false);
    }

    public void SetResumeTimer(float timeRemaining)
    {
        if (resumeTimerText.enabled == false)
        {
            resumeTimerText.enabled = true;
        }
        if (_resumeTime != Mathf.CeilToInt(timeRemaining))
        {
            _resumeTime = Mathf.CeilToInt(timeRemaining);
            if (_resumeTime == 0)
            {
                return;
            }
            StartCoroutine("PulseObject", _resumeTimerRt);
            resumeTimerText.text = _resumeTime.ToString();
        }
    }

    public void HideResumeTimer()
    {
        resumeTimerText.enabled = false;
    }

    public void SetStartText(string startText)
    {
        StartCoroutine("PulseObject", _resumeTimerRt);
        resumeTimerText.text = startText;
    }

    public void SetLevelText(LevelProgressionHandler.Levels levelId)
    {
        // Note: this must be called by the LevelScript once the level has been set in GameData
        var level = (int) levelId;
        if (level == -1)
        {
            levelText.text = "Level: Endless";
        }
        else
        {
            levelText.text = "Level: " + Toolbox.Instance.LevelNames[levelId];
        }
    }

    public void SetCustomText(string text)
    {
        currencyText.text = text;
    }

    public void GamePaused(bool paused)
    {
        _bGamePaused = paused;
        gameUICanvas.alpha = paused ? 0f : 1f;
        gameUICanvas.blocksRaycasts = !paused;
    }

    public void StartGame()
    {
        if (GameData.Instance.Level.ToString().Contains("Boss"))
            HideBossUIElements();
        else
            InvokeRepeating("SetScore", 1f, 0.1f);

        gameUICanvas.alpha = 1f;
        gameUICanvas.blocksRaycasts = true;
    }

    public void GameOver()
    {
        gameUICanvas.alpha = 0f;
        gameUICanvas.blocksRaycasts = false;
    }

    public void LevelWon()
    {
        gameUICanvas.alpha = 0f;
        gameUICanvas.blocksRaycasts = false;
    }

    public void SetCooldown(float ratio)
    {
        cooldownBar.localScale = new Vector2(ratio, cooldownBar.localScale.y);
        if (Math.Abs(ratio - 1f) < 0.01f && !_bCooldownReady)
        {
            StartCoroutine(PulseObject(cooldownBar));
            _bCooldownReady = true;
            cooldownImage.color = new Color(212 / 255f, 195 / 255f, 126 / 255f);
        }
        else if (ratio < 1 && _bCooldownReady)
        {
            _bCooldownReady = false;
            cooldownImage.color = new Color(110 / 255f, 229 / 255f, 119 / 255f);
        }
    }

    public void ShowCooldown(bool bShow)
    {
        cooldownImage.enabled = bShow;
    }
    
    private IEnumerator PulseObject(RectTransform textObject)
    {
        float animTimer = 0f;
        const float animDuration = 0.2f;
        const float scaleMax = 0.25f;

        Vector3 startScale = textObject.localScale;
        
        while (animTimer < animDuration)
        {
            float scale;
            if (animTimer > animDuration / 2)
            {
                scale = (1f + scaleMax) - (scaleMax * (animTimer - animDuration / 2) / (animDuration / 2));
            }
            else
            {
                scale = 1f + (scaleMax * animTimer / (animDuration / 2));
            }
            textObject.localScale = startScale * scale;
            animTimer += Time.deltaTime;
            yield return null;
        }
    }
}
