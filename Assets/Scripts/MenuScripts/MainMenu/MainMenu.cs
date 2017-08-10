using UnityEngine;

public class MainMenu : MonoBehaviour {
    
    public GameObject MenuButtons;

    private RectTransform playButton;
    
    private CamPositioner camPositioner;
    private MainMenuDropdownHandler dropdownHandler;
    
    private void Awake()
    {
        GameData.Instance.Data.LoadDataObjects();
        camPositioner = GetComponent<CamPositioner>();
        dropdownHandler = FindObjectOfType<MainMenuDropdownHandler>();

        GetMenuButtonTransforms();
        Toolbox.UIAnimator.PopInObject(playButton);
    }
    
    private void Update()
    {
        GameData.Instance.Data.Stats.IdleTime += Time.deltaTime;
    }
    
    public void PlayButtonClicked()
    {
        Toolbox.UIAnimator.PopOutObject(playButton);
        SaveData();
        camPositioner.MoveToLevelMenu();
    }

    public void ReturnToMainScreen()
    {
        Toolbox.UIAnimator.PopInObject(playButton);
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

    private void GetMenuButtonTransforms()
    {
        foreach (Transform tf in MenuButtons.transform)
        {
            if (tf.name == "PlayButton")
            {
                playButton = tf.GetComponent<RectTransform>();
            }
        }
    }

    private void SaveData() { GameData.Instance.Data.SaveData(); }
}
