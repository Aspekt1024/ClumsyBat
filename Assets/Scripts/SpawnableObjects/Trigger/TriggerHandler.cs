using UnityEngine;

namespace ClumsyBat.Objects
{
    public sealed class TriggerHandler : SpawnPool<TriggerClass>
    {
        public TriggerHandler()
        {
            ParentName = "Triggers";
            ParentZ = Toolbox.Instance.ZLayers["Trigger"];
            ResourcePath = "Interactables/Trigger";
            ObjTag = "Trigger";
        }

        public enum EventType { Dialogue, Event }
        public enum ForceOptions { Never, Once, Always }

        public enum EventId { None }
        public enum DependencyId { None, HasHypersonic, NoHypersonic, HasDash, NoDash }

        public struct TriggerType
        {
            public Spawnable.SpawnType SpawnTransform;
            public TriggerEvent TrigEvent;
        }

        public void SetupTriggersInList(TriggerType[] triggerList, float xOffset)
        {
            foreach (TriggerType trigger in triggerList)
            {
                TriggerClass newTrigger = GetObjectFromPool();
                Spawnable.SpawnType spawnTf = trigger.SpawnTransform;
                spawnTf.Pos += new Vector2(xOffset, 0f);
                newTrigger.Spawn(trigger, spawnTf);
            }
        }
    }
}