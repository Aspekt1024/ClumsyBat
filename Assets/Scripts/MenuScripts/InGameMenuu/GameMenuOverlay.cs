using UnityEngine;
using UnityEngine.SceneManagement;

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
        Toolbox.Instance.TooltipCompletionPersist = false;
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.MainMenu;
        _loadingOverlay.ShowLoadScreen();
        SaveData();
        SceneManager.LoadScene("Play");
    }

    public void RestartButtonPressed()
    {
        Toolbox.Instance.TooltipCompletionPersist = true;
        _loadingOverlay.ShowLoadScreen();
        SaveData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    public void NextButtonPressed()
    {

        Toolbox.Instance.TooltipCompletionPersist = false;
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.LevelSelect;
        _loadingOverlay.ShowLoadScreen();
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
}
