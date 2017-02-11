using UnityEngine;

public class LevelScript : MonoBehaviour {

    public LevelProgressionHandler.Levels DefaultLevel = LevelProgressionHandler.Levels.Main1;
    
    public ParralaxBG Background;
    public GameMenuOverlay GameMenu;
    public GameUI GameHud;
    public StatsHandler Stats;

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
        Stats = FindObjectOfType<StatsHandler>();
        CreateGameObjects();
        GameMenu.Hide();
        Toolbox.Instance.LevelSpeed = LevelScrollSpeed;
        SetLevel();
    }

    private void Update ()
    {
        Stats.TotalTime += Time.deltaTime;
        if (!_bGameStarted || _bGamePaused) { Stats.IdleTime += Time.deltaTime; return; }
        Stats.PlayTime += Time.deltaTime;

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
        Toolbox.Instance.ShowLevelTooltips = (!Stats.LevelData.IsCompleted((int)level));
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
        Stats.SaveStats();
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
        Stats.SaveStats();
        GameMenu.GameOver();
        GameHud.GameOver();
    }

    public void LevelWon()
    {
        GameHud.LevelWon();
        GameMenu.WinGame();
        EventListener.LevelWon();
        GameObject.Find("Clumsy").GetComponent<PlayerController>().PauseGame(showMenu: false); // TODO refer clumsy in awake

        // TODO add sound to sound controller script
        var victoryClip = (AudioClip) Resources.Load("Audio/LevelComplete");
        _audioControl.PlayOneShot(victoryClip);
    }
}
