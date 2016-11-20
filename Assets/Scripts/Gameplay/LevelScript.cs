using UnityEngine;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour {

    public int DefaultLevel = 1;

    // Game objects not created at runtime
    public ParralaxBG Background;
    
    // Game objects created at runtime
    private GameObject LevelScripts;
    private Cave Cave;

    // Gameplay attributes
    public const float LevelScrollSpeed = 4;    // first initialisation of LevelSpeed
    private bool bGameStarted = false;
    private bool bGamePaused = false;
    private float GameSpeed = 1f;
    private float PrevGameSpeed;

    public Text ScoreText;
    public Text HighScoreText;
    public Text TapToStartText;
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
            Cave.SetMode(bIsEndless: true);
        }
        else
        {
            Cave.SetMode(bIsEndless: false);
        }
        LevelText.text = "Level: " + LevelNum;
    }

    void CreateGameObjects()
    {
        Cave = LevelScripts.AddComponent<Cave>();
    }

    public void UpdateGameSpeed(float _gameSpeed)
    {
        GameSpeed = _gameSpeed;
        float Speed = GameSpeed * LevelScrollSpeed;
        
        Background.SetVelocity(Speed);
        Cave.SetVelocity(Speed);
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
        Cave.SetVelocity(LevelScrollSpeed);

        PauseButton.SetActive(true);
        Destroy(TapToStartText);
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
        Cave.DestroyOnScreenHazards();
    }
}
