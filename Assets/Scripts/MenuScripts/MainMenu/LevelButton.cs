using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public LevelProgressionHandler.Levels Level;

    private Sprite _availableImage;
    private Sprite _completedImage;
    private Sprite _availableClickedImage;
    private Sprite _completedClickedImage;
    private Sprite _unavailableImage;

    private Image _levelImage;
    private RectTransform _namePanel;
    private Text _levelName;
    
    private float _buttonAnimationTimer;

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
            if (rt.name == "NamePanel")
                _namePanel = rt.GetComponent<RectTransform>();
        }
        _levelName = _namePanel.GetComponentInChildren<Text>();
        _levelImage = GetComponent<Image>();
        GetLevelImages();
    }

	private void Start ()
    {
        _levelName.text = Toolbox.Instance.LevelNames[Level];
        _levelName.enabled = false;
        _namePanel.GetComponent<Image>().enabled = false;
        _state = BtnState.Unclicked;
    }

    private void Update()
    {
        // TODO decide if we want any animation for the buttons... else remove this function
        if (_state != BtnState.Clicked || (_levelState != LevelStates.Enabled && _levelState != LevelStates.Completed)) { return; }
        _buttonAnimationTimer += Time.deltaTime;
    }

    private void GetLevelImages()
    {
        _availableImage = Resources.Load<Sprite>("LevelButtons/" + Level + "Available");
        _availableClickedImage = Resources.Load<Sprite>("LevelButtons/" + Level + "AvailableClicked");
        _completedImage = Resources.Load<Sprite>("LevelButtons/" + Level + "Completed");
        _completedClickedImage = Resources.Load<Sprite>("LevelButtons/" + Level + "CompletedClicked");
        _unavailableImage = Resources.Load<Sprite>("LevelButtons/LevelUnavailable");

        if (_availableImage == null) { _availableImage = Resources.Load<Sprite>("LevelButtons/LevelAvailableNotFound"); }
        if (_availableClickedImage == null) { _availableClickedImage = _availableImage; }
        if (_completedImage == null) { _completedImage = Resources.Load<Sprite>("LevelButtons/LevelCompleteNotFound"); }
        if (_completedClickedImage == null) { _completedClickedImage = _completedImage; }
    }

    public void Click(LevelProgressionHandler.Levels levelId)
    {
        if (levelId == Level)
        {
            if (_state == BtnState.Unclicked)
            {
                _namePanel.GetComponent<Image>().enabled = true;
                _levelName.enabled = true;
                SetLevelImage();
                _state = BtnState.Clicked;
            }
            else if (_state == BtnState.Clicked)
            {
                _state = BtnState.LoadLevel;
            }
        }
        else
        {
            _levelName.enabled = false;
            _namePanel.GetComponent<Image>().enabled = false;
            SetLevelImage();
            _state = BtnState.Unclicked;
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
}
