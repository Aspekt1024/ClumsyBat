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

    public Text ScoreText;
    public Text HighScoreText;
    public Text PauseText;
    public Text LevelText;

    public GameObject GameoverMenu;
    public GameObject PauseButton;      // TODO replace with bottom panel
    public GameObject ResumeButton;     // TODO replace

    public StatsHandler Stats;

    void Awake()
    {
        LevelScripts = new GameObject("Level Scripts");
        Stats = LevelScripts.AddComponent<StatsHandler>();
    }

    void Start ()
    {
        CreateGameObjects();
        SetupUIElements();
        Toolbox.Instance.LevelSpeed = LevelScrollSpeed;
        HighScoreText.text = "Best Distance: " + ((int)Stats.BestDistance).ToString() + "m";
        SetLevel();
    }
	
	void Update ()
    {
        Stats.TotalTime += Time.deltaTime;
        if (!bGameStarted || bGamePaused) { Stats.IdleTime += Time.deltaTime; return; }
        Stats.PlayTime += Time.deltaTime;
        ScoreText.text = Mathf.FloorToInt(Stats.Distance).ToString() + "m";

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

        string LevelNum = Level.ToString();
        if (Level == -1)
        {
            LevelNum = "Endless";
            LevelObjects.SetMode(bIsEndless: true);
        }
        else
        {
            LevelObjects.SetMode(bIsEndless: false);
        }
        LevelText.text = "Level: " + LevelNum;
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
        if (Speed == 0)
        {
            LevelObjects.SetPaused(true);
        }
        else
        {
            LevelObjects.SetPaused(false);
        }
    }

    public void HorribleDeath()
    {
        // Stop all objects in the level moving and bring up the gameover screen
        // TODO move Gameover overlay handler here
        bGamePaused = true;
        UpdateGameSpeed(0);
        GetComponent<AudioSource>().Stop();
    }

    public void StartGame()
    {
        bGameStarted = true;
        UpdateGameSpeed(1);
        LevelObjects.SetVelocity(LevelScrollSpeed);

        PauseButton.SetActive(true);
    }

    public void PauseGame()
    {
        // TODO Play pause sound
        bGamePaused = true;
        PrevGameSpeed = GameSpeed;
        UpdateGameSpeed(0);

        // TODO Replace with delegates
        ResumeButton.SetActive(true);
        PauseButton.SetActive(false);
        PauseText.enabled = true;
        PauseText.text = "Game Paused";
        Stats.SaveStats();
    }

    public void ResumeGame()
    {
        // Play resume sound
        bGamePaused = false;
        UpdateGameSpeed(PrevGameSpeed);

        ResumeButton.SetActive(false);
        PauseButton.SetActive(true);
    }

    void OnGUI()
    {
        //Debug.Log("Something");
    }

    public void ShowGameoverMenu()
    {
        Stats.SaveStats();
        GameoverMenu.SetActive(true);

        // Hide any other unnecessary UI elements
        PauseButton.SetActive(false);
        ResumeButton.SetActive(false);
    }

    public void AddDistance(double TimeTravelled, float PlayerSpeed, bool bIsDashing)
    {
        float AddDist = (float)TimeTravelled * PlayerSpeed * Toolbox.Instance.LevelSpeed;
        if (bIsDashing)
        {
            Stats.DashDistance += AddDist;
        }
        Stats.Distance += AddDist;
        Stats.TotalDistance += AddDist;
        ScoreText.text = (int)Stats.Distance + "m";

        if (Stats.Distance > Stats.BestDistance)
        {
            Stats.BestDistance = Stats.Distance;
            HighScoreText.text = "Best Distance: " + (int)Stats.Distance + "m";
        }
    }

    void SetupUIElements()
    {
        ResumeButton.SetActive(false);
        PauseButton.SetActive(false);
        GameoverMenu.SetActive(false);
        PauseText.enabled = false;
    }

    public void DestroyOnScreenEvils()
    {
        LevelObjects.DestroyOnScreenHazards();
    }

    public void LevelWon()
    {
        Stats.LevelWon(Toolbox.Instance.Level);
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.LevelSelect;
        Stats.SaveStats();
        SceneManager.LoadScene("Play");
    }
}
