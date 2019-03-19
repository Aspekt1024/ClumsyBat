using ClumsyBat;
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
    }
}
