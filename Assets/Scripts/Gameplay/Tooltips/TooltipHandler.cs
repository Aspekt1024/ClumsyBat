using UnityEngine;
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

    public void ShowDialogue(TriggerEvent triggerEvent)
    {
        if (!GameStatics.Player.Clumsy.State.IsAlive) { return; }

        TriggerEventSerializer.Instance.SetEventSeen(triggerEvent.Id);
        StartCoroutine(ShowDialogueRoutine(triggerEvent));
    }

    private IEnumerator ShowDialogueRoutine(TriggerEvent triggerEvent)
    {
        GameStatics.GameManager.PauseGame();
        GameStatics.Data.GameState.IsPausedForTooltip = true;

        Vector3 position = GameStatics.Player.Clumsy.Model.position;
        position.z = -8f;
        GameStatics.Player.Clumsy.Model.position = position;

        float yPos = (GameStatics.Player.Clumsy.Model.position.y > 0) ? -2f : 2f;
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

        ui.Close();
        
        position.z = -1;
        GameStatics.Player.Clumsy.Model.position = position;

        GameStatics.GameManager.ResumeGame();
        GameStatics.Data.GameState.IsPausedForTooltip = false;
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
