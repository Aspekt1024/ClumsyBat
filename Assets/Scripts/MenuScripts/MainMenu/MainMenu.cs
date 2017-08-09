using UnityEngine;

public class MainMenu : MonoBehaviour {
    
    public GameObject MenuButtons;

    private GameObject _runtimeScripts;
    private MenuScroller _scroller;
    private CamPositioner camPositioner;
    
    private void Awake()
    {
        GameData.Instance.Data.LoadDataObjects();
        _runtimeScripts = new GameObject("Runtime Scripts");
        _scroller = _runtimeScripts.AddComponent<MenuScroller>();   // TODO required?
        camPositioner = GetComponent<CamPositioner>();
    }

    private void Start()
    {
        //GetComponent<AudioSource>().Play();
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

    public void QuitButtonClicked()
    {
        SaveData();
        Application.Quit();
    }

    public void StatsButtonClicked()
    {
        SaveData();
        _scroller.StatsScreen();
    }
    
    public void LvEndlessButtonClicked() { StartCoroutine("LoadLevel", LevelProgressionHandler.Levels.Endless); }

    private void SaveData() { GameData.Instance.Data.SaveData(); }
}
