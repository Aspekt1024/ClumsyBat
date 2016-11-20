using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameoverOverlay : MonoBehaviour {

    private StatsHandler Stats;
	// Use this for initialization
	void Start () {
        Stats = FindObjectOfType<StatsHandler>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MenuButtonPressed()
    {
        Stats.SaveStats();
        SceneManager.LoadScene("Play");
    }

    public void PlayAgainButtonPressed()
    {
        Stats.SaveStats();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
