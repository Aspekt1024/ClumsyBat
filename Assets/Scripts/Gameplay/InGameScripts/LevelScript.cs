using ClumsyBat.Managers;
using UnityEngine;

public class LevelScript : MonoBehaviour {

    // These attributes can be set in the inspector
    public float ClumsyBaseSpeed = 5f;    // TODO should this really belong with player?
    public LevelProgressionHandler.Levels DefaultLevel = LevelProgressionHandler.Levels.Main1;
    
    [HideInInspector]
    public GameUI GameHud;

    private GameObject _levelScripts;
    private AudioSource _audioControl;

    private int scoreToBeat;

    // Gameplay attributes
    private bool _bGameStarted;
    private bool _bGamePaused;
    private bool _bAtEnd;

    private void Awake()
    {
        _levelScripts = GameObject.Find("LevelScripts");
        _audioControl = _levelScripts.AddComponent<AudioSource>();
        GameHud = GameObject.Find("UI_Overlay").GetComponent<GameUI>();
    }

    private void Start ()
    {
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
        LevelManager.Instance.ObjectHandler.SetMode(bIsEndless: level == LevelProgressionHandler.Levels.Endless);
    }
    
    public void StartGame()
    {
        _bGameStarted = true;
        GameHud.StartGame();
        LevelManager.Instance.ObjectHandler.SetPaused(false);
    }

    public void PauseGame()
    {
        // TODO Play pause sound
        _bGamePaused = true;
        LevelManager.Instance.ObjectHandler.SetPaused(true);
        GameHud.GamePaused(true);
        
        GameData.Instance.Data.SaveData();
    }

    public void ResumeGame()
    {
        // Play resume sound
        _bGamePaused = false;
        LevelManager.Instance.ObjectHandler.SetPaused(false);
        GameHud.GamePaused(false);
    }

    public void ShowGameoverMenu()
    {
        GameData.Instance.Data.SaveData();
        GameHud.GameOver();
    }

    public void LevelWon()
    {
        if (Toolbox.Player.ExitViaSecretPath)
        {
            GameData.Instance.SetLevelCompletion(GameData.LevelCompletePaths.Secret1);
            GameData.Instance.NextLevel = LevelProgressionHandler.GetSecretLevel1(GameData.Instance.Level);
        }
        else
        {
            GameData.Instance.SetLevelCompletion(GameData.LevelCompletePaths.MainPath);
            GameData.Instance.NextLevel = LevelProgressionHandler.GetNextLevel(GameData.Instance.Level);
        }
        EventListener.LevelWon();
        GameHud.LevelWon();

        // TODO add sound to sound controller script
        if (GameData.Instance.Data.Stats.Settings.Music)
        {
            var victoryClip = (AudioClip)Resources.Load("Audio/LevelComplete");
            _audioControl.PlayOneShot(victoryClip);
        }
    }
}
