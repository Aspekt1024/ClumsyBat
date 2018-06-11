using UnityEngine;

public class MainMenu : MonoBehaviour {
    
    public RectTransform MenuButtons;

    private RectTransform playButton;
    private RectTransform optionsButton;
    private RectTransform statsButton;

    private CamPositioner camPositioner;
    private MainMenuDropdownHandler dropdownHandler;
    
    private void Awake()
    {
        GameData.Instance.Data.LoadDataObjects();
        camPositioner = GetComponent<CamPositioner>();
        dropdownHandler = FindObjectOfType<MainMenuDropdownHandler>();
        
        GetMenuButtonRects();
        ShowMenuButtons();
    }
    
    private void Update()
    {
        GameData.Instance.Data.Stats.IdleTime += Time.deltaTime;
    }
    
    public void PlayButtonClicked()
    {
        if (camPositioner.IsMoving()) return;
        HideMenuButtons();
        SaveData();
        camPositioner.MoveToLevelMenu();
    }

    public void ReturnToMainScreen()
    {
        if (camPositioner.IsMoving()) return;
        ShowMenuButtons();
        SaveData();
        camPositioner.MoveToMainMenu();
    }

    public void StatsButtonClicked()
    {
        if (camPositioner.IsMoving()) return;
        SaveData();
        camPositioner.MoveToDropdownArea();
        dropdownHandler.StatsPressed();
    }

    public void OptionsButtonClicked()
    {
        if (camPositioner.IsMoving()) return;
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

    private void GetMenuButtonRects()
    {
        foreach (RectTransform rt in MenuButtons.GetComponentsInChildren<RectTransform>())
        {
            if (rt.name == "PlayButton")
                playButton = rt;
            else if (rt.name == "OptionsButton")
                optionsButton = rt;
            else if (rt.name == "StatsButton")
                statsButton = rt;
        }
    }

    private void HideMenuButtons()
    {
        UIObjectAnimator.Instance.PopOutObject(playButton);
        UIObjectAnimator.Instance.PopOutObject(optionsButton);
        UIObjectAnimator.Instance.PopOutObject(statsButton);
    }

    private void ShowMenuButtons()
    {
        UIObjectAnimator.Instance.PopInObject(playButton);
        UIObjectAnimator.Instance.PopInObject(optionsButton);
        UIObjectAnimator.Instance.PopInObject(statsButton);
    }

    private void SaveData() { GameData.Instance.Data.SaveData(); }
}
