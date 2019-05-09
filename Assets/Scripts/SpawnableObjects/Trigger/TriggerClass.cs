using UnityEngine;

namespace ClumsyBat.Objects
{
    [ExecuteInEditMode]
    public class TriggerClass : Spawnable
    {

        public TriggerEvent TriggerEvent;
        [HideInInspector] public BoxCollider2D Collider;
        [HideInInspector] public int TriggerId; // For serialization/deserialization - not used in-game

        private TooltipHandler _tHandler;

        private bool isTriggered;

        public void Spawn(TriggerHandler.TriggerType triggerProps, SpawnType spawnTf)
        {
            base.Spawn(transform, spawnTf);
            Collider.enabled = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = Toolbox.Instance.Debug;

            TriggerEvent = GameStatics.Data.TriggerEvents.GetTriggerEvent(triggerProps.TrigEvent.Id);
        }

        protected override void Init() { }

        private void Awake()
        {
            Collider = GetComponent<BoxCollider2D>();
            _tHandler = FindObjectOfType<TooltipHandler>();

        }
        private void Start()
        {
            if (TriggerEvent.Id > 0)
            {
                TriggerEvent = GameStatics.Data.TriggerEvents.GetTriggerEvent(TriggerEvent.Id);
            }
#if UNITY_EDITOR
            if (TriggerEvent.Id == 0 && !Application.isPlaying)
            {
                TriggerEvent = GameStatics.Data.TriggerEvents.CreateNewTriggerEvent();
                TriggerId = TriggerEvent.Id;
            }
#endif
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!GameStatics.Data.Settings.TooltipsOn || isTriggered) return;

            isTriggered = true;
            switch (TriggerEvent.EventType)
            {
                case (TriggerHandler.EventType.Dialogue):
                    ActionDialogueEvent();
                    break;
                case (TriggerHandler.EventType.Event):
                    ActionEvent();
                    break;
            }
        }

        private void ActionDialogueEvent()
        {
            bool isSeen = GameStatics.Data.TriggerEvents.IsEventSeen(TriggerEvent.Id);
            if (TriggerEvent.ShowOnce && isSeen) return;

            bool shownBeforeRestart = Toolbox.Instance.TooltipCompleted(TriggerEvent.Id);
            if (!TriggerEvent.ShowOnRestart && shownBeforeRestart) return;

            bool levelCompleted = GameStatics.Data.LevelDataHandler.IsCompleted(GameStatics.LevelManager.Level);
            if (!TriggerEvent.ShowOnCompletedLevel && levelCompleted) return;

            if (!MeetsDependency()) return;

            if (TriggerEvent.ForceShow == TriggerHandler.ForceOptions.Always || (TriggerEvent.ForceShow == TriggerHandler.ForceOptions.Once && !isSeen))
                _tHandler.ShowDialogue(TriggerEvent);
            else
                _tHandler.StoreTriggerEvent(TriggerEvent);
        }

        private bool MeetsDependency()
        {
            if (!TriggerEvent.HasDependency) return true;

            switch (TriggerEvent.DependencyId)
            {
                case TriggerHandler.DependencyId.None:
                    return true;
                case TriggerHandler.DependencyId.HasHypersonic:
                    return GameStatics.Data.Abilities.GetHypersonicStats().AbilityUnlocked;
                case TriggerHandler.DependencyId.NoHypersonic:
                    return !GameStatics.Data.Abilities.GetHypersonicStats().AbilityUnlocked;
                case TriggerHandler.DependencyId.HasDash:
                    return GameStatics.Data.Abilities.GetDashStats().AbilityUnlocked;
                case TriggerHandler.DependencyId.NoDash:
                    return !GameStatics.Data.Abilities.GetDashStats().AbilityUnlocked;
                default:
                    return true;
            }

        }

        private void ActionEvent()
        {
            Debug.Log("Events from triggers not yet implemented");
        }

    }
}