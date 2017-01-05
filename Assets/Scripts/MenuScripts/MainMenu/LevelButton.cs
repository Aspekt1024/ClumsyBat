using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : MonoBehaviour {

    private Sprite AvailableImage;
    private Sprite CompletedImage;
    private Sprite AvailableClickedImage;
    private Sprite CompletedClickedImage;
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
        LevelName.text = Toolbox.Instance.LevelNames[LevelNum];
        LevelName.enabled = false;
        NamePanel.GetComponent<Image>().enabled = false;
    }

    void Update()
    {
        if (!bClicked || (LevelState != LevelStates.Enabled && LevelState != LevelStates.Completed)) { return; }
        ButtonAnimationTimer += Time.deltaTime;
    }

    private void GetLevelImages()
    {
        AvailableImage = Resources.Load<Sprite>("LevelButtons/Level" + LevelNum + "Available");
        AvailableClickedImage = Resources.Load<Sprite>("LevelButtons/Level" + LevelNum + "AvailableClicked");
        CompletedImage = Resources.Load<Sprite>("LevelButtons/Level" + LevelNum + "Completed");
        CompletedClickedImage = Resources.Load<Sprite>("LevelButtons/Level" + LevelNum + "CompletedClicked");
        UnavailableImage = Resources.Load<Sprite>("LevelButtons/LevelUnavailable");

        if (AvailableImage == null) { AvailableImage = Resources.Load<Sprite>("LevelButtons/LevelAvailableNotFound"); }
        if (AvailableClickedImage == null) { AvailableClickedImage = AvailableImage; }
        if (CompletedImage == null) { CompletedImage = Resources.Load<Sprite>("LevelButtons/LevelCompleteNotFound"); }
        if (CompletedClickedImage == null) { CompletedClickedImage = CompletedImage; }
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
            LevelName.enabled = true;
            bClicked = true;
            SetLevelImage();
        }
        return bLoadLevel;
    }

    public void Unclick()
    {
        bClicked = false;
        LevelName.enabled = false;
        NamePanel.GetComponent<Image>().enabled = false;
        SetLevelImage();
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
        SetLevelImage();
    }

    private void SetLevelImage()
    {
        switch (LevelState)
        {
            case LevelStates.Hidden:
                gameObject.SetActive(false);
                break;
            case LevelStates.Disabled:
                LevelImage.sprite = UnavailableImage;
                break;
            case LevelStates.Enabled:
                LevelImage.sprite = bClicked ? AvailableClickedImage : AvailableImage;
                break;
            case LevelStates.Completed:
                LevelImage.sprite = bClicked ? CompletedClickedImage : CompletedImage;
                break;
        }
    }

    private void UpdateClickedColour()
    {
        // No longer in use but kept for reference.
        // If this is still out of use in Feb 2016, delete it.
        const float IntensityDepth = 0.9f;
        const float ButtonAnimationDuration = 1f;
        if (ButtonAnimationTimer > ButtonAnimationDuration)
        {
            ButtonAnimationTimer -= ButtonAnimationDuration;
            bAnimationReverse = !bAnimationReverse;
        }

        float Intensity = IntensityDepth * (ButtonAnimationTimer / ButtonAnimationDuration);
        if (bAnimationReverse)
        {
            Intensity += (1 - IntensityDepth) + Intensity;
        }
        else
        {
            Intensity = 1 - Intensity;
        }

        switch (LevelState)
        {
            case LevelStates.Completed:
                LevelImage.color = new Color(1f, 1f, Intensity);
                break;
            case LevelStates.Enabled:
                LevelImage.color = new Color(1f, 1f, Intensity);
                break;
        }
    }
}
