using UnityEngine;
using System.Collections;

public class DropdownOptionsMenu : MonoBehaviour {

    private StatsHandler Stats = null;

    private Animator ToggleMusic = null;
    private Animator ToggleSFX = null;
    private Animator ToggleTooltips = null;

    // Use this for initialization
    void Start ()
    {
        Stats = FindObjectOfType<StatsHandler>();
        GetMenuObjects();
    }

    private void GetMenuObjects()
    {
        ToggleMusic = GameObject.Find("ToggleMusic").GetComponent<Animator>();
        ToggleSFX = GameObject.Find("ToggleSFX").GetComponent<Animator>();
        ToggleTooltips = GameObject.Find("ToggleTooltips").GetComponent<Animator>();
    }

    public void ClearTooltips()
    {

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
}
