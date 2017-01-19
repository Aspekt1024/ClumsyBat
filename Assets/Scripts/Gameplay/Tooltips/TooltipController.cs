using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipController : MonoBehaviour {

    private CanvasGroup _tooltipCanvas;
    private RectTransform _tooltipPanel;
    private Text _toolTipText;
    private RectTransform _toolTipTextRt;
    private Animator _tooltipAnimator;
    private RectTransform _nomee;
    private Image _nomeeImage;

    private Text _resumeNextText;
    private Image _resumeNextImage;
    private Image _resumePlayImage;
    private RectTransform _resumeNextRt;
    private RectTransform _resumePlayRt;

    private float _originalTooltipScale;
    private float _originalNomeeScale;
    private float _originalTooltipTextScale;

    private void Awake()
    {
        GetTooltipComponents();
        gameObject.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        ShowTooltipCanvas(false);
        _originalTooltipScale = _tooltipPanel.localScale.x;
        _originalTooltipTextScale = _toolTipTextRt.localScale.x;
        _originalNomeeScale = _nomee.localScale.x;
    }

    public void SetText(string toolText)
    {
        _toolTipText.text = toolText;
    }
    
    public IEnumerator OpenTooltip()
    {
        ShowTooltipCanvas(true);
        _toolTipText.enabled = false;
        _nomeeImage.enabled = false;
        _resumeNextText.enabled = false;
        _resumeNextImage.enabled = false;
        _resumePlayImage.enabled = false;
        _tooltipAnimator.Play("TooltipScrollClosed", 0, 0f);
        
        yield return StartCoroutine("PopOutObject", _tooltipPanel);
        
        _tooltipAnimator.Play("TooltipScrollOpen", 0, 0f);
        float animTimer = 0f;
        const float animDuration = 0.3f;

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            yield return null;
        }
        _nomeeImage.enabled = true;
        StartCoroutine("PopOutObject", _nomee);
    }

    public IEnumerator PopOutObject(RectTransform rt)
    {
        Vector2 originalScale = rt.localScale;

        float animTimer = 0f;
        float startScale = 0f;
        float endScale = 1.1f;
        float animDuration = 0.1f;
        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            rt.localScale = originalScale * (startScale - (startScale - endScale) * (animTimer / animDuration));
            yield return null;
        }

        startScale = 1.1f;
        endScale = 1f;
        animTimer = 0f;
        animDuration = 0.05f;
        while(animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            rt.localScale = originalScale * (startScale - (startScale - endScale) * (animTimer / animDuration));
            yield return null;
        }
        rt.localScale = originalScale;
    }

    public IEnumerator PopInObject(RectTransform rt)
    {
        Vector2 originalScale = rt.localScale;

        float animTimer = 0f;
        float startScale = 1f;
        float endScale = 1.1f;
        float animDuration = 0.05f;
        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            rt.localScale = originalScale * (startScale - (startScale - endScale) * (animTimer / animDuration));
            yield return null;
        }

        startScale = 1.1f;
        endScale = 0f;
        animTimer = 0f;
        animDuration = 0.1f;
        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            rt.localScale = originalScale * (startScale - (startScale - endScale) * (animTimer / animDuration));
            yield return null;
        }
        rt.localScale = originalScale;
    }

    public IEnumerator ShowText(bool show)
    {
        _resumeNextText.enabled = false;
        _resumeNextImage.enabled = false;
        _resumePlayImage.enabled = false;
        if (show)
        {
            _toolTipText.enabled = true;
            yield return StartCoroutine("PopOutObject", _toolTipTextRt);
        }
        else
        {
            yield return StartCoroutine("PopInObject", _toolTipTextRt);
            _toolTipText.enabled = false;
        }
    }
    
    public IEnumerator CloseTooltip()
    {
        _resumeNextText.enabled = false;
        _resumeNextImage.enabled = false;
        _resumePlayImage.enabled = false;
        StartCoroutine("PopInObject", _toolTipTextRt);
        yield return StartCoroutine("PopInObject", _nomee);

        _toolTipText.enabled = false;
        _nomeeImage.enabled = false;

        _tooltipAnimator.Play("TooltipScrollClose");

        float AnimTimer = 0f;
        const float AnimDuration = 0.2f;
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            yield return null;
        }

        yield return StartCoroutine("PopInObject", _tooltipPanel);
        ShowTooltipCanvas(false);
    }

    public void RestoreOriginalScale()
    {
        StopCoroutine("CloseTooltip");
        ShowTooltipCanvas(false);

        _tooltipPanel.localScale = Vector2.one * _originalTooltipScale;
        _toolTipTextRt.localScale = Vector2.one * _originalTooltipTextScale;
        _nomee.localScale = new Vector2(_originalNomeeScale, -_originalNomeeScale);
    }

    public void ShowTapToResume()
    {
        _resumeNextText.enabled = true;
        _resumeNextImage.enabled = true;
        StartCoroutine("PopOutObject", _resumeNextRt);
    }

    public void ShowTapToPlay()
    {
        _resumePlayImage.enabled = true;
        StartCoroutine("PopOutObject", _resumePlayRt);
    }

    public void HideResumeImages()
    {
        if (_resumeNextImage.enabled)
        {
            StartCoroutine("PopInObject", _resumeNextRt);
        }
        else if (_resumePlayImage.enabled)
        {
            StartCoroutine("PopInObject", _resumePlayRt);
        }
    }

    private void GetTooltipComponents()
    {
        foreach (RectTransform RT in GetComponent<RectTransform>())
        {
            switch (RT.name)
            {
                case "ToolTipPanel":
                    _tooltipPanel = RT;
                    _tooltipCanvas = RT.GetComponent<CanvasGroup>();
                    _tooltipAnimator = RT.GetComponent<Animator>();
                    break;
            }
        }

        foreach (RectTransform RT in _tooltipPanel)
        {
            switch (RT.name)
            {
                case "ToolTipTextBox":
                    _toolTipTextRt = RT;
                    _toolTipText = RT.GetComponent<Text>();
                    break;
                case "ResumeNextImage":
                    GetResumeNextComponents(RT);
                    break;
                case "ResumePlayImage":
                    _resumePlayRt = RT;
                    _resumePlayImage = RT.GetComponent<Image>();
                    break;
                case "NomeeWindow":
                    GetNomee(RT);
                    break;
            }
        }
    }

    private void GetNomee(RectTransform NomeeWindow)
    {
        foreach(RectTransform RT in NomeeWindow)
        {
            if (RT.name == "Nomee")
            {
                _nomee = RT;
                _nomeeImage = RT.GetComponent<Image>();
            }
        }
    }

    private void GetResumeNextComponents(RectTransform ResumeRT)
    {
        _resumeNextRt = ResumeRT;
        _resumeNextImage = _resumeNextRt.GetComponent<Image>();
        foreach (RectTransform RT in _resumeNextRt)
        {
            if (RT.name == "ResumeNextText")
            {
                _resumeNextText = RT.GetComponent<Text>();
            }
        }
    }
    
    public void ShowTooltipCanvas(bool Show)
    {
        _tooltipCanvas.interactable = Show;
        _tooltipCanvas.blocksRaycasts = Show;
        _tooltipCanvas.alpha = (Show ? 1f : 0f);
    }
}
