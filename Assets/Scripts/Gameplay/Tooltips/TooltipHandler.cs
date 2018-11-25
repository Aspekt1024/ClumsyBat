﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ClumsyBat;

public class TooltipHandler : MonoBehaviour {
    
    public RectTransform DialogueButton;

    private TriggerEvent storedEvent;

    private Canvas dialogueOverlay;
    private RectTransform dialogueScroll;
    private Animator dialogueScrollAnimator;
    private Text dialogueText;
    private RectTransform resumeNextImage;
    private RectTransform resumePlayImage;
    private RectTransform nomee;
    
    private TooltipButtonEffects buttonEffects;

    private bool tooltipSetup;

    #region Initialisation
    private void OnEnable()
    {
        EventListener.OnDeath += RemoveDialogue;
        EventListener.OnLevelWon += RemoveDialogue;
    }
    private void OnDisable()
    {
        EventListener.OnDeath -= RemoveDialogue;
        EventListener.OnLevelWon -= RemoveDialogue;
    }

    private void Awake()
    {
        dialogueOverlay = GetComponent<Canvas>();
        GetDialogueComponents();
        dialogueOverlay.enabled = false;
        buttonEffects = DialogueButton.GetComponent<TooltipButtonEffects>();
    }
    #endregion

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
        if (!GameStatics.Player.Clumsy.State.IsAlive) { return; }
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
        GameStatics.GameManager.PauseGame();
        GameStatics.LevelManager.GameHandler.GameState = GameHandler.GameStates.PausedForTooltip;

        Vector3 position = GameStatics.Player.Clumsy.transform.position;
        position.z = -8f;
        GameStatics.Player.Clumsy.transform.position = position;

        float yPos = (GameStatics.Player.Clumsy.transform.position.y > 0) ? -2f : 2f;
        dialogueScroll.position = new Vector3(dialogueScroll.position.x, yPos, dialogueScroll.position.z);

        if (!tooltipSetup)
        {
            yield return StartCoroutine(ShowDialogueWindow());
        }

        for (int i = 0; i < triggerEvent.Dialogue.Count; i++)
        {
            SetDialogueText(triggerEvent.Dialogue[i]);
            yield return StartCoroutine(WaitForDialogue(i == triggerEvent.Dialogue.Count - 1));
        }

        if (tooltipSetup)
        {
            yield return StartCoroutine(HideDialogueWindow());
        }
        
        position.z = -1;
        GameStatics.Player.Clumsy.transform.position = position;
    }

    private IEnumerator ShowDialogueWindow()
    {
        dialogueText.text = "";
        nomee.gameObject.SetActive(false);
        resumePlayImage.gameObject.SetActive(false);
        resumeNextImage.gameObject.SetActive(false);

        tooltipSetup = true;
        dialogueOverlay.enabled = true;

        dialogueScroll.gameObject.SetActive(true);
        dialogueScrollAnimator.Play("TooltipScrollClosed", 0, 0f);
        yield return StartCoroutine(UIObjectAnimator.Instance.PopInObjectRoutine(dialogueScroll));
        dialogueScrollAnimator.Play("TooltipScrollOpen", 0, 0f);
        yield return new WaitForSeconds(0.2f);
        UIObjectAnimator.Instance.PopInObject(nomee);
    }

    private IEnumerator HideDialogueWindow()
    {
        UIObjectAnimator.Instance.PopOutObject(dialogueText.GetComponent<RectTransform>());
        yield return StartCoroutine(UIObjectAnimator.Instance.PopOutObjectRoutine(nomee));
        dialogueScrollAnimator.Play("TooltipScrollClose", 0, 0f);
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(UIObjectAnimator.Instance.PopOutObjectRoutine(dialogueScroll));
        tooltipSetup = false;
        dialogueOverlay.enabled = false;
    }

    private void SetDialogueText(string text)
    {
        dialogueText.text = text;
        UIObjectAnimator.Instance.PopInObject(dialogueText.GetComponent<RectTransform>());
    }
    
    private IEnumerator WaitForDialogue(bool isFinal)
    {
        const float tooltipPauseDuration = 0.3f;
        yield return new WaitForSeconds(tooltipPauseDuration);

        if (isFinal)
            UIObjectAnimator.Instance.PopInObject(resumePlayImage);
        else
            UIObjectAnimator.Instance.PopInObject(resumeNextImage);

        //_inputManager.ClearInput();
        //while (!_inputManager.IsTapRegistered())
        //{
        //    yield return null;
        //}

        if (isFinal)
            UIObjectAnimator.Instance.PopOutObject(resumePlayImage);
        else
            UIObjectAnimator.Instance.PopOutObject(resumeNextImage);
    }

    private void RemoveDialogue()
    {
        StopAllCoroutines();
        dialogueOverlay.enabled = false;
    }

    private void GetDialogueComponents()
    {
        foreach (RectTransform rt in dialogueOverlay.GetComponentsInChildren<RectTransform>())
        {
            if (rt.name == "TooltipPanel")
            {
                dialogueScroll = rt;
                dialogueScrollAnimator = rt.GetComponent<Animator>();
                foreach (RectTransform r in rt.GetComponentsInChildren<RectTransform>())
                {
                    if (r.name == "ToolTipTextBox")
                    {
                        dialogueText = r.GetComponent<Text>();
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
