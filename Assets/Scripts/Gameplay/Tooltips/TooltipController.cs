using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipController : MonoBehaviour {

    private CanvasGroup TooltipCanvas = null;
    private RectTransform TooltipPanel = null;
    private Text ToolTipText = null;
    private RectTransform ToolTipTextRT = null;
    private Animator TooltipAnimator = null;
    private RectTransform Nomee = null;
    private Image NomeeImage = null;

    private Text ResumeNextText = null;
    private Image ResumeNextImage = null;
    private Image ResumePlayImage = null;
    private RectTransform ResumeNextRT = null;
    private RectTransform ResumePlayRT = null;

    private float TooltipStartY;

    void Awake()
    {
        GetTooltipComponents();
        gameObject.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        TooltipStartY = TooltipPanel.position.y;
        ShowTooltipCanvas(false);
    }

    public void SetText(string ToolText)
    {
        ToolTipText.text = ToolText;
    }
    
    public IEnumerator OpenTooltip()
    {
        ShowTooltipCanvas(true);
        ToolTipText.enabled = false;
        NomeeImage.enabled = false;
        ResumeNextText.enabled = false;
        ResumeNextImage.enabled = false;
        ResumePlayImage.enabled = false;
        TooltipAnimator.Play("TooltipScrollClosed", 0, 0f);
        
        yield return StartCoroutine("PopOutObject", TooltipPanel);
        
        TooltipAnimator.Play("TooltipScrollOpen", 0, 0f);
        float AnimTimer = 0f;
        const float AnimDuration = 0.3f;

        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            yield return null;
        }
        NomeeImage.enabled = true;
        StartCoroutine("PopOutObject", Nomee);
    }

    public IEnumerator PopOutObject(RectTransform RT)
    {
        Vector2 OriginalScale = RT.localScale;

        float AnimTimer = 0f;
        float StartScale = 0f;
        float EndScale = 1.1f;
        float AnimDuration = 0.1f;
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            RT.localScale = OriginalScale * (StartScale - (StartScale - EndScale) * (AnimTimer / AnimDuration));
            yield return null;
        }

        StartScale = 1.1f;
        EndScale = 1f;
        AnimTimer = 0f;
        AnimDuration = 0.05f;
        while(AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            RT.localScale = OriginalScale * (StartScale - (StartScale - EndScale) * (AnimTimer / AnimDuration));
            yield return null;
        }
        RT.localScale = OriginalScale;
    }

    public IEnumerator PopInObject(RectTransform RT)
    {
        Vector2 OriginalScale = RT.localScale;

        float AnimTimer = 0f;
        float StartScale = 1f;
        float EndScale = 1.1f;
        float AnimDuration = 0.05f;
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            RT.localScale = OriginalScale * (StartScale - (StartScale - EndScale) * (AnimTimer / AnimDuration));
            yield return null;
        }

        StartScale = 1.1f;
        EndScale = 0f;
        AnimTimer = 0f;
        AnimDuration = 0.1f;
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            RT.localScale = OriginalScale * (StartScale - (StartScale - EndScale) * (AnimTimer / AnimDuration));
            yield return null;
        }
        RT.localScale = OriginalScale;
    }

    public IEnumerator ShowText(bool Show)
    {
        ToolTipText.enabled = true;
        ResumeNextText.enabled = false;
        ResumeNextImage.enabled = false;
        ResumePlayImage.enabled = false;
        if (Show)
        {
            yield return StartCoroutine("PopOutObject", ToolTipTextRT);
        }
        else
        {
            yield return StartCoroutine("PopInObject", ToolTipTextRT);
        }
    }
    
    private IEnumerator CloseTooltip()
    {
        ResumeNextText.enabled = false;
        ResumeNextImage.enabled = false;
        ResumePlayImage.enabled = false;
        StartCoroutine("PopInObject", ToolTipTextRT);
        yield return StartCoroutine("PopInObject", Nomee);

        ToolTipText.enabled = false;
        NomeeImage.enabled = false;

        TooltipAnimator.Play("TooltipScrollClose");

        float AnimTimer = 0f;
        const float AnimDuration = 0.2f;
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            yield return null;
        }

        yield return StartCoroutine("PopInObject", TooltipPanel);
        ShowTooltipCanvas(false);
    }

    public void ShowTapToResume()
    {
        ResumeNextText.enabled = true;
        ResumeNextImage.enabled = true;
        StartCoroutine("PopOutObject", ResumeNextRT);
    }

    public void ShowTapToPlay()
    {
        ResumePlayImage.enabled = true;
        StartCoroutine("PopOutObject", ResumePlayRT);
    }

    public void HideResumeImages()
    {
        if (ResumeNextImage.enabled)
        {
            StartCoroutine("PopInObject", ResumeNextRT);
        }
        else if (ResumePlayImage.enabled)
        {
            StartCoroutine("PopInObject", ResumePlayRT);
        }
    }

    private void GetTooltipComponents()
    {
        foreach (RectTransform RT in GetComponent<RectTransform>())
        {
            switch (RT.name)
            {
                case "ToolTipPanel":
                    TooltipPanel = RT;
                    TooltipCanvas = RT.GetComponent<CanvasGroup>();
                    TooltipAnimator = RT.GetComponent<Animator>();
                    break;
            }
        }

        foreach (RectTransform RT in TooltipPanel)
        {
            switch (RT.name)
            {
                case "ToolTipTextBox":
                    ToolTipTextRT = RT;
                    ToolTipText = RT.GetComponent<Text>();
                    break;
                case "ResumeNextImage":
                    GetResumeNextComponents(RT);
                    break;
                case "ResumePlayImage":
                    ResumePlayRT = RT;
                    ResumePlayImage = RT.GetComponent<Image>();
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
                Nomee = RT;
                NomeeImage = RT.GetComponent<Image>();
            }
        }
    }

    private void GetResumeNextComponents(RectTransform ResumeRT)
    {
        ResumeNextRT = ResumeRT;
        ResumeNextImage = ResumeNextRT.GetComponent<Image>();
        foreach (RectTransform RT in ResumeNextRT)
        {
            if (RT.name == "ResumeNextText")
            {
                ResumeNextText = RT.GetComponent<Text>();
            }
        }
    }
    
    public void ShowTooltipCanvas(bool Show)
    {
        TooltipCanvas.interactable = Show;
        TooltipCanvas.blocksRaycasts = Show;
        TooltipCanvas.alpha = (Show ? 1f : 0f);
    }
}
