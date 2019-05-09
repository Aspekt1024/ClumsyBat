using ClumsyBat.DataManagement;
using ClumsyBat.Serialization;
using UnityEngine;

namespace ClumsyBat
{
    /// <summary>
    /// Manages all data modules for Clumsy Bat
    /// </summary>
    public class DataManager : IManager
    {
        public AbilityControl Abilities { get; } // TODO this is actually Player Data

        public LevelDataHandler LevelDataHandler { get; private set; } = new LevelDataHandler();
        public StatsContainer Stats { get; private set; } = new StatsContainer();
        public GameData GameState { get; } = new GameData();
        public EventDataHandler EventData { get; } = new EventDataHandler();
        public UserSettings Settings { get; } = new UserSettings();
        
        public TriggerEventSerializer TriggerEvents;

        public readonly string PersistentDataPath;

        private readonly SerializationHandler serializationHandler = new SerializationHandler();

        public DataManager(AbilityControl abilityData)
        {
            PersistentDataPath = Application.persistentDataPath;
            Abilities = abilityData;
        }

        /// <summary>
        /// Initializes the Data manager and loads data
        /// </summary>
        public void InitAwake()
        {
            TriggerEvents = Object.FindObjectOfType<TriggerEventSerializer>();
            
            Abilities.Load();
            TriggerEvents.LoadData();

            LevelDataHandler.SetData(serializationHandler.Deserialize<LevelDataContainer>(LevelDataContainer.FILE_NAME));
            Stats = serializationHandler.Deserialize<StatsContainer>(StatsContainer.FILE_NAME);
            Settings.LoadUserSettings();
        }

        public void SaveData()
        {
            Abilities.Save();

            serializationHandler.Serialize(LevelDataHandler.LevelData.Clone(), LevelDataContainer.FILE_NAME);
            serializationHandler.Serialize(Stats.Clone(), StatsContainer.FILE_NAME);
            serializationHandler.Serialize(EventData, EventDataContainer.FILE_NAME);

            Settings.SaveUserSettings();
        }
        
        public void ResetAllData()
        {
            Abilities.ClearAbilityData();

            LevelDataHandler.ResetData();
            Stats = new StatsContainer();

            TriggerEvents.ClearProgressionData();

            SaveData();
        }

        public void ResetTooltips()
        {
            TriggerEvents.ClearProgressionData();
        }
    }
}
