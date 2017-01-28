using UnityEngine;

public class TriggerClass : Spawnable {
    
    public struct TriggerProps
    {
        public BoxCollider2D Collider;
    }
    
    [HideInInspector]
    public TriggerProps Trigger;
    private TriggerHandler _tHandler;

    // Editor properties
    public TriggerHandler.EventType EventType;
    public TooltipHandler.DialogueId EventId;
    public bool PausesGame;

    private void Awake()
    {
        Trigger.Collider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!IsActive) { return; }
        MoveLeft(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (EventType)
        {
            case (TriggerHandler.EventType.Dialogue):
                if (Toolbox.Instance.ShowLevelTooltips)
                {
                    _tHandler.ActivateDialogue(EventId, PausesGame);
                }
                break;
        }
        
        DeactivateTrigger();
    }
    
    public void Activate(TriggerHandler.TriggerType triggerProps, SpawnType spawnTf)
    {
        base.Activate(transform, spawnTf);
        EventType = triggerProps.EventType;
        EventId = triggerProps.EventId;
        PausesGame = triggerProps.PausesGame;
        Trigger.Collider.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = Toolbox.Instance.Debug;
    }

    public void DeactivateTrigger() { SendToInactivePool(); }
    public bool Active() { return IsActive; }
    public void SetTHandler(TriggerHandler handler) { _tHandler = handler; }
}
