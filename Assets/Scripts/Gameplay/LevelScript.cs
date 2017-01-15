using UnityEngine;

public class LevelScript : MonoBehaviour {

    public int DefaultLevel = 1;
    
    public ParralaxBG Background;
    public GameMenuOverlay GameMenu;
    public GameUI GameHUD;
    public StatsHandler Stats;

    private GameObject _levelScripts;
    private LevelObjectHandler _levelObjects;

    // Gameplay attributes
    public const float LevelScrollSpeed = 5f;    // first initialisation of LevelSpeed
    private bool _bGameStarted;
    private bool _bGamePaused;
    private bool _bAtEnd;
    private float _gameSpeed = 1f;
    private float _prevGameSpeed;

    private void Awake()
    {
        _levelScripts = new GameObject("Level Scripts");
        Stats = _levelScripts.AddComponent<StatsHandler>();
        GameHUD = GameObject.Find("UI_Overlay").GetComponent<GameUI>();
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
        int level = Toolbox.Instance.Level;
        if (level == 0)
        {
            Toolbox.Instance.Level = DefaultLevel;
            level = DefaultLevel;
        }
        GameHUD.SetLevelText(level);
        Toolbox.Instance.ShowLevelTooltips = (!Stats.LevelData.IsCompleted(level));
        _levelObjects.SetMode(bIsEndless: level == -1);
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
        GameHUD.StartGame();
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
        GameHUD.GamePaused(true);

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
        GameHUD.GamePaused(false);
    }

    public void ShowGameoverMenu()
    {
        Stats.SaveStats();
        GameMenu.GameOver();
        GameHUD.GameOver();
    }

    public void AddDistance(double timeTravelled, float playerSpeed)
    {
        float addDist = (float)timeTravelled * playerSpeed * Toolbox.Instance.LevelSpeed;
        Stats.Distance += addDist;
        Stats.TotalDistance += addDist;

        if (Stats.Distance > Stats.BestDistance)
        {
            Stats.BestDistance = Stats.Distance;
        }
    }

    public void LevelWon()
    {
        GameHUD.LevelWon();
        GameMenu.WinGame();
        Stats.LevelWon(Toolbox.Instance.Level);
        Stats.SaveStats();
        GameObject.Find("Clumsy").GetComponent<PlayerController>().PauseGame(ShowMenu: false);
    }
}
