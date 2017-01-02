using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DropdownOptionsMenu : MonoBehaviour {

    private StatsHandler Stats = null;

    private Animator ToggleMusic = null;
    private Animator ToggleSFX = null;
    private Animator ToggleTooltips = null;

    private CanvasGroup OptionsMainPanel;
    private CanvasGroup OptionsYesNoPanel;
    private CanvasGroup OptionsOKPanel;

    private Text OptionText;
    private Text OptionConfirmText;

    private enum YesNo
    {
        ResetData,
        ResetTooltips
    }
    private YesNo ConfirmOption;
    
    void Start ()
    {
        Stats = FindObjectOfType<StatsHandler>();
        GetMenuObjects();
        InitialiseOptionsView();
    }

    public void InitialiseOptionsView()
    {
        SetPanelVisible(OptionsMainPanel, true);
        SetPanelVisible(OptionsYesNoPanel, false);
        SetPanelVisible(OptionsOKPanel, false);
    }

    private void GetMenuObjects()
    {
        ToggleMusic = GameObject.Find("ToggleMusic").GetComponent<Animator>();
        ToggleSFX = GameObject.Find("ToggleSFX").GetComponent<Animator>();
        ToggleTooltips = GameObject.Find("ToggleTooltips").GetComponent<Animator>();

        OptionsMainPanel = GameObject.Find("OptionsMainPanel").GetComponent<CanvasGroup>();
        OptionsYesNoPanel = GameObject.Find("OptionsYesNoPanel").GetComponent<CanvasGroup>();
        OptionsOKPanel = GameObject.Find("OptionsOKPanel").GetComponent<CanvasGroup>();

        OptionText = GameObject.Find("OptionText").GetComponent<Text>();
        OptionConfirmText = GameObject.Find("OptionConfirmText").GetComponent<Text>();
    }

    public void ResetTooltipsPressed()
    {
        ConfirmOption = YesNo.ResetTooltips;
        OptionText.text = "Are you sure you want to reset tooltips?";
        SetPanelVisible(OptionsMainPanel, false);
        SetPanelVisible(OptionsYesNoPanel, true);
    }

    public void ResetDataPressed()
    {
        ConfirmOption = YesNo.ResetData;
        OptionText.text = "Are you sure you want to reset all game data? This is not reversible!";
        SetPanelVisible(OptionsMainPanel, false);
        SetPanelVisible(OptionsYesNoPanel, true);
    }

    public void ClearConfirmPressed()
    {
        switch(ConfirmOption)
        {
            case YesNo.ResetData:
                OptionConfirmText.text = "Game Data has been reset!";
                Stats.CompletionData.ClearCompletionData();
                Stats.ClearPlayerPrefs();
                break;
            case YesNo.ResetTooltips:
                OptionConfirmText.text = "Tooltips have been reset!";
                Stats.CompletionData.ResetTooltips();
                break;
        }
        SetPanelVisible(OptionsYesNoPanel, false);
        SetPanelVisible(OptionsOKPanel, true);
    }

    public void UnconfirmPressed()
    {
        SetPanelVisible(OptionsYesNoPanel, false);
        SetPanelVisible(OptionsMainPanel, true);
    }

    public void OKPressed()
    {
        switch(ConfirmOption)
        {
            case YesNo.ResetData:
                FindObjectOfType<LoadScreen>().ShowLoadScreen();
                SceneManager.LoadScene("Play");
                break;
            case YesNo.ResetTooltips:
                SetPanelVisible(OptionsOKPanel, false);
                SetPanelVisible(OptionsMainPanel, true);
                break;
        }
    }

    public void ToggleMusicPressed()
    {
        Stats.Settings.Music = !Stats.Settings.Music;
        ToggleMusic.Play(Stats.Settings.Music ? "MusicON" : "MusicOFF");
    }

    public void ToggleSFXPressed()
    {
        Stats.Settings.SFX = !Stats.Settings.SFX;
        ToggleSFX.Play(Stats.Settings.SFX ? "SFXON" : "SFXOFF");
    }

    public void ToggleTooltipsPressed()
    {
        Stats.Settings.Tooltips = !Stats.Settings.Tooltips;
        ToggleTooltips.Play(Stats.Settings.Tooltips ? "TooltipsON" : "TooltipsOFF");
    }
    
    public void SetToggleStates()
    {
        string MusicState = (Stats.Settings.Music ? "MusicON" : "MusicOFF");
        string SFXState = (Stats.Settings.SFX ? "SFXON" : "SFXOFF");
        string TooltipsState = (Stats.Settings.Tooltips ? "TooltipsON" : "TooltipsOFF");

        ToggleMusic.Play(MusicState);
        ToggleSFX.Play(SFXState);
        ToggleTooltips.Play(TooltipsState);
    }

    private void SetPanelVisible(CanvasGroup Panel, bool Visible)
    {
        Panel.alpha = (Visible ? 1f : 0f);
        Panel.blocksRaycasts = Visible;
        Panel.interactable = Visible;
    }
}
