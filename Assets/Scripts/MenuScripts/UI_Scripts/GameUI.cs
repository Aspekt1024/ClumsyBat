﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {
    
    private Image PauseImage;
    private Button PauseButton;
    private Text ScoreText;
    private Text CurrencyText;
    private Text CollectedCurrencyText;
    private Text LevelText;
    private Text ResumeTimerText;
    private RectTransform CooldownBar;
    private Image CooldownImage;

    // We're keeping these to isolate the values from the stats when we do the level won currency collect animation
    private int Currency = 0;
    private int CollectedCurrency = 0;

    private bool bPulseAnimating = false;
    private Vector3 CurrencyScale;
    private Vector3 CollectedCurrencyScale;
    private RectTransform CurrencyRT;
    private RectTransform CollectedCurrencyRT;

    private RectTransform ResumeTimerRT;
    private int ResumeTime;
    private bool bCooldownReady = false;

    private StatsHandler Stats = null;

    private bool bGamePaused = false;
    
	void Start ()
    {
        Stats = FindObjectOfType<StatsHandler>();
        GetTextObjects();
        SetupUI();
	}
	
	void Update ()
    {
        if (bGamePaused) { return; }
        ScoreText.text = ((int)Stats.Distance).ToString() + "m";
    }

    private void SetupUI()
    {
        CurrencyText.text = Stats.Currency.ToString();
        EnablePauseButton(false);
        UpdateCurrency(Pulse: false);
    }

    public void UpdateCurrency(bool Pulse)
    {
        Currency = Stats.Currency;
        CollectedCurrency = Stats.CollectedCurrency;
        if (Pulse)
        {
            StartCoroutine("PulseObject", CollectedCurrencyRT);
        }
        SetCurrencyText();
    }

    private void SetCurrencyText()
    {
        if (CollectedCurrency > 0)
        {
            CollectedCurrencyText.text = "+ " + CollectedCurrency.ToString();
        }
        else
        {
            CollectedCurrencyText.text = string.Empty;
        }
        CurrencyText.text = Currency.ToString();
    }

    private void GetTextObjects()
    {
        foreach (RectTransform RT in GetComponent<RectTransform>())
        {
            switch (RT.name)
            {
                case "ResumeTimerText":
                    ResumeTimerText = RT.GetComponent<Text>();
                    ResumeTimerRT = RT;
                    break;
                case "PauseButton":
                    PauseButton = RT.GetComponent<Button>();
                    PauseImage = RT.GetComponent<Image>();
                    break;
                case "LevelText":
                    LevelText = RT.GetComponent<Text>();
                    break;
                case "ScoreText":
                    ScoreText = RT.GetComponent<Text>();
                    break;
                case "CurrencyText":
                    CurrencyText = RT.GetComponent<Text>();
                    CurrencyRT = RT;
                    break;
                case "CollectedCurrencyText":
                    CollectedCurrencyText = RT.GetComponent<Text>();
                    CollectedCurrencyRT = RT;
                    break;
                case "CooldownPanel":
                    foreach (RectTransform childRT in RT)
                    {
                        if (childRT.name == "CooldownBar")
                        {
                            CooldownBar = childRT;
                            CooldownImage = childRT.GetComponent<Image>();
                        }
                    }
                    break;
            }
        }
    }

    public void SetResumeTimer(float TimeRemaining)
    {
        if (ResumeTimerText.enabled == false)
        {
            ResumeTimerText.enabled = true;
        }
        if (ResumeTime != Mathf.CeilToInt(TimeRemaining))
        {
            ResumeTime = Mathf.CeilToInt(TimeRemaining);
            if (ResumeTime == 0)
            {
                return;
            }
            StartCoroutine("PulseObject", ResumeTimerRT);
            ResumeTimerText.text = ResumeTime.ToString();
        }
    }

    public void HideResumeTimer()
    {
        ResumeTimerText.enabled = false;
    }

    public void SetStartText(string StartText)
    {
        StartCoroutine("PulseObject", ResumeTimerRT);
        ResumeTimerText.text = StartText;
    }

    public void SetLevelText(int Level)
    {
        // Note: this must be called by the LevelScript once the level has been set in the Toolbox
        if (Level == -1)
        {
            LevelText.text = "Level: Endless";
        }
        else
        {
            LevelText.text = "Level: " + Toolbox.Instance.LevelNames[Level];
        }
    }

    public void GamePaused(bool Paused)
    {
        bGamePaused = Paused;
        EnablePauseButton(!Paused);
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
        PauseButton.interactable = bEnabled;
        PauseImage.enabled = bEnabled;
    }

    public void LevelWon()
    {
        EnablePauseButton(false);
        StartCoroutine("ProcessCurrency", true);
    }

    public void SetCooldown(float Ratio)
    {
        CooldownBar.localScale = new Vector2(Ratio, CooldownBar.localScale.y);
        if (Ratio == 1f && !bCooldownReady)
        {
            StartCoroutine("PulseObject", CooldownBar);
            bCooldownReady = true;
            CooldownImage.color = new Color(212 / 255f, 195 / 255f, 126 / 255f);
        }
        else if (Ratio < 1 && bCooldownReady)
        {
            bCooldownReady = false;
            CooldownImage.color = new Color(110 / 255f, 229 / 255f, 119 / 255f);
        }
    }

    private IEnumerator ProcessCurrency(bool bCollect)
    {
        // Note: Currency has already been processed elsewhere
        // This is just an animation
        float AnimTimer = 0f;
        const float AnimDuration = 1f;
        float FromCurrency = CollectedCurrency;
        float ToCurrency = Currency + CollectedCurrency;
        
        CurrencyScale = CurrencyRT.localScale;
        CollectedCurrencyScale = CollectedCurrencyRT.localScale;
        
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float Delta = AnimTimer / AnimDuration;
            
            if (bCollect)
            {
                int OldCurrency = Currency;
                Currency = (int)(ToCurrency - (int)((1 - Delta) * FromCurrency));
                if (OldCurrency != Currency)
                {
                    CurrencyRT.localScale = CurrencyScale;
                    StartCoroutine("PulseObject", CurrencyRT);
                }
            }

            int OldCollectedCurrency = CollectedCurrency;
            CollectedCurrency = (int)((1 - Delta) * FromCurrency);
            if (CollectedCurrency != OldCollectedCurrency)
            {
                CollectedCurrencyRT.localScale = CollectedCurrencyScale;
                StartCoroutine("PulseObject", CollectedCurrencyRT);
            }
            
            while (bPulseAnimating && CollectedCurrency == 0)
            {
                AnimTimer += Time.deltaTime;
                yield return null;
            }

            SetCurrencyText();
            yield return null;
        }
    }

    private IEnumerator PulseObject(RectTransform TextObject)
    {
        float AnimTimer = 0f;
        const float AnimDuration = 0.2f;
        const float ScaleMax = 0.25f;

        Vector3 StartScale = TextObject.localScale;

        bPulseAnimating = true;
        while (AnimTimer < AnimDuration)
        {
            float Scale = 1f;
            if (AnimTimer > AnimDuration / 2)
            {
                Scale = (1f + ScaleMax) - (ScaleMax * (AnimTimer - AnimDuration / 2) / (AnimDuration / 2));
            }
            else
            {
                Scale = 1f + (ScaleMax * AnimTimer / (AnimDuration / 2));
            }
            TextObject.localScale = StartScale * Scale;
            AnimTimer += Time.deltaTime;
            yield return null;
        }
        bPulseAnimating = false;
    }
}