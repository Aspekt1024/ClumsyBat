using UnityEngine;

public class DataHandler
{
    public LevelDataControl LevelData;
    public AbilityControl AbilityData;
    public StoryEventControl StoryData;
    public StatsHandler Stats;

    private bool _bDataLoaded;

    public void LoadDataObjects()
    {
        // Create and load non-persistent objects
        var dataObject = new GameObject("DataObjects");
        LevelData = dataObject.AddComponent<LevelDataControl>();
        AbilityData = dataObject.AddComponent<AbilityControl>();
        StoryData = dataObject.AddComponent<StoryEventControl>();
        
        LevelData.Load();
        AbilityData.Load();
        StoryData.Load();

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
        StoryData.Save();
        Stats.SaveStats();
    }

    public void ResetStoryData()
    {
        LevelData.ClearCompletionData();
        StoryData.ClearStoryEventData();
        AbilityData.ClearAbilityData();
        Stats.ResetCurrency();
    }
}
