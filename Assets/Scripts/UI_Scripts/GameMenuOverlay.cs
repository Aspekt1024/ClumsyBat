﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuOverlay : MonoBehaviour {

    private StatsHandler Stats;
    private LoadScreen LoadingOverlay = null;
    private DropdownMenu Menu = null;
    
    void Awake()
    {
        LoadingOverlay = GameObject.Find("LoadScreen").GetComponent<LoadScreen>();
        Menu = FindObjectOfType<DropdownMenu>();
    }
    
    void Start ()
    {
        Stats = FindObjectOfType<StatsHandler>();
    }

    /// <summary>
    /// Button presses
    /// </summary>
    
    public void MenuButtonPressed()
    {
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.MainMenu;
        LoadingOverlay.ShowLoadScreen();
        Stats.SaveStats();
        SceneManager.LoadScene("Play");
    }

    public void RestartButtonPressed()
    {
        LoadingOverlay.ShowLoadScreen();
        Stats.SaveStats();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OptionsButtonPressed()
    {
        Menu.StartCoroutine("MenuSwitchAnim", true);
    }

    public void BackToMainPressed()
    {
        Stats.SaveStats(); // Just because
        Menu.StartCoroutine("MenuSwitchAnim", false);
    }

    public void NextButtonPressed()
    {
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.LevelSelect;
        LoadingOverlay.ShowLoadScreen();
        SceneManager.LoadScene("Play");
    }

    public void ShareButtonPressed()
    {

    }

    /// <summary>
    /// Gameplay Events
    /// </summary>
   
    public void GameOver()
    {
        Menu.InGameMenu.GameOver();
    }
    
    public void PauseGame()
    {
        Menu.InGameMenu.PauseMenu();
    }

    public void WinGame()
    {
        string LevelName = Toolbox.Instance.LevelNames[Toolbox.Instance.Level];
        Menu.InGameMenu.LevelComplete(LevelName);
    }

    public void Hide()
    {
        Menu.Hide();
    }

    public float RaiseMenu()
    {
        float WaitTime = Menu.RaiseMenu();
        return WaitTime;
    }

    public void RemoveLoadingOverlay()
    {
        LoadingOverlay.HideLoadScreen();
    }
}
