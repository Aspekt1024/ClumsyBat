using UnityEngine;
using System.Collections;

public class TrainingScript : MonoBehaviour
{
    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;

	void Start ()
    {
        StartCoroutine("LoadSequence");
        _loadScreen = FindObjectOfType<LoadScreen>();
        _gameMenu = FindObjectOfType<GameMenuOverlay>();
        _gameMenu.Hide();
    }
	
	void Update ()
    {
	    
	}

    private IEnumerator LoadSequence()
    {
        yield return new WaitForSeconds(1f);
        _loadScreen.HideLoadScreen();
    }
}
