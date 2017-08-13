using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class TriggerClass : Spawnable {
    
    public TriggerEvent TriggerEvent;
    [HideInInspector] public BoxCollider2D Collider;
    [HideInInspector] public int TriggerId; // For serialization/deserialization - not used in-game

    private TooltipHandler _tHandler;

    private void Awake()
    {
        Collider = GetComponent<BoxCollider2D>();
        _tHandler = FindObjectOfType<TooltipHandler>();

    }
    private void Start()
    {
        if (TriggerId > 0)
        {
            TriggerEvent = TriggerEventSerializer.Instance.GetTriggerEvent(TriggerId);
        }
#if UNITY_EDITOR
        if (TriggerId == 0 && !Application.isPlaying)
        {
            TriggerEvent = TriggerEventSerializer.Instance.CreateNewTriggerEvent();
            TriggerId = TriggerEvent.Id;
        }
#endif
    }

    private void FixedUpdate()
    {
        if (!IsActive) { return; }
        MoveLeft(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameData.Instance.Data.Stats.Settings.Tooltips || !IsActive) return;
        IsActive = false;
        if (!TriggerEventSerializer.Instance.IsEventSeen(TriggerEvent.Id) || !TriggerEvent.ShowOnce)
        {
            if (TriggerEvent.PausesGame == false)
                _tHandler.StoreTriggerEvent(TriggerEvent);
            else
                _tHandler.ShowDialogue(TriggerEvent);
        }

        // TODO logic
        switch (TriggerEvent.EventType)
        {
            case (TriggerHandler.EventType.Dialogue):
                //_tHandler.ShowDialogue(EventId, _waitType);
                break;
            case (TriggerHandler.EventType.Tooltip):
                //_tHandler.ShowDialogue(TooltipText, TooltipDuration, PausesGame);
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
        Collider.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = Toolbox.Instance.Debug;
        
        TriggerEvent = TriggerEventSerializer.Instance.GetTriggerEvent(triggerProps.TrigEvent.Id);
    }
    
    public void DeactivateTrigger() { SendToInactivePool(); }
    public bool Active() { return IsActive; }
    
}
