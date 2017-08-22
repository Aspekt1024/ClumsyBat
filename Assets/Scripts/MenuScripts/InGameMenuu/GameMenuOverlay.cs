using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameMenuOverlay : MonoBehaviour {
    
    private LoadScreen _loadingOverlay;
    private DropdownMenu _menu;
    
    private void Awake()
    {
        _loadingOverlay = GameObject.Find("LoadScreen").GetComponent<LoadScreen>();
        _menu = FindObjectOfType<DropdownMenu>();
    }

    /// <summary>
    /// Button presses
    /// </summary>
    
    public void MenuButtonPressed()
    {
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.MainMenu;
        StartCoroutine(GotoMainMenu());
    }

    public void NextButtonPressed()
    {
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.LevelSelect;
        StartCoroutine(GotoMainMenu());
    }

    public void RestartButtonPressed()
    {
        Toolbox.Instance.ResetTooltips();
        StartCoroutine(RestartLevel());
    }

    public void OptionsButtonPressed()
    {
        _menu.StartCoroutine("MenuSwitchAnim", true);
    }

    public void BackToMainPressed()
    {
        SaveData(); // Just because
        _menu.StartCoroutine("MenuSwitchAnim", false);
    }

    public void ShareButtonPressed()
    {

    }

    /// <summary>
    /// Gameplay Events
    /// </summary>
   
    public void GameOver()
    {
        _menu.InGameMenu.GameOver();
    }
    
    public void PauseGame()
    {
        _menu.InGameMenu.PauseMenu();
    }

    public void WinGame()
    {
        string levelName = Toolbox.Instance.LevelNames[GameData.Instance.Level];
        _menu.InGameMenu.LevelComplete(levelName);
    }

    public void Hide()
    {
        _menu.Hide();
    }

    public float RaiseMenu()
    {
        float waitTime = _menu.RaiseMenu();
        return waitTime;
    }

    public void RemoveLoadingOverlay()
    {
        _loadingOverlay.HideLoadScreen();
    }

    private void SaveData()
    {
        GameData.Instance.Data.SaveData();
    }

    private IEnumerator GotoMainMenu()
    {
        Toolbox.Instance.ResetTooltips();
        yield return StartCoroutine(ShowLoadScreenRoutine());
        SceneManager.LoadScene("Play");
    }

    private IEnumerator RestartLevel()
    {
        yield return ShowLoadScreenRoutine();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator ShowLoadScreenRoutine()
    {
        yield return StartCoroutine(_loadingOverlay.FadeIn());
        SaveData();
    }
}
