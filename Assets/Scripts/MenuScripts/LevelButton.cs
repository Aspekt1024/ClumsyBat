using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : MonoBehaviour {

    private Sprite AvailableImage;
    private Sprite CompletedImage;
    private Sprite UnavailableImage;

    private Image LevelImage = null;
    private RectTransform NamePanel = null;
    private Text LevelName = null;

    bool bAnimationReverse;
    float ButtonAnimationTimer;

    public enum LevelStates
    {
        Hidden,
        Disabled,
        Enabled,
        Completed
    }
    private LevelStates LevelState = LevelStates.Disabled;

    private int LevelNum;
    private bool bClicked = false;

    void Awake ()
    {
        foreach (RectTransform RT in GetComponent<RectTransform>())
        {
            if (RT.name == "NamePanel")
            {
                NamePanel = RT.GetComponent<RectTransform>();
            }
        }
        LevelName = NamePanel.GetComponentInChildren<Text>();
        LevelImage = GetComponent<Image>();
        LevelNum = int.Parse(name.Substring(2, name.Length - 2));
        GetLevelImages();
    }

	void Start ()
    {
        LevelName.enabled = false;
        NamePanel.GetComponent<Image>().enabled = false;
    }

    void Update()
    {
        if (!bClicked || (LevelState != LevelStates.Enabled && LevelState != LevelStates.Completed)) { return; }
        ButtonAnimationTimer += Time.deltaTime;
        UpdateClickedColour();
    }

    private void GetLevelImages()
    {
        AvailableImage = Resources.Load<Sprite>("LevelButtons/Level" + LevelNum + "Available");
        CompletedImage = Resources.Load<Sprite>("LevelButtons/Level" + LevelNum + "Complete");
        UnavailableImage = Resources.Load<Sprite>("LevelButtons/LevelUnavailable");

        if (AvailableImage == null)
        {
            AvailableImage = Resources.Load<Sprite>("LevelButtons/LevelAvailableNotFound");
        }
        if (CompletedImage == null)
        {
            CompletedImage = Resources.Load<Sprite>("LevelButtons/LevelCompleteNotFound");
        }
    }

    public bool Clicked()
    {
        if (LevelState == LevelStates.Disabled) { return false; }

        bool bLoadLevel = false;
        if (bClicked)
        {
            bLoadLevel = true;
        }
        else
        {
            NamePanel.GetComponent<Image>().enabled = true;
            SetClickedColour();
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
        LevelImage.color = new Color(1f, 1f, 1f);
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

    public void SetLevelState(LevelStates State)
    {
        LevelState = State;
        switch (LevelState)
        {
            case LevelStates.Hidden:
                gameObject.SetActive(false);
                break;
            case LevelStates.Disabled:
                LevelImage.sprite = UnavailableImage;
                break;
            case LevelStates.Enabled:
                LevelImage.sprite = AvailableImage;
                break;
            case LevelStates.Completed:
                LevelImage.sprite = CompletedImage;
                break;
        }
    }

    private void SetClickedColour()
    {
        switch (LevelState)
        {
            case LevelStates.Hidden:
                return;
            case LevelStates.Disabled:
                return;
            case LevelStates.Enabled:
                LevelImage.color = new Color(1f, 1f, 0.6f);
                break;
            case LevelStates.Completed:
                LevelImage.color = new Color(0.6f, 1f, 1f);
                break;
        }
    }

    private void UpdateClickedColour()
    {
        const float ButtonAnimationDuration = 1f;
        if (ButtonAnimationTimer > ButtonAnimationDuration)
        {
            ButtonAnimationTimer -= ButtonAnimationDuration;
            bAnimationReverse = !bAnimationReverse;
        }

        float Intensity = 0.4f * (ButtonAnimationTimer / ButtonAnimationDuration);
        if (bAnimationReverse)
        {
            Intensity += 0.6f + Intensity;
        }
        else
        {
            Intensity = 1 - Intensity;
        }

        switch (LevelState)
        {
            case LevelStates.Completed:
                LevelImage.color = new Color(Intensity, 1f, 1f);
                break;
            case LevelStates.Enabled:
                LevelImage.color = new Color(1f, 1f, Intensity);
                break;
        }
    }

    public void SetLevelName(string lvlName)
    {
        LevelName.text = lvlName;
        // TODO Setup LevelNames list in an accessible place... Stats?
    }
}
