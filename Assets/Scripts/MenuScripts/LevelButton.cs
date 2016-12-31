using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : MonoBehaviour {

    private RectTransform NamePanel = null;
    private Text LevelName = null;
    public enum LevelStates
    {
        Disabled,
        Enabled,
        Completed
    }
    public LevelStates LevelState;

    private bool bClicked = false;

	void Start ()
    {
        foreach (RectTransform RT in GetComponent<RectTransform>())
        {
            if (RT.name == "NamePanel")
            {
                NamePanel = RT.GetComponent<RectTransform>();
            }
        }
        LevelName = NamePanel.GetComponentInChildren<Text>();

        LevelName.enabled = false;
        NamePanel.GetComponent<Image>().enabled = false;
    }

    public bool Clicked()
    {
        bool bLoadLevel = false;
        if (bClicked)
        {
            bLoadLevel = true;
        }
        else
        {
            NamePanel.GetComponent<Image>().enabled = true;
            LevelName.enabled = true;
            bClicked = true;
        }
        return bLoadLevel;
    }

    public void Unclick()
    {
        bClicked = false;
        LevelName.enabled = false;
        NamePanel.GetComponent<Image>().enabled = false;
    }

    public bool LevelAvailable()
    {
        bool Available = false;
        if (LevelState == LevelStates.Completed || LevelState == LevelStates.Enabled)
        {
            Available = true;
        }
        return Available;
    }
}
