using UnityEngine;

public class LevelScript : MonoBehaviour {

    // These attributes can be set in the inspector
    public float ClumsyBaseSpeed = 5f;    // TODO should this really belong with player?
    public LevelProgressionHandler.Levels DefaultLevel = LevelProgressionHandler.Levels.Main1;
    
    // These attributes must be set in the inspector (remnants of the early days)
    public ParralaxBG Background;
    public GameMenuOverlay GameMenu;

    [HideInInspector]
    public GameUI GameHud;

    private GameObject _levelScripts;
    private LevelObjectHandler _levelObjects;
    private AudioSource _audioControl;

    // Gameplay attributes
    private bool _bGameStarted;
    private bool _bGamePaused;
    private bool _bAtEnd;

    private void Awake()
    {
        _levelScripts = GameObject.Find("Scripts");
        _audioControl = _levelScripts.AddComponent<AudioSource>();
        GameHud = GameObject.Find("UI_Overlay").GetComponent<GameUI>();
    }

    private void Start ()
    {
        CreateGameObjects();
        GameMenu.Hide();
        Toolbox.Player.SetPlayerSpeed(ClumsyBaseSpeed);
        SetLevel();
    }

    private void Update ()
    {
        GameData.Instance.Data.Stats.TotalTime += Time.deltaTime;
        if (!_bGameStarted || _bGamePaused) { GameData.Instance.Data.Stats.IdleTime += Time.deltaTime; return; }
        GameData.Instance.Data.Stats.PlayTime += Time.deltaTime;
        
    }

    private void SetLevel()
    {
        var level = GameData.Instance.Level;
        if (level == LevelProgressionHandler.Levels.Unassigned)
        {
            GameData.Instance.Level = DefaultLevel;
            level = DefaultLevel;
        }
        GameHud.SetLevelText(level);
        Toolbox.Instance.ShowLevelTooltips = (!GameData.Instance.Data.LevelData.IsCompleted((int)level));
        _levelObjects.SetMode(bIsEndless: level == LevelProgressionHandler.Levels.Endless);
    }

    private void CreateGameObjects()
    {
        _levelObjects = _levelScripts.AddComponent<LevelObjectHandler>();
    }
    
    public void HorribleDeath()
    {
        
        GetComponent<AudioSource>().Stop();
    }

    public void StartGame()
    {
        _bGameStarted = true;
        GameHud.StartGame();
        _levelObjects.SetPaused(false);
    }

    public void PauseGame(bool showMenu = true)
    {
        // TODO Play pause sound
        _bGamePaused = true;
        _levelObjects.SetPaused(true);
        GameHud.GamePaused(true);

        if (showMenu)
        {
            GameMenu.PauseGame();
        }
        GameData.Instance.Data.SaveData();
    }

    public void ResumeGame()
    {
        // Play resume sound
        _bGamePaused = false;
        _levelObjects.SetPaused(false);
        GameHud.GamePaused(false);
    }

    public void ShowGameoverMenu()
    {
        GameData.Instance.Data.SaveData();
        GameMenu.GameOver();
        GameHud.GameOver();
    }

    public void LevelWon()
    {
        if (Toolbox.Player.ExitViaSecretPath)
        {
            GameData.Instance.SetLevelCompletion(GameData.LevelCompletePaths.Secret1);
        }
        else
        {
            GameData.Instance.SetLevelCompletion(GameData.LevelCompletePaths.MainPath);
        }
        EventListener.LevelWon();
        GameHud.LevelWon();
        GameMenu.WinGame();
        Toolbox.Player.GetComponent<PlayerController>().PauseGame(showMenu: false);

        // TODO add sound to sound controller script
        var victoryClip = (AudioClip) Resources.Load("Audio/LevelComplete");
        _audioControl.PlayOneShot(victoryClip);
    }
}
