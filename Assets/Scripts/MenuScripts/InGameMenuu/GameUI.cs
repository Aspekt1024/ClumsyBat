using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {
    
    private Image _pauseImage;
    private Button _pauseButton;
    private Text _scoreText;
    private Text _currencyText;
    private Text _collectedCurrencyText;
    private Text _levelText;
    private Text _resumeTimerText;
    private RectTransform _cooldownBar;
    private Image _cooldownImage;

    // We're keeping these to isolate the values from the stats when we do the level won currency collect animation
    private int _currency;
    private int _collectedCurrency;

    private bool _bPulseAnimating;
    private Vector3 _currencyScale;
    private Vector3 _collectedCurrencyScale;
    private RectTransform _currencyRt;
    private RectTransform _collectedCurrencyRt;

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
        SetupUi();
	}
	
	void Update ()
    {
        if (_bGamePaused) { return; }
        _scoreText.text = ((int)_stats.Distance) + "m";
    }

    private void SetupUi()
    {
        _currencyText.text = _stats.Currency.ToString();
        EnablePauseButton(false);
        UpdateCurrency(pulse: false);
    }

    public void UpdateCurrency(bool pulse)
    {
        _currency = _stats.Currency;
        _collectedCurrency = _stats.CollectedCurrency;
        if (pulse)
        {
            StartCoroutine("PulseObject", _collectedCurrencyRt);
        }
        SetCurrencyText();
    }

    private void SetCurrencyText()
    {
        if (_collectedCurrency > 0)
        {
            _collectedCurrencyText.text = "+ " + _collectedCurrency;
        }
        else
        {
            _collectedCurrencyText.text = string.Empty;
        }
        _currencyText.text = _currency.ToString();
    }

    private void GetTextObjects()
    {
        foreach (RectTransform rt in GetComponent<RectTransform>())
        {
            switch (rt.name)
            {
                case "ResumeTimerText":
                    _resumeTimerText = rt.GetComponent<Text>();
                    _resumeTimerRt = rt;
                    break;
                case "PauseButton":
                    _pauseButton = rt.GetComponent<Button>();
                    _pauseImage = rt.GetComponent<Image>();
                    break;
                case "LevelText":
                    _levelText = rt.GetComponent<Text>();
                    break;
                case "ScoreText":
                    _scoreText = rt.GetComponent<Text>();
                    break;
                case "CurrencyText":
                    _currencyText = rt.GetComponent<Text>();
                    _currencyRt = rt;
                    break;
                case "CollectedCurrencyText":
                    _collectedCurrencyText = rt.GetComponent<Text>();
                    _collectedCurrencyRt = rt;
                    break;
                case "CooldownPanel":
                    foreach (RectTransform childRt in rt)
                    {
                        if (childRt.name == "CooldownBar")
                        {
                            _cooldownBar = childRt;
                            _cooldownImage = childRt.GetComponent<Image>();
                        }
                    }
                    break;
            }
        }
    }

    public void SetResumeTimer(float timeRemaining)
    {
        if (_resumeTimerText.enabled == false)
        {
            _resumeTimerText.enabled = true;
        }
        if (_resumeTime != Mathf.CeilToInt(timeRemaining))
        {
            _resumeTime = Mathf.CeilToInt(timeRemaining);
            if (_resumeTime == 0)
            {
                return;
            }
            StartCoroutine("PulseObject", _resumeTimerRt);
            _resumeTimerText.text = _resumeTime.ToString();
        }
    }

    public void HideResumeTimer()
    {
        _resumeTimerText.enabled = false;
    }

    public void SetStartText(string startText)
    {
        StartCoroutine("PulseObject", _resumeTimerRt);
        _resumeTimerText.text = startText;
    }

    public void SetLevelText(LevelProgressionHandler.Levels levelId)
    {
        // Note: this must be called by the LevelScript once the level has been set in GameData
        var level = (int) levelId;
        if (level == -1)
        {
            _levelText.text = "Level: Endless";
        }
        else
        {
            _levelText.text = "Level: " + Toolbox.Instance.LevelNames[levelId];
        }
    }

    public void SetCustomText(string text)
    {
        _currencyText.text = text;
    }

    public void GamePaused(bool paused)
    {
        _bGamePaused = paused;
        EnablePauseButton(!paused);
    }

    public void StartGame()
    {
        EnablePauseButton(true);
    }

    public void GameOver()
    {
        EnablePauseButton(false);
        StartCoroutine("ProcessCurrency", false);
    }

    private void EnablePauseButton(bool bEnabled)
    {
        _pauseButton.interactable = bEnabled;
        _pauseImage.enabled = bEnabled;
    }

    public void LevelWon()
    {
        EnablePauseButton(false);
        StartCoroutine("ProcessCurrency", true);
    }

    public void SetCooldown(float ratio)
    {
        _cooldownBar.localScale = new Vector2(ratio, _cooldownBar.localScale.y);
        if (Math.Abs(ratio - 1f) < 0.01f && !_bCooldownReady)
        {
            StartCoroutine("PulseObject", _cooldownBar);
            _bCooldownReady = true;
            _cooldownImage.color = new Color(212 / 255f, 195 / 255f, 126 / 255f);
        }
        else if (ratio < 1 && _bCooldownReady)
        {
            _bCooldownReady = false;
            _cooldownImage.color = new Color(110 / 255f, 229 / 255f, 119 / 255f);
        }
    }

    public void ShowCooldown(bool bShow)
    {
        _cooldownImage.enabled = bShow;
    }

    private IEnumerator ProcessCurrency(bool bCollect)
    {
        // Note: Currency has already been processed elsewhere
        // This is just an animation
        float animTimer = 0f;
        const float animDuration = 1f;
        float fromCurrency = _collectedCurrency;
        float toCurrency = _currency + _collectedCurrency;
        
        _currencyScale = _currencyRt.localScale;
        _collectedCurrencyScale = _collectedCurrencyRt.localScale;
        
        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            float delta = animTimer / animDuration;
            
            if (bCollect)
            {
                int oldCurrency = _currency;
                _currency = (int)(toCurrency - (int)((1 - delta) * fromCurrency));
                if (oldCurrency != _currency)
                {
                    _currencyRt.localScale = _currencyScale;
                    StartCoroutine("PulseObject", _currencyRt);
                }
            }

            int oldCollectedCurrency = _collectedCurrency;
            _collectedCurrency = (int)((1 - delta) * fromCurrency);
            if (_collectedCurrency != oldCollectedCurrency)
            {
                _collectedCurrencyRt.localScale = _collectedCurrencyScale;
                StartCoroutine("PulseObject", _collectedCurrencyRt);
            }
            
            while (_bPulseAnimating && _collectedCurrency == 0)
            {
                animTimer += Time.deltaTime;
                yield return null;
            }

            SetCurrencyText();
            yield return null;
        }
    }

    private IEnumerator PulseObject(RectTransform textObject)
    {
        float animTimer = 0f;
        const float animDuration = 0.2f;
        const float scaleMax = 0.25f;

        Vector3 startScale = textObject.localScale;

        _bPulseAnimating = true;
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
        _bPulseAnimating = false;
    }
}
