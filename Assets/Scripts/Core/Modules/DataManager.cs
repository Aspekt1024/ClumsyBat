using ClumsyBat.DataManagement;
using ClumsyBat.Serialization;
using System;
using System.Threading.Tasks;
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

        public readonly string PersistentDataPath;

        private readonly SerializationHandler serializationHandler = new SerializationHandler();

        public DataManager(AbilityControl abilityData)
        {
            PersistentDataPath = Application.persistentDataPath;
            Abilities = abilityData;
        }

        public void InitAwake()
        {
        }

        public void SaveData()
        {
            Abilities.Save();

            serializationHandler.Serialize(LevelDataHandler.LevelData.Clone(), LevelDataContainer.FILE_NAME);
            serializationHandler.Serialize(Stats.Clone(), StatsContainer.FILE_NAME);
            serializationHandler.Serialize(EventData, EventDataContainer.FILE_NAME);

            Settings.SaveUserSettings();
        }

        public void LoadData(Action callback)
        {
            Abilities.Load();

            LevelDataHandler.SetData(serializationHandler.Deserialize<LevelDataContainer>(LevelDataContainer.FILE_NAME));
            Stats = serializationHandler.Deserialize<StatsContainer>(StatsContainer.FILE_NAME);
            Settings.LoadUserSettings();
            callback.Invoke();
        }

        public void ResetStoryData()
        {
            // TODO this
            Abilities.ClearAbilityData();

            LevelDataHandler.ResetData();
            Stats = new StatsContainer();

            // TODO make a this here
            TriggerEventSerializer.Instance.ClearEventData();

            SaveData();
        }
    }
}
