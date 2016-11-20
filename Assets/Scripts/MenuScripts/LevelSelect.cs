using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelect : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    private void LoadLevel(int LevelNum)
    {
        Toolbox.Instance.Level = LevelNum;
        SceneManager.LoadScene("Levels");
    }

    public void Lv1BtnClick() { LoadLevel(1); }
    public void Lv2BtnClick() { LoadLevel(2); }
    public void Lv3BtnClick() { LoadLevel(3); }
    public void Lv4BtnClick() { LoadLevel(4); }
    public void Lv5BtnClick() { LoadLevel(5); }
    public void Lv6BtnClick() { LoadLevel(6); }
    public void Lv7BtnClick() { LoadLevel(7); }
    public void Lv8BtnClick() { LoadLevel(8); }
    public void Lv9BtnClick() { LoadLevel(9); }
}
