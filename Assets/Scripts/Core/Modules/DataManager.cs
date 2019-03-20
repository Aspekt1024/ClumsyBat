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
    public class DataManager
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

        public async void SaveData()
        {
            await Task.Run(() => 
            {
                Abilities.Save();
            });

            await serializationHandler.Serialize(LevelDataHandler.LevelData.Clone(), LevelDataContainer.FILE_NAME);
            await serializationHandler.Serialize(Stats.Clone(), StatsContainer.FILE_NAME);
            await serializationHandler.Serialize(EventData, EventDataContainer.FILE_NAME);
        }

        public async void LoadData(Action callback)
        {
            await Task.Run(() =>
            {
                Abilities.Load();
            });

            LevelDataHandler.SetData(await serializationHandler.Deserialize<LevelDataContainer>(LevelDataContainer.FILE_NAME));
            Stats = await serializationHandler.Deserialize<StatsContainer>(StatsContainer.FILE_NAME);
            GameStatics.Player.Clumsy.Abilities.SetData(Abilities);
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
