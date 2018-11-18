using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public LevelProgressionHandler.Levels Level;
    public RectTransform PreviousLevel;
    [HideInInspector] public bool Star1Complete;
    [HideInInspector] public bool Star2Complete;
    [HideInInspector] public bool Star3Complete;
    [HideInInspector] public bool StarsSet;

    private bool starImagesSet;

    private Sprite _availableImage;
    private Sprite _completedImage;
    private Sprite _availableClickedImage;
    private Sprite _completedClickedImage;
    private Sprite _unavailableImage;

    private Image _levelImage;
    private RectTransform starsRt;
    private MothStar[] stars = new MothStar[3];
    
    private enum BtnState
    {
        Unclicked, Clicked
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
        if (Level.ToString().Contains("BossS"))
        {
            // TODO shrine level images
            _availableImage = Resources.Load<Sprite>("LevelButtons/Boss2Available");
            _availableClickedImage = Resources.Load<Sprite>("LevelButtons/Boss2AvailableClicked");
            _completedImage = Resources.Load<Sprite>("LevelButtons/Boss2Completed");
            _completedClickedImage = Resources.Load<Sprite>("LevelButtons/Boss2CompletedClicked");
        }
        else if (Level.ToString().Contains("Boss"))
        {
            _availableImage = Resources.Load<Sprite>("LevelButtons/Boss2Available");
            _availableClickedImage = Resources.Load<Sprite>("LevelButtons/Boss2AvailableClicked");
            _completedImage = Resources.Load<Sprite>("LevelButtons/Boss2Completed");
            _completedClickedImage = Resources.Load<Sprite>("LevelButtons/Boss2CompletedClicked");
        }
        else
        {
            _availableImage = Resources.Load<Sprite>("LevelButtons/" + Level + "Available");
            _availableClickedImage = Resources.Load<Sprite>("LevelButtons/" + Level + "AvailableClicked");
            _completedImage = Resources.Load<Sprite>("LevelButtons/" + Level + "Completed");
            _completedClickedImage = Resources.Load<Sprite>("LevelButtons/" + Level + "CompletedClicked");
        }
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
            if (_state == BtnState.Clicked) return;
            _state = BtnState.Clicked;
            SetLevelImage();
        }
        else
        {
            _state = BtnState.Unclicked;
            SetLevelImage();
        }
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

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        LevelButton[] buttons = transform.parent.GetComponentsInChildren<LevelButton>();

        foreach (LevelButton button in buttons)
        {
            if (button.PreviousLevel != null)
            {
                Vector2 prevPos = button.PreviousLevel.position;
                Vector2 thisPos = button.transform.position;
                
                Gizmos.color = new Color(0.4f, 1f, 1f);
                Gizmos.DrawLine(prevPos, thisPos);
            }
        }
    }
#endif
}
