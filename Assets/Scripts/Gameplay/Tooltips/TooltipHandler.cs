﻿using UnityEngine;
using System.Collections;
using ClumsyBat;
using ClumsyBat.UI;

public class TooltipHandler : MonoBehaviour {

#pragma warning disable 649
    [SerializeField] private TooltipUI ui;
    [SerializeField] private TooltipButtonEffects buttonEffects;
#pragma warning restore 649

    private TriggerEvent storedEvent;

    private enum States
    {
        None, Frozen
    }
    private States state = States.None;

    // TODO this isn't the best implementation but it's not worth changing at this point in development
    private bool continuePressed;

    #region Lifecycle
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
    #endregion Lifecycle

    public void TooltipButtonPressed()
    {
        if (storedEvent == null) return;
        ShowDialogue(storedEvent);
        buttonEffects.DisplayIdle();
    }

    public void InputReceived()
    {
        if (state == States.Frozen) return;
        continuePressed = true;
    }

    public void StoreTriggerEvent(TriggerEvent triggerEvent)
    {
        storedEvent = triggerEvent;
        buttonEffects.ShowNewTip();
    }

    public void ShowDialogue(TriggerEvent triggerEvent, System.Action callback = null)
    {
        if (!GameStatics.Player.Clumsy.State.IsAlive || !GameStatics.Data.Settings.TooltipsOn)
        {
            callback?.Invoke();
            return;
        }

        GameStatics.Data.TriggerEvents.SetEventSeen(triggerEvent.Id);
        StartCoroutine(ShowDialogueRoutine(triggerEvent, callback));
    }

    private IEnumerator ShowDialogueRoutine(TriggerEvent triggerEvent, System.Action callback)
    {
        GameStatics.GameManager.PauseGame();
        GameStatics.Data.GameState.IsPausedForTooltip = true;

        GameStatics.Player.Clumsy.model.GetComponent<SpriteRenderer>().sortingLayerName = "UIFront";

        float yPos = (GameStatics.Player.Clumsy.model.position.y > 0) ? -2f : 2f;
        ui.Open(yPos);

        for (int i = 0; i < triggerEvent.Dialogue.Count; i++)
        {
            if (i == triggerEvent.Dialogue.Count - 1)
            {
                ui.SetText(triggerEvent.Dialogue[i], TooltipUI.NextDialogueImages.ResumeGame);
            }
            else
            {
                ui.SetText(triggerEvent.Dialogue[i], TooltipUI.NextDialogueImages.NextDialogue);
            }
            yield return StartCoroutine(WaitForDialogue(i == triggerEvent.Dialogue.Count - 1));
        }

        yield return StartCoroutine(ui.Close());
        GameStatics.Player.Clumsy.model.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        GameStatics.GameManager.ResumeGame();
        callback?.Invoke();
    }
    
    private IEnumerator WaitForDialogue(bool isFinal)
    {
        state = States.Frozen;
        const float tooltipPauseDuration = 0.3f;
        yield return new WaitForSecondsRealtime(tooltipPauseDuration);

        continuePressed = false;
        state = States.None;

        // TODO this isn't the best implementation but it's not worth changing at this point in development
        while (!continuePressed)
        {
            yield return null;
        }
    }

    private void RemoveDialogue()
    {
        StopAllCoroutines();
        ui.CloseImmediate();
    }
}
