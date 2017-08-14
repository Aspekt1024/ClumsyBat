using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TooltipHandler : MonoBehaviour {
    
    public RectTransform TooltipButton;
    [HideInInspector] public bool IsPausedForTooltip;

    private TriggerEvent storedEvent;

    private Canvas tooltipOverlay;
    private Image tooltipBackground;
    private RectTransform tooltipScroll;
    private Animator tooltipScrollAnimator;
    private Text tooltipText;
    private RectTransform tooltipTextRt;
    private RectTransform resumeNextImage;
    private RectTransform resumePlayImage;
    private RectTransform nomee;

    private PlayerController _playerControl;
    private InputManager _inputManager;
    private TooltipButtonEffects buttonEffects;

    private bool tooltipSetup;
    
    private void OnEnable()
    {
        EventListener.OnDeath += RemoveTooltips;
        EventListener.OnLevelWon += RemoveTooltips;
    }
    private void OnDisable()
    {
        EventListener.OnDeath -= RemoveTooltips;
        EventListener.OnLevelWon -= RemoveTooltips;
    }

    private void Awake()
    {
        tooltipOverlay = GetComponent<Canvas>();
        GetTooltipComponents();
        tooltipOverlay.enabled = false;
        buttonEffects = TooltipButton.GetComponent<TooltipButtonEffects>();
    }

    private void Start()
    {
        _playerControl = FindObjectOfType<PlayerController>();
        _inputManager = _playerControl.GetInputManager();
    }


    public void TooltipButtonPressed()
    {
        if (storedEvent == null) return;
        ShowDialogue(storedEvent);
        buttonEffects.DisplayIdle();
    }

    public void StoreTriggerEvent(TriggerEvent triggerEvent)
    {
        storedEvent = triggerEvent;
        buttonEffects.ShowNewTip();
    }

    public void ClearStoredTrigger()
    {
        storedEvent = null;
        buttonEffects.DisplayIdle();
    }

    public void ShowDialogue(TriggerEvent triggerEvent)
    {
        if (!_playerControl.ThePlayer.IsAlive()) { return; }
        TriggerEventSerializer.Instance.SetEventSeen(triggerEvent.Id);
        StartCoroutine(ShowDialogueRoutine(triggerEvent));
    }

    public void ShowDialogue(string text, float duration, bool pausesGame = false)
    {
        TriggerEvent trigEvent = new TriggerEvent();
        trigEvent.Dialogue.Add(text);
        StartCoroutine(ShowDialogueRoutine(trigEvent));
    }

    private IEnumerator ShowDialogueRoutine(TriggerEvent triggerEvent)
    {
        _playerControl.PauseGame(false);
        _playerControl.WaitForTooltip();
        IsPausedForTooltip = true;

        if (!tooltipSetup)
            yield return StartCoroutine(ShowTooltipWindow());

        for (int i = 0; i < triggerEvent.Dialogue.Count; i++)
        {
            SetTooltipText(triggerEvent.Dialogue[i]);
            yield return StartCoroutine(WaitForTooltip(i == triggerEvent.Dialogue.Count - 1));
        }

        if (tooltipSetup)
            yield return StartCoroutine(HideTooltipWindow());

        IsPausedForTooltip = false;
        _playerControl.TooltipResume();
    }

    private IEnumerator ShowTooltipWindow()
    {
        tooltipText.text = "";
        nomee.gameObject.SetActive(false);
        resumePlayImage.gameObject.SetActive(false);
        resumeNextImage.gameObject.SetActive(false);

        tooltipSetup = true;
        tooltipOverlay.enabled = true;

        tooltipScroll.gameObject.SetActive(true);
        tooltipScrollAnimator.Play("TooltipScrollClosed", 0, 0f);
        yield return StartCoroutine(UIObjectAnimator.Instance.PopInObjectRoutine(tooltipScroll));
        tooltipScrollAnimator.Play("TooltipScrollOpen", 0, 0f);
        yield return new WaitForSeconds(0.2f);
        UIObjectAnimator.Instance.PopInObject(nomee);
    }

    private IEnumerator HideTooltipWindow()
    {
        UIObjectAnimator.Instance.PopOutObject(tooltipText.GetComponent<RectTransform>());
        yield return StartCoroutine(UIObjectAnimator.Instance.PopOutObjectRoutine(nomee));
        tooltipScrollAnimator.Play("TooltipScrollClose", 0, 0f);
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(UIObjectAnimator.Instance.PopOutObjectRoutine(tooltipScroll));
        tooltipSetup = false;
        tooltipOverlay.enabled = false;
    }

    private void SetTooltipText(string text)
    {
        tooltipText.text = text;
        UIObjectAnimator.Instance.PopInObject(tooltipText.GetComponent<RectTransform>());
    }
    
    private IEnumerator WaitForTooltip(bool isFinal)
    {
        const float tooltipPauseDuration = 0.3f;
        yield return new WaitForSeconds(tooltipPauseDuration);

        if (isFinal)
            UIObjectAnimator.Instance.PopInObject(resumePlayImage);
        else
            UIObjectAnimator.Instance.PopInObject(resumeNextImage);

        _inputManager.ClearInput();
        while (!_inputManager.TapRegistered())
        {
            yield return null;
        }

        if (isFinal)
            UIObjectAnimator.Instance.PopOutObject(resumePlayImage);
        else
            UIObjectAnimator.Instance.PopOutObject(resumeNextImage);
    }

    private void RemoveTooltips()
    {
        StopAllCoroutines();
        tooltipOverlay.enabled = false;
    }

    private void GetTooltipComponents()
    {
        foreach (RectTransform rt in tooltipOverlay.GetComponentsInChildren<RectTransform>())
        {
            if (rt.name == "TooltipBackground")
                tooltipBackground = rt.GetComponent<Image>();
            else if (rt.name == "TooltipPanel")
            {
                tooltipScroll = rt;
                tooltipScrollAnimator = rt.GetComponent<Animator>();
                foreach (RectTransform r in rt.GetComponentsInChildren<RectTransform>())
                {
                    if (r.name == "ToolTipTextBox")
                    {
                        tooltipTextRt = r;
                        tooltipText = r.GetComponent<Text>();
                    }
                    else if (r.name == "ResumeNextImage")
                        resumeNextImage = r;
                    else if (r.name == "ResumePlayImage")
                        resumePlayImage = r;
                    else if (r.name == "NomeeWindow")
                    {
                        foreach(RectTransform rect in r.GetComponentsInChildren<RectTransform>())
                        {
                            if (rect.name == "Nomee")
                                nomee = rect;
                        }
                    }
                }
            }
        }
    }

}
