using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using ClumsyBat.Managers;

public class GameMenuOverlay : MonoBehaviour {
    
    private DropdownMenu _menu;
    
    private void Awake()
    {
        _menu = FindObjectOfType<DropdownMenu>();
        CameraManager.OnCameraChanged += CameraChanged;
    }

    private void Start()
    {
        _menu.Hide();
    }

    /// <summary>
    /// Button presses
    /// </summary>

    public void MenuButtonPressed()
    {
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.MainMenu; // TODO no no no no no no!
        StartCoroutine(GotoMainMenu());
    }

    public void NextButtonPressed()
    {
        GameData.Instance.Level = GameData.Instance.NextLevel;
        if (GameData.Instance.Level == LevelProgressionHandler.Levels.Unassigned)
        {
            Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.LevelSelect;
            StartCoroutine(GotoMainMenu());
        }
        else if (GameData.Instance.Level.ToString().Substring(0, 4) == "Boss")
        {
            StartCoroutine(GotoBossLevel());
        }
        else
        {
            StartCoroutine(GotoNormalLevel());

        }
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
        _menu.PauseMenu.GameOver();
    }
    
    public void PauseGame()
    {
        _menu.PauseMenu.PauseMenu();
        _menu.DropMenu();
    }

    public void WinGame()
    {
        string levelName = Toolbox.Instance.LevelNames[GameData.Instance.Level];
        _menu.PauseMenu.LevelComplete(levelName);
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

    private void SaveData()
    {
        GameData.Instance.Data.SaveData();
    }

    private IEnumerator GotoBossLevel()
    {
        yield return StartCoroutine(ShowLoadScreenRoutine());
        SceneManager.LoadScene("Boss");
    }

    private IEnumerator GotoNormalLevel()
    {
        yield return StartCoroutine(ShowLoadScreenRoutine());
        SceneManager.LoadScene("Levels");
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
        yield return StartCoroutine(OverlayManager.Instance.LoadScreen.FadeIn());
        SaveData();
    }

    private void CameraChanged(Camera newCamera)
    {
        GetComponent<Canvas>().worldCamera = newCamera;
    }
}
