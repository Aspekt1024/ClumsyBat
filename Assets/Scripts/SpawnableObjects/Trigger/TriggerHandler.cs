using UnityEngine;

public sealed class TriggerHandler : SpawnPool<TriggerClass> {

    public TriggerHandler()
    {
        ParentName = "Triggers";
        ParentZ = Toolbox.Instance.ZLayers["Trigger"];
        NumObjectsInPool = 4;
        ResourcePath = "Interactables/Trigger";
        ObjTag = "Trigger";
        SetupPool();
    }

    public enum EventType { Dialogue, StoryEvent }
    public struct TriggerType
    {
        public Spawnable.SpawnType SpawnTransform;
        public EventType EventType;
        public TooltipHandler.DialogueId EventId;
        public bool PausesGame;
    }
    
    public void SetupTriggersInList(TriggerType[] triggerList, float xOffset)
    {
        foreach (TriggerType trigger in triggerList)
        {
            TriggerClass newTrigger = GetNextObject();
            Spawnable.SpawnType spawnTf = trigger.SpawnTransform;
            spawnTf.Pos += new Vector2(xOffset, 0f);
            newTrigger.Activate(trigger, spawnTf);
        }
    }
}
