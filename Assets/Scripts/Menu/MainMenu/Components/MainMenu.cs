using ClumsyBat;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    
    public RectTransform MenuButtons;

    private RectTransform playButton;
    private RectTransform optionsButton;
    private RectTransform statsButton;

    private CamPositioner camPositioner;
    
    private void Awake()
    {
        camPositioner = GetComponent<CamPositioner>();
        
        GetMenuButtonRects();
        ShowMenuButtons();
    }
    
    public void PlayButtonClicked()
    {
        if (camPositioner.IsMoving()) return;
        HideMenuButtons();
        GameStatics.Data.SaveData();
        camPositioner.MoveToLevelMenu();
    }

    public void ReturnToMainScreen()
    {
        if (camPositioner.IsMoving()) return;
        ShowMenuButtons();
        GameStatics.Data.SaveData();
        camPositioner.MoveToMainMenu();
    }

    public void StatsButtonClicked()
    {
        if (camPositioner.IsMoving()) return;
        GameStatics.Data.SaveData();
        camPositioner.MoveToDropdownArea();
        GameStatics.UI.DropdownMenu.ShowStats();
    }

    public void OptionsButtonClicked()
    {
        if (camPositioner.IsMoving()) return;
        GameStatics.Data.SaveData();
        camPositioner.MoveToDropdownArea();
        GameStatics.UI.DropdownMenu.ShowOptions();
    }

    public void QuitButtonClicked()
    {
        GameStatics.Data.SaveData();
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
}
