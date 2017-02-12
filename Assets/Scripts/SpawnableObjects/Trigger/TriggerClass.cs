using UnityEngine;

public class TriggerClass : Spawnable {
    
    public struct TriggerProps
    {
        public BoxCollider2D Collider;
    }
    
    [HideInInspector]
    public TriggerProps Trigger;
    private TooltipHandler _tHandler;

    // Editor properties
    public TriggerHandler.EventType EventType;
    public TooltipHandler.DialogueId EventId;
    public bool PausesGame;

    private TooltipHandler.WaitType _waitType;

    private void Awake()
    {
        Trigger.Collider = GetComponent<BoxCollider2D>();
        _tHandler = FindObjectOfType<TooltipHandler>();
        _waitType = TooltipHandler.WaitType.InGameNoPause;
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
                    _tHandler.ShowDialogue(EventId, _waitType);
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
        _waitType = PausesGame ? TooltipHandler.WaitType.InGamePause : TooltipHandler.WaitType.InGameNoPause;
    }

    public void DeactivateTrigger() { SendToInactivePool(); }
    public bool Active() { return IsActive; }
}
