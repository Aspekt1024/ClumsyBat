using UnityEngine;
using System.Collections;

public class TrainingGameHandler : GameHandler {

    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;

    private void Start()
    {
        _loadScreen = FindObjectOfType<LoadScreen>();
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
        const float MinY = -2f;
        const float MaxY = 4f;
        if (ThePlayer.transform.position.y < MinY)
        {
            ThePlayer.transform.position -= new Vector3(0f, ThePlayer.transform.position.y - MinY, 0f);
        }
        if (ThePlayer.transform.position.y > MaxY)
        {
            ThePlayer.transform.position -= new Vector3(0f, ThePlayer.transform.position.y - MaxY);
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
