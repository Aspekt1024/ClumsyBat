using UnityEngine;

public class TriggerHandler {

    public TriggerHandler()
    {
        SetupTriggerPool();
        _tooltipControl = GameObject.Find("Scripts").GetComponent<TooltipHandler>();
    }
    
    private const int NumTriggersInPool = 4;
    private const string TriggerResourcePath = "Interactables/Trigger";
    private readonly TooltipHandler _tooltipControl;

    public struct TriggerType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
        public EventType EventType;
        public TooltipHandler.DialogueId EventId;
        public bool PausesGame;
    }

    public enum EventType
    {
        Dialogue,
        StoryEvent
    }
    
    TriggerClass[] _triggers;
    int _index;

    private TriggerClass GetTriggerFromPool()
    {
        TriggerClass trigger = _triggers[_index];
        _index++;
        if (_index == _triggers.Length)
        {
            _index = 0;
        }
        return trigger;
    }

    public void SetupTriggerPool()
    {
        TriggerClass[] triggerList = new TriggerClass[NumTriggersInPool];
        for (int i = 0; i < NumTriggersInPool; i++)
        {
            GameObject triggerObj = (GameObject)Object.Instantiate(Resources.Load(TriggerResourcePath));
            TriggerClass trigger = triggerObj.GetComponent<TriggerClass>();
            triggerObj.transform.position = Toolbox.Instance.HoldingArea;
            trigger.SetTHandler(this);
            triggerList[i] = trigger;
        }
        _triggers = triggerList;
        _index = 0;
    }

    public void SetupTriggersInList(TriggerType[] triggerList, float xOffset)
    {
        foreach (TriggerType trigger in triggerList)
        {
            float triggerZLayer = Toolbox.Instance.ZLayers["Trigger"];
            TriggerClass newTrigger = GetTriggerFromPool();
            newTrigger.transform.position = new Vector3(trigger.Pos.x + xOffset, trigger.Pos.y, triggerZLayer);
            newTrigger.transform.localScale = trigger.Scale;
            newTrigger.transform.localRotation = trigger.Rotation;
            newTrigger.ActivateTrigger(trigger.EventType, trigger.EventId, trigger.PausesGame);
        }
    }

    public void SetVelocity(float speed)
    {
        foreach (TriggerClass trigger in _triggers)
        {
            trigger.SetSpeed(speed);
        }
    }

    public void ActivateDialogue(TooltipHandler.DialogueId eventId, bool pauseGame)
    {
        _tooltipControl.ShowDialogue(eventId, pauseGame);
    }
}
