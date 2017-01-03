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

    void Awake()
    {
        Trigger.bIsActive = false;
        Trigger.Collider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        if (!Trigger.bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
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
    }

    public void DeactivateTrigger()
    {
        Trigger.bIsActive = false;
        Trigger.Collider.enabled = false;
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
