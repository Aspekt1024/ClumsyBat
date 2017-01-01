using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {
    
    private Image PauseImage;
    private Button PauseButton;
    private Text ScoreText;
    private Text CurrencyText;
    private Text LevelText;
    private Text ResumeTimerText = null;

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
        CurrencyText.text = Stats.Currency.ToString();
    }

    private void GetTextObjects()
    {
        foreach (RectTransform RT in GetComponent<RectTransform>())
        {
            switch (RT.name)
            {
                case "ResumeTimerText":
                    ResumeTimerText = RT.GetComponent<Text>();
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
        ResumeTimerText.text = Mathf.Ceil(TimeRemaining).ToString();
    }

    public void HideResumeTimer()
    {
        ResumeTimerText.enabled = false;
    }

    public void SetStartText(string StartText)
    {
        ResumeTimerText.text = StartText;
    }

    private void SetupUI()
    {
        CurrencyText.text = Stats.Currency.ToString();
        EnablePauseButton(true);
    }

    private void SetLevelText()
    {
        if (Toolbox.Instance.Level == -1)
        {
            LevelText.text = "Level: Endless";
        }
        else
        {
            LevelText.text = "Level: " + Toolbox.Instance.LevelNames[Toolbox.Instance.Level];
        }
    }

    public void GamePaused(bool Paused)
    {
        bGamePaused = Paused;
        EnablePauseButton(false);
    }

    public void GameOver()
    {
        EnablePauseButton(false);
    }

    private void EnablePauseButton(bool bEnabled)
    {
        PauseButton.interactable = bEnabled;
        PauseImage.enabled = bEnabled;
    }
}
