using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelScript : MonoBehaviour {

    public int DefaultLevel = 1;

    // Game objects not created at runtime
    public ParralaxBG Background;
    
    // Game objects created at runtime
    private GameObject LevelScripts;
    private LevelObjectHandler LevelObjects;

    // Gameplay attributes
    public const float LevelScrollSpeed = 4;    // first initialisation of LevelSpeed
    private bool bGameStarted = false;
    private bool bGamePaused = false;
    private bool bAtEnd = false;
    private float GameSpeed = 1f;
    private float PrevGameSpeed;

    public GameMenuOverlay GameMenu;
    public GameUI GameHUD;

    public StatsHandler Stats;

    void Awake()
    {
        LevelScripts = new GameObject("Level Scripts");
        Stats = LevelScripts.AddComponent<StatsHandler>();
        GameHUD = GameObject.Find("UI_Overlay").GetComponent<GameUI>();
    }

    void Start ()
    {
        CreateGameObjects();
        GameMenu.Hide();
        Toolbox.Instance.LevelSpeed = LevelScrollSpeed;
        SetLevel();
    }
	
	void Update ()
    {
        Stats.TotalTime += Time.deltaTime;
        if (!bGameStarted || bGamePaused) { Stats.IdleTime += Time.deltaTime; return; }
        Stats.PlayTime += Time.deltaTime;

        if (LevelObjects.AtCaveEnd())
        {
            SetMovementForExit();
        }
    }

    private void SetMovementForExit()
    {
        bAtEnd = true;
        Background.SetVelocity(0);
    }

    public bool AtCaveEnd()
    {
        return bAtEnd;
    }

    void SetLevel()
    {
        int Level = Toolbox.Instance.Level;
        if (Level == 0)
        {
            Toolbox.Instance.Level = DefaultLevel;
            Level = DefaultLevel;
        }
        GameHUD.SetLevelText(Level);
        Toolbox.Instance.ShowLevelTooltips = (Stats.LevelData.IsCompleted(Level) ? false : true);
        LevelObjects.SetMode(bIsEndless: Level == -1 ? true : false);
    }

    void CreateGameObjects()
    {
        LevelObjects = LevelScripts.AddComponent<LevelObjectHandler>();
    }

    public void UpdateGameSpeed(float _gameSpeed)
    {
        GameSpeed = _gameSpeed;
        float Speed = GameSpeed * LevelScrollSpeed;
        
        Background.SetVelocity(Speed);
        LevelObjects.SetVelocity(Speed);
    }

    public void HorribleDeath()
    {
        //bGamePaused = true;
        GameSpeed = 0f;
        float Speed = GameSpeed * LevelScrollSpeed;

        Background.SetVelocity(Speed);
        LevelObjects.SetVelocity(Speed);

        GetComponent<AudioSource>().Stop();
    }

    public void StartGame()
    {
        bGameStarted = true;
        GameHUD.StartGame();
        UpdateGameSpeed(1);
        LevelObjects.SetPaused(false);
        LevelObjects.SetVelocity(LevelScrollSpeed);
    }

    public void PauseGame(bool ShowMenu = true)
    {
        // TODO Play pause sound
        bGamePaused = true;
        PrevGameSpeed = GameSpeed;
        UpdateGameSpeed(0);
        LevelObjects.SetPaused(true);
        GameHUD.GamePaused(true);

        if (ShowMenu)
        {
            GameMenu.PauseGame();
        }
        Stats.SaveStats();
    }

    public void ResumeGame()
    {
        // Play resume sound
        bGamePaused = false;
        UpdateGameSpeed(PrevGameSpeed);
        LevelObjects.SetPaused(false);
        GameHUD.GamePaused(false);
    }

    public void ShowGameoverMenu()
    {
        Stats.SaveStats();
        GameMenu.GameOver();
        GameHUD.GameOver();
    }

    public void AddDistance(double TimeTravelled, float PlayerSpeed)
    {
        float AddDist = (float)TimeTravelled * PlayerSpeed * Toolbox.Instance.LevelSpeed;
        Stats.Distance += AddDist;
        Stats.TotalDistance += AddDist;

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
