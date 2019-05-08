using ClumsyBat;
using ClumsyBat.DataManagement;
using ClumsyBat.LevelManagement;
using UnityEngine;

using LevelCompletionPaths = ClumsyBat.LevelManagement.LevelCompletionHandler.LevelCompletionPaths;

public class LevelScript : MonoBehaviour {

    public BossHandler BossHandler;
    public LevelStateHandler stateHandler;
    
    private void Awake()
    {
        stateHandler = new LevelStateHandler();
    }
    
    private void Update ()
    {
        stateHandler.Tick(Time.deltaTime);
    }
    
    public void StartGame()
    {
        stateHandler.Begin();

        GameStatics.UI.GameHud.StartGame();

    }
    
    public void ShowGameoverMenu()
    {
        GameStatics.Data.SaveData();
        GameStatics.UI.DropdownMenu.ShowGameOverMenu();
    }

    /// <summary>
    /// Called by LevelGameHandler.LevelComplete
    /// </summary>
    public LevelData LevelWon(bool viaSecretPath)
    {
        if (viaSecretPath)
        {
            return GameStatics.LevelManager.LevelCompleted(LevelCompletionPaths.Secret1);
        }
        else
        {
            return GameStatics.LevelManager.LevelCompleted(LevelCompletionPaths.MainPath);
        }
    }
}
