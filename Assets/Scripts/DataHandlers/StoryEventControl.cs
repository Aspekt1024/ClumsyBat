using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

public class StoryEventControl : MonoBehaviour {

    public enum StoryEvents
    {
        FirstDeath,
        FirstGoldMoth
    }
    public bool[] StoryData = new bool[Enum.GetNames(typeof(StoryEvents)).Length];
    public Dictionary<StoryEvents, TooltipHandler.TooltipId[]> TooltipSet = new Dictionary<StoryEvents, TooltipHandler.TooltipId[]>();

    private TooltipHandler _tooltipControl;
    private PlayerController _playerControl;
    
    private void Start()
    {
        _tooltipControl = GameObject.Find("Scripts").GetComponent<TooltipHandler>();
        _playerControl = FindObjectOfType<PlayerController>();
        SetupTooltipDict();
    }

    public void TriggerEvent(StoryEvents eventId)
    {
        if (!EventCompleted(eventId) && _playerControl.ThePlayer.IsAlive())
        {
            StartCoroutine("TriggerEventCoroutine", eventId);
        }
    }
    public IEnumerator TriggerEventCoroutine(StoryEvents eventId)
    {
        yield return StartCoroutine("ShowStoryDialogue", eventId);
    }

    public bool EventCompleted(StoryEvents eventId)
    {
        return StoryData[(int)eventId];
    }
    
    private IEnumerator ShowStoryDialogue(StoryEvents eventId)
    {
        yield return _tooltipControl.StartCoroutine("SetupDialogue", TooltipSet[eventId]);
        StoryData[(int)eventId] = true;
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/StoryEventData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/StoryEventData.dat", FileMode.Open);

            StoryEventContainer data;
            try
            {
                data = (StoryEventContainer)bf.Deserialize(file);
            }
            catch
            {
                data = new StoryEventContainer();
                Debug.Log("Unable to load existing Story data set");
            }
            file.Close();

            StoryData = data.StoryEventData;
        }
    }

    public void Save()
    {
        var bf = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/StoryEventData.dat", FileMode.OpenOrCreate);

        var data = new StoryEventContainer { StoryEventData = StoryData };

        bf.Serialize(file, data);
        file.Close();
    }

    public void ClearStoryEventData()
    {
        var bf = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/StoryEventData.dat", FileMode.Create);

        var blankStoryData = new StoryEventContainer();
        blankStoryData.StoryEventData = new bool[Enum.GetNames(typeof(StoryEvents)).Length];

        bf.Serialize(file, blankStoryData);
        file.Close();
        Load();
        Debug.Log("Story Event Data Cleared");
    }
    
    private void SetupTooltipDict()
    {
        TooltipSet.Add(StoryEvents.FirstGoldMoth, new[] { TooltipHandler.TooltipId.FirstGoldMoth1, TooltipHandler.TooltipId.FirstGoldMoth2 });
        TooltipSet.Add(StoryEvents.FirstDeath, new[] { TooltipHandler.TooltipId.FirstDeath });
        //TooltipSet.Add(StoryEvents, new ToolID[] { ToolID });
    }
}

[Serializable]
public class StoryEventContainer
{
    public bool[] StoryEventData;
}
