using UnityEngine;

public sealed class TriggerHandler : SpawnPool<TriggerClass> {

    public TriggerHandler()
    {
        ParentName = "Triggers";
        ParentZ = Toolbox.Instance.ZLayers["Trigger"];
        ResourcePath = "Interactables/Trigger";
        ObjTag = "Trigger";
    }

    public enum EventType { Dialogue, OneTimeEvent, Tooltip }
    public struct TriggerType
    {
        public Spawnable.SpawnType SpawnTransform;
        public EventType EventType;
        public string TooltipText;
        public float TooltipDuration;
        public TooltipHandler.DialogueId EventId;
        public bool PausesGame;
    }
    
    public void SetupTriggersInList(TriggerType[] triggerList, float xOffset)
    {
        foreach (TriggerType trigger in triggerList)
        {
            TriggerClass newTrigger = GetNewObject();
            Spawnable.SpawnType spawnTf = trigger.SpawnTransform;
            spawnTf.Pos += new Vector2(xOffset, 0f);
            newTrigger.Activate(trigger, spawnTf);
        }
    }
    
}
