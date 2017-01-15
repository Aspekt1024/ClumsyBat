using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using ToolID = TooltipHandler.TooltipID;

public class StoryEventControl : MonoBehaviour {

    public enum StoryEvents
    {
        FirstDeath,
        FirstGoldMoth
    }
    public bool[] StoryData = new bool[Enum.GetNames(typeof(StoryEvents)).Length];
    public Dictionary<StoryEvents, ToolID[]> TooltipSet = new Dictionary<StoryEvents, ToolID[]>();

    TooltipHandler TooltipControl = null;
    PlayerController PlayerControl = null;
    
    void Start()
    {
        TooltipControl = GameObject.Find("Scripts").GetComponent<TooltipHandler>();
        PlayerControl = FindObjectOfType<PlayerController>();
        SetupTooltipDict();
    }


    public void TriggerEvent(StoryEvents EventID)
    {
        if (!EventCompleted(EventID) && PlayerControl.ThePlayer.IsAlive())
        {
            StartCoroutine("TriggerEventCoroutine", EventID);
        }
    }
    public IEnumerator TriggerEventCoroutine(StoryEvents EventID)
    {
        yield return StartCoroutine("ShowStoryDialogue", EventID);
    }

    public bool EventCompleted(StoryEvents EventID)
    {
        return StoryData[(int)EventID];
    }
    
    private IEnumerator ShowStoryDialogue(StoryEvents EventID)
    {
        yield return TooltipControl.StartCoroutine("SetupDialogue", TooltipSet[EventID]);
        StoryData[(int)EventID] = true;
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/StoryEventData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file;
            file = File.Open(Application.persistentDataPath + "/StoryEventData.dat", FileMode.Open);

            StoryEventContainer Data;
            try
            {
                Data = (StoryEventContainer)bf.Deserialize(file);
            }
            catch
            {
                Data = new StoryEventContainer();
                Debug.Log("Unable to load existing Story data set");
            }
            file.Close();

            StoryData = Data.StoryEventData;
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        file = File.Open(Application.persistentDataPath + "/StoryEventData.dat", FileMode.OpenOrCreate);

        StoryEventContainer Data = new StoryEventContainer();
        Data.StoryEventData = StoryData;

        bf.Serialize(file, Data);
        file.Close();
    }

    public void ClearStoryEventData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/StoryEventData.dat", FileMode.Create);

        StoryEventContainer BlankStoryData = new StoryEventContainer();
        BlankStoryData.StoryEventData = new bool[Enum.GetNames(typeof(StoryEvents)).Length];

        bf.Serialize(file, BlankStoryData);
        file.Close();
        Load();
        Debug.Log("Story Event Data Cleared");
    }
    
    private void SetupTooltipDict()
    {
        TooltipSet.Add(StoryEvents.FirstGoldMoth, new ToolID[] { ToolID.FirstGoldMoth1, ToolID.FirstGoldMoth2 });
        TooltipSet.Add(StoryEvents.FirstDeath, new ToolID[] { ToolID.FirstDeath });
        //TooltipSet.Add(StoryEvents, new ToolID[] { ToolID });
    }
}

[Serializable]
public class StoryEventContainer
{
    public bool[] StoryEventData;
}
