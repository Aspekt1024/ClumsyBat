using UnityEngine;

public class MainMenu : MonoBehaviour {
    
    public GameObject MenuButtons;

    private GameObject _runtimeScripts;
    private MenuScroller _scroller;
    private CamPositioner camPositioner;
    private MainMenuDropdownHandler dropdownHandler;
    
    private void Awake()
    {
        GameData.Instance.Data.LoadDataObjects();
        _runtimeScripts = new GameObject("Runtime Scripts");
        _scroller = _runtimeScripts.AddComponent<MenuScroller>();   // TODO required?
        camPositioner = GetComponent<CamPositioner>();
        dropdownHandler = FindObjectOfType<MainMenuDropdownHandler>();
    }
    
    private void Update()
    {
        GameData.Instance.Data.Stats.IdleTime += Time.deltaTime;
    }
    
    public void PlayButtonClicked()
    {
        SaveData();
        camPositioner.MoveToLevelMenu();
    }

    public void ReturnToMainScreen()
    {
        SaveData();
        camPositioner.MoveToMainMenu();
    }

    public void StatsButtonClicked()
    {
        SaveData();
        camPositioner.MoveToDropdownArea();
        dropdownHandler.StatsPressed();
    }

    public void OptionsButtonClicked()
    {
        SaveData();
        camPositioner.MoveToDropdownArea();
        dropdownHandler.OptionsPressed();
    }

    public void QuitButtonClicked()
    {
        SaveData();
        Application.Quit();
    }
    
    public void LvEndlessButtonClicked() {
        //StartCoroutine(LoadLevel(LevelProgressionHandler.Levels.Endless));
    }

    private void SaveData() { GameData.Instance.Data.SaveData(); }
}
