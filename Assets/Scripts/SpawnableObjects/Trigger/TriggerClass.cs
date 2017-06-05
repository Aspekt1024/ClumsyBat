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
    public string TooltipText;
    public float TooltipDuration = 3f;
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
        if (!GameData.Instance.Data.Stats.Settings.Tooltips) return;

        switch (EventType)
        {
            case (TriggerHandler.EventType.Dialogue):
                _tHandler.ShowDialogue(EventId, _waitType);
                break;
            case (TriggerHandler.EventType.Tooltip):
                _tHandler.ShowDialogue(TooltipText, TooltipDuration);
                break;
            case TriggerHandler.EventType.OneTimeEvent:
                //_tHandler.ShowDialogue()
                break;
        }
        
        DeactivateTrigger();
    }
    
    public void Activate(TriggerHandler.TriggerType triggerProps, SpawnType spawnTf)
    {
        base.Activate(transform, spawnTf);
        EventType = triggerProps.EventType;
        EventId = triggerProps.EventId;
        TooltipText = triggerProps.TooltipText;
        TooltipDuration = triggerProps.TooltipDuration;
        PausesGame = triggerProps.PausesGame;
        Trigger.Collider.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = Toolbox.Instance.Debug;
        _waitType = PausesGame ? TooltipHandler.WaitType.InGamePause : TooltipHandler.WaitType.InGameNoPause;
    }
    
    public void DeactivateTrigger() { SendToInactivePool(); }
    public bool Active() { return IsActive; }
}
