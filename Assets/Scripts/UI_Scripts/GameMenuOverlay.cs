using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameMenuOverlay : MonoBehaviour {

    private StatsHandler Stats;
    private CanvasGroup LoadingOverlay = null;
    private DropdownMenu Menu = null;
    
    void Awake()
    {
        LoadingOverlay = GameObject.Find("LoadScreen").GetComponent<CanvasGroup>();
        SetCanvasActive(LoadingOverlay, true);
        Menu = FindObjectOfType<DropdownMenu>();
    }
    
    void Start ()
    {
        Stats = FindObjectOfType<StatsHandler>();
    }

    public void MenuButtonPressed()
    {
        Stats.SaveStats();
        // TODO loading screen
        SceneManager.LoadScene("Play");
    }

    public void RestartButtonPressed()
    {
        // TODO loading screen
        Stats.SaveStats();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OptionsButtonPressed()
    {
        Stats.SaveStats(); // Just because
        Menu.StartCoroutine("MenuSwitchAnim", true);
    }

    public void BackToMainPressed()
    {
        Menu.StartCoroutine("MenuSwitchAnim", false);
    }
    
    public void RemoveLoadingOverlay()
    {
        SetCanvasActive(LoadingOverlay, false);
    }
    
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
        // TODO get level name
        // TODO probably need to do other things here too...
        string LevelName = "Level " + Toolbox.Instance.Level;
        Menu.InGameMenu.LevelComplete(LevelName);
    }

    public void Hide()
    {
        Menu.Hide();
    }

    private void SetCanvasActive(CanvasGroup CanvasGrp, bool Active)
    {
        CanvasGrp.alpha = (Active ? 1f : 0f);
        CanvasGrp.interactable = Active;
        CanvasGrp.blocksRaycasts = Active;
    }

}
