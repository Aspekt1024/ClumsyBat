using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {

    private Text ResumeTimerText = null;

	void Start ()
    {
        GetTextObjects();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void GetTextObjects()
    {
        foreach (RectTransform RT in gameObject.GetComponent<RectTransform>())
        {
            switch (RT.name)
            {
                case "ResumeTimerText":
                    ResumeTimerText = RT.GetComponent<Text>();
                    break;
            }
        }
    }

    public void SetResumeTimer(float TimeRemaining)
    {
        if (ResumeTimerText.enabled == false)
        {
            ResumeTimerText.enabled = true;
        }
        ResumeTimerText.text = Mathf.Ceil(TimeRemaining).ToString();
    }

    public void HideResumeTimer()
    {
        ResumeTimerText.enabled = false;
    }
}
