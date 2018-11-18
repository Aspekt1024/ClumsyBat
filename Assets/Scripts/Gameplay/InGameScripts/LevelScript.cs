using ClumsyBat;
using ClumsyBat.LevelManagement;
using UnityEngine;

using LevelCompletionPaths = ClumsyBat.LevelManagement.LevelCompletionHandler.LevelCompletionPaths;

public class LevelScript : MonoBehaviour {

    // These attributes can be set in the inspector
    public float ClumsyBaseSpeed = 5f;    // TODO should this really belong with player?
    public LevelProgressionHandler.Levels DefaultLevel = LevelProgressionHandler.Levels.Main1;
    public LevelStateHandler statsHandler;
    
    private GameObject _levelScripts;
    private AudioSource _audioControl;
    
    private void Awake()
    {
        _levelScripts = GameObject.Find("LevelScripts");
        _audioControl = _levelScripts.AddComponent<AudioSource>();
        statsHandler = new LevelStateHandler();
    }
    
    private void Update ()
    {
        statsHandler.Tick(Time.deltaTime);
    }

    public void SetLevel()
    {
        var level = GameStatics.LevelManager.Level;
        if (level == LevelProgressionHandler.Levels.Unassigned)
        {
            GameStatics.LevelManager.Level = DefaultLevel;
            level = DefaultLevel;
        }
        GameStatics.UI.GameHud.SetLevelText(level);
        Toolbox.Instance.ShowLevelTooltips = (!GameStatics.Data.LevelDataHandler.IsCompleted(level));
    }
    
    public void StartGame()
    {
        statsHandler.Begin();

        GameStatics.UI.GameHud.StartGame();
        GameStatics.Objects.ObjectHandler.SetPaused(false);

    }
    
    public void ShowGameoverMenu()
    {
        GameStatics.Data.SaveData();
        GameStatics.UI.DropdownMenu.ShowGameOverMenu();
    }

    public void LevelWon(bool viaSecretPath)
    {
        if (viaSecretPath)
        {
            GameStatics.LevelManager.LevelCompleted(LevelCompletionPaths.Secret1);
        }
        else
        {
            GameStatics.LevelManager.LevelCompleted(LevelCompletionPaths.MainPath);
        }

        // TODO add sound to sound controller script
        if (GameStatics.Data.Settings.MusicOn)
        {
            var victoryClip = (AudioClip)Resources.Load("Audio/LevelComplete");
            _audioControl.PlayOneShot(victoryClip);
        }
    }


}
