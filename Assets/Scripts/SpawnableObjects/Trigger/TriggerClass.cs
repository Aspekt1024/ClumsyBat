using UnityEngine;

public class TriggerClass : MonoBehaviour {
    
    public struct TriggerProps
    {
        public bool bIsActive;
        public BoxCollider2D Collider;
    }
    
    private float _speed;
    
    public TriggerProps Trigger;
    private TriggerHandler _tHandler;

    public TriggerHandler.EventType EventType;
    public TooltipHandler.DialogueId EventId;
    public bool PausesGame;

    private float _triggerZLayer;

    private void Awake()
    {
        Trigger.bIsActive = false;
        Trigger.Collider = GetComponent<BoxCollider2D>();
        _triggerZLayer = Toolbox.Instance.ZLayers["Trigger"];
    }

    private void FixedUpdate()
    {
        if (!Trigger.bIsActive) { return; }
        transform.position += new Vector3(-_speed * Time.deltaTime, 0f, 0f);
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

    public void SetTHandler(TriggerHandler handler)
    {
        _tHandler = handler;
    }

    public void ActivateTrigger(TriggerHandler.EventType eType, TooltipHandler.DialogueId eId, bool pausesGame)
    {
        Trigger.bIsActive = true;
        EventType = eType;
        EventId = eId;
        PausesGame = pausesGame;
        Trigger.Collider.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = Toolbox.Instance.Debug;
        transform.position = new Vector3(transform.position.x, transform.position.y, _triggerZLayer);
    }

    public void DeactivateTrigger()
    {
        Trigger.bIsActive = false;
        Trigger.Collider.enabled = false;
        transform.position = Toolbox.Instance.HoldingArea;
        
    }

    public bool IsActive()
    {
        return Trigger.bIsActive;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
