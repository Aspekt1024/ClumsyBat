using ClumsyBat.Objects;
using UnityEngine;

public class BossMoths : MonoBehaviour
{
    private MothPool _moths;
    
    private void OnEnable()
    {
        EventListener.OnPauseGame += PauseGame;
        EventListener.OnResumeGame += ResumeGame;
    }
    private void OnDisable()
    {
        EventListener.OnPauseGame -= PauseGame;
        EventListener.OnResumeGame -= ResumeGame;
    }

    private void Start ()
    {
		_moths = new MothPool();
    }

    private void PauseGame()
    {
        _moths.PauseGame(true);
    }

    private void ResumeGame()
    {
        _moths.PauseGame(false);
    }
}
