using UnityEngine;

public class LevelScript : MonoBehaviour {

    public LevelProgressionHandler.Levels DefaultLevel = LevelProgressionHandler.Levels.Main1;
    
    public ParralaxBG Background;
    public GameMenuOverlay GameMenu;
    public GameUI GameHud;

    private GameObject _levelScripts;
    private LevelObjectHandler _levelObjects;
    private AudioSource _audioControl;

    // Gameplay attributes
    public const float LevelScrollSpeed = 5f;    // first initialisation of LevelSpeed
    private bool _bGameStarted;
    private bool _bGamePaused;
    private bool _bAtEnd;
    private float _gameSpeed = 1f;
    private float _prevGameSpeed;

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
        Toolbox.Instance.LevelSpeed = LevelScrollSpeed;
        SetLevel();
    }

    private void Update ()
    {
        GameData.Instance.Data.Stats.TotalTime += Time.deltaTime;
        if (!_bGameStarted || _bGamePaused) { GameData.Instance.Data.Stats.IdleTime += Time.deltaTime; return; }
        GameData.Instance.Data.Stats.PlayTime += Time.deltaTime;

        if (_levelObjects.AtCaveEnd())
        {
            SetMovementForExit();
        }
    }

    private void SetMovementForExit()
    {
        _bAtEnd = true;
        Background.SetVelocity(0);
    }

    public bool AtCaveEnd()
    {
        return _bAtEnd;
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

    public void UpdateGameSpeed(float gameSpeed)
    {
        _gameSpeed = gameSpeed;
        float speed = _gameSpeed * LevelScrollSpeed;
        
        Background.SetVelocity(speed);
        _levelObjects.SetVelocity(speed);
    }

    public float GetGameSpeed() { return _gameSpeed; }

    public void HorribleDeath()
    {
        _gameSpeed = 0f;
        float speed = _gameSpeed * LevelScrollSpeed;

        Background.SetVelocity(speed);
        _levelObjects.SetVelocity(speed);

        GetComponent<AudioSource>().Stop();
    }

    public void StartGame()
    {
        _bGameStarted = true;
        GameHud.StartGame();
        UpdateGameSpeed(1);
        _levelObjects.SetPaused(false);
        _levelObjects.SetVelocity(LevelScrollSpeed);
    }

    public void PauseGame(bool showMenu = true)
    {
        // TODO Play pause sound
        _bGamePaused = true;
        _prevGameSpeed = _gameSpeed;
        UpdateGameSpeed(0);
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
        UpdateGameSpeed(_prevGameSpeed);
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
        GameHud.LevelWon();
        GameMenu.WinGame();
        GameData.Instance.SetLevelCompletion(GameData.LevelCompletePaths.MainPath);
        EventListener.LevelWon();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PauseGame(showMenu: false);

        // TODO add sound to sound controller script
        var victoryClip = (AudioClip) Resources.Load("Audio/LevelComplete");
        _audioControl.PlayOneShot(victoryClip);
    }
}
