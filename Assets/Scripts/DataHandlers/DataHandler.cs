using UnityEngine;

public class DataHandler
{
    public LevelDataControl LevelData;
    public AbilityControl AbilityData;
    public StatsHandler Stats;

    private bool _bDataLoaded;

    public void LoadDataObjects()
    {
        // Create and load non-persistent objects
        var dataObject = new GameObject("DataObjects");
        LevelData = dataObject.AddComponent<LevelDataControl>();
        AbilityData = dataObject.AddComponent<AbilityControl>();
        
        LevelData.Load();
        AbilityData.Load();

        // Load persistent data, if not already loaded
        if (_bDataLoaded) return;
        Stats = new StatsHandler();

        Stats.LoadStats();
        _bDataLoaded = true;
    }

    public void SaveData()
    {
        LevelData.Save();
        AbilityData.Save();
        Stats.SaveStats();
        TriggerEventSerializer.Instance.SaveEventProgressionData();
    }

    public void ResetStoryData()
    {
        LevelData.ClearCompletionData();
        AbilityData.ClearAbilityData();
        Stats.ResetCurrency();
        TriggerEventSerializer.Instance.ClearEventData();
    }
}
