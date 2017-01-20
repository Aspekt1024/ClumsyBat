using UnityEngine;
using System.Collections;

public class BossGameHandler : GameHandler {

    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;
    private GameUI _gameHud;

    private void Start()
    {
        _loadScreen = FindObjectOfType<LoadScreen>();
        _gameHud = FindObjectOfType<GameUI>();
        _gameMenu = FindObjectOfType<GameMenuOverlay>();
        _gameMenu.Hide();
        ThePlayer.Fog.Disable();
        StartCoroutine("LoadSequence");
    }
	
	private void Update ()
	{
	    KeepClumsyOnScreen();
	}

    private void KeepClumsyOnScreen()
    {
        const float minY = -2f;
        const float maxY = 4f;
        if (ThePlayer.transform.position.y < minY)
        {
            ThePlayer.transform.position -= new Vector3(0f, ThePlayer.transform.position.y - minY, 0f);
        }
        if (ThePlayer.transform.position.y > maxY)
        {
            ThePlayer.transform.position -= new Vector3(0f, ThePlayer.transform.position.y - maxY);
        }
    }
    
    private IEnumerator LoadSequence()
    {
        yield return new WaitForSeconds(1f);
        _loadScreen.HideLoadScreen();
        PlayerController.EnterGamePlay();
    }

    public override void StartGame()
    {
        Debug.Log("Game Started");
        _gameHud.StartGame();
    }

    public override void PauseGame(bool showMenu)
    {
    }

    public override void ResumeGame(bool immediate = false)
    {
    }

    public override void UpdateGameSpeed(float speed)
    {
    }

    public override void LevelComplete()
    {
    }
}
