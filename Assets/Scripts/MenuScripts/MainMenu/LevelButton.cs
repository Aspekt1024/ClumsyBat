using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public LevelProgressionHandler.Levels Level;
    public bool Star1Complete;
    public bool Star2Complete;
    public bool Star3Complete;
    public bool StarsSet;
    private bool starImagesSet;

    private Sprite _availableImage;
    private Sprite _completedImage;
    private Sprite _availableClickedImage;
    private Sprite _completedClickedImage;
    private Sprite _unavailableImage;

    private Image _levelImage;
    private RectTransform namePanel;
    private Text levelName;

    private RectTransform starsRt;
    private MothStar[] stars = new MothStar[3];
    
    private enum BtnState
    {
        Unclicked, Clicked, LoadLevel
    }
    private BtnState _state;

    public enum LevelStates
    {
        Hidden,
        Disabled,
        Enabled,
        Completed
    }
    private LevelStates _levelState = LevelStates.Disabled;

    private void Awake ()
    {
        foreach (RectTransform rt in GetComponent<RectTransform>())
        {
            if (rt.name == "Stars")
                GetStarComponents(rt);
        }
        namePanel = GameObject.Find("LevelID").GetComponent<RectTransform>();
        levelName = namePanel.GetComponentInChildren<Text>();
        _levelImage = GetComponent<Image>();
        GetLevelImages();
    }

	private void Start ()
    {
        starsRt.gameObject.SetActive(false);
        _state = BtnState.Unclicked;
    }

    private void Update()
    {
        if (!starImagesSet && StarsSet)
            SetStarCompletion();
    }

    private void GetLevelImages()
    {
        _availableImage = Resources.Load<Sprite>("LevelButtons/" + Level + "Available");
        _availableClickedImage = Resources.Load<Sprite>("LevelButtons/" + Level + "AvailableClicked");
        _completedImage = Resources.Load<Sprite>("LevelButtons/" + Level + "Completed");
        _completedClickedImage = Resources.Load<Sprite>("LevelButtons/" + Level + "CompletedClicked");
        _unavailableImage = Resources.Load<Sprite>("LevelButtons/LevelUnavailable");

        if (_availableImage == null) { _availableImage = Resources.Load<Sprite>("LevelButtons/LevelAvailableNotFound"); }
        if (_availableClickedImage == null) { _availableClickedImage = Resources.Load<Sprite>("LevelButtons/LevelAvailableClickedNotFound"); }
        if (_completedImage == null) { _completedImage = Resources.Load<Sprite>("LevelButtons/LevelCompleteNotFound"); }
        if (_completedClickedImage == null) { _completedClickedImage = Resources.Load<Sprite>("LevelButtons/LevelCompleteClickedNotFound"); }
    }
    
    public void Click(LevelProgressionHandler.Levels levelId)
    {
        if (levelId == Level)
        {
            if (_state == BtnState.Unclicked)
            {
                levelName.text = Toolbox.Instance.LevelNames[Level];
                levelName.enabled = true;
                _state = BtnState.Clicked;
                SetLevelImage();
            }
            else if (_state == BtnState.Clicked)
            {
                _state = BtnState.LoadLevel;
            }
        }
        else
        {
            _state = BtnState.Unclicked;
            SetLevelImage();
        }
    }

    public bool IsDoubleClicked(LevelProgressionHandler.Levels levelId)
    {
        if (levelId != Level) return false;
        return _state == BtnState.LoadLevel;
    }
    
    public bool LevelAvailable()
    {
        return _levelState == LevelStates.Completed || _levelState == LevelStates.Enabled;
    }

    public void SetLevelState(LevelStates state)
    {
        _levelState = state;
        SetLevelImage();
    }

    private void SetLevelImage()
    {
        switch (_levelState)
        {
            case LevelStates.Hidden:
                gameObject.SetActive(false);
                break;
            case LevelStates.Disabled:
                _levelImage.sprite = _unavailableImage;
                break;
            case LevelStates.Enabled:
                _levelImage.sprite = _state == BtnState.Clicked ? _availableClickedImage : _availableImage;
                break;
            case LevelStates.Completed:
                _levelImage.sprite = _state == BtnState.Clicked ? _completedClickedImage : _completedImage;
                break;
        }
    }

    private void GetStarComponents(RectTransform starsRectTransform)
    {
        starsRt = starsRectTransform;
        foreach (RectTransform rt in starsRectTransform)
        {
            if (rt.name == "Star1")
                stars[0] = rt.GetComponent<MothStar>();
            else if (rt.name == "Star2")
                stars[1] = rt.GetComponent<MothStar>();
            else if (rt.name == "Star3")
                stars[2] = rt.GetComponent<MothStar>();
        }
    }

    private void SetStarCompletion()
    {
        starsRt.gameObject.SetActive(true);

        starImagesSet = true;
        if (stars[0] != null) {
            if (Star1Complete) stars[0].SetActive(); else stars[0].SetInactive();
        }
        if (stars[0] != null) {
            if (Star2Complete) stars[1].SetActive(); else stars[1].SetInactive();
        }
        if (stars[0] != null) {
            if (Star3Complete) stars[2].SetActive(); else stars[2].SetInactive();
        }
    }
}
