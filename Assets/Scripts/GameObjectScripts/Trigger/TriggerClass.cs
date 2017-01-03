using UnityEngine;
using System.Collections;

public class TriggerClass : MonoBehaviour {
    
    public struct TriggerProps
    {
        public bool bIsActive;
        public BoxCollider2D Collider;
    }
    
    private float Speed;
    
    public TriggerProps Trigger;
    private TriggerHandler THandler;

    public TriggerHandler.EventType EventType;
    public TriggerHandler.EventID EventID;

    private float TriggerZLayer;

    void Awake()
    {
        Trigger.bIsActive = false;
        Trigger.Collider = GetComponent<BoxCollider2D>();
        TriggerZLayer = Toolbox.Instance.ZLayers["Trigger"];
    }

    void FixedUpdate()
    {
        if (!Trigger.bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0f, 0f);
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (EventType)
        {
            case (TriggerHandler.EventType.Tooltip):
                THandler.ActivateTooltip(EventID);
                break;
        }
        
        DeactivateTrigger();
    }

    public void SetTHandler(TriggerHandler Handler)
    {
        THandler = Handler;
    }

    public void ActivateTrigger(TriggerHandler.EventType eType, TriggerHandler.EventID eID)
    {
        Trigger.bIsActive = true;
        EventType = eType;
        EventID = eID;
        Trigger.Collider.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = Toolbox.Instance.Debug;
        transform.position = new Vector3(transform.position.x, transform.position.y, TriggerZLayer);
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

    public void SetSpeed(float _speed)
    {
        Speed = _speed;
    }
}
