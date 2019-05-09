using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;
using ClumsyBat.Objects;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TriggerEventSerializer : MonoBehaviour {

    private List<TriggerEvent> TriggerEvents;
    private List<TriggerProgressionData> ProgressionData;
    private static TriggerEventSerializer triggerEventSerializer;
    
    private const string resourceLoadPath = "EventData/TriggerEventData";
    private const string savePath = "Assets/Resources/EventData/TriggerEventData.xml";
    private const string progressionSavePath = "TriggerEventData.dat";

    public void LoadData()
    {
        Init();
    }

    public TriggerEvent CreateNewTriggerEvent()
    {
        Load();
        TriggerEvent triggerEvent = new TriggerEvent();
        triggerEvent.Id = GetUniqueId();
        triggerEvent.Dialogue.Add("");
        TriggerEvents.Add(triggerEvent);
        Save();
        return triggerEvent;
    }

    public TriggerEvent GetTriggerEvent(int id)
    {
        foreach(TriggerEvent te in TriggerEvents)
        {
            if (te.Id == id)
            {
                return te;
            }
        }
        return null;
    }

    public void RemoveTriggerEvent(int id)
    {
        Load();
        foreach (TriggerEvent te in TriggerEvents)
        {
            if (te.Id == id)
            {
                if (TriggerEvents.Contains(te))
                    TriggerEvents.Remove(te);
                break;
            }
        }
        Save();
    }

    public void Save()
    {
        TriggerContainer data = new TriggerContainer();
        data.EventData = TriggerEvents;

        XmlSerializer serializer = new XmlSerializer(typeof(TriggerContainer));
        using (var stream = new FileStream(savePath, FileMode.Create))
        {
            serializer.Serialize(stream, data);
        }
#if UNITY_EDITOR
        AssetDatabase.ImportAsset(savePath);
#endif
    }

    public void Load()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TriggerContainer));

        TriggerContainer data;
        TextAsset dataText = (TextAsset)Resources.Load(resourceLoadPath);
        if (dataText == null)
        {
            Debug.Log("Creating new Trigger Event data file");
            TriggerEvents = new List<TriggerEvent>();
            Save();
        }
        else
        {
            using (var stream = new StringReader(dataText.text))
            {
                data = (TriggerContainer)serializer.Deserialize(stream);
            }
            TriggerEvents = data.EventData;
        }
    }

    public void SaveEventProgressionData()
    {
        var bf = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/" + progressionSavePath, FileMode.OpenOrCreate);

        var data = new TriggerProgressionContainer { TriggerData = ProgressionData };

        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadEventProgressionData()
    {
        if (File.Exists(Application.persistentDataPath + "/" + progressionSavePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + progressionSavePath, FileMode.Open);

            TriggerProgressionContainer data;
            try
            {
                data = (TriggerProgressionContainer)bf.Deserialize(file);
            }
            catch
            {
                data = new TriggerProgressionContainer();
                Debug.Log("Unable to load existing Trigger Event Progression data");
            }
            file.Close();

            ProgressionData = data.TriggerData;
        }
    }

    public void ClearProgressionData()
    {
        ProgressionData = new List<TriggerProgressionData>();
        SaveEventProgressionData();
    }

    public bool IsEventSeen(int triggerId)
    {
        foreach (var trigEvent in ProgressionData)
        {
            if (trigEvent.Id == triggerId)
                return trigEvent.TooltipSeen;
        }
        return false;
    }

    public void SetEventSeen(int triggerId)
    {
        foreach (var te in ProgressionData)
        {
            if (te.Id == triggerId)
            {
                te.TooltipSeen = true;
                SaveEventProgressionData();
                return;
            }
        }
        TriggerProgressionData trigEvent = new TriggerProgressionData();
        trigEvent.Id = triggerId;
        trigEvent.TooltipSeen = true;
        ProgressionData.Add(trigEvent);
    }

    private void Init()
    {
        ProgressionData = new List<TriggerProgressionData>();
        Load();
        LoadEventProgressionData();
    }

    private int GetUniqueId()
    {
        int id = 1;
        bool uniqueIdFound = false;
        while (!uniqueIdFound)
        {
            uniqueIdFound = true;
            for (int i = 0; i < TriggerEvents.Count; i++)
            {
                if (id == TriggerEvents[i].Id)
                {
                    uniqueIdFound = false;
                    id++;
                    break;
                }
            }
        }
        return id;
    }
}

[Serializable]
public class TriggerContainer
{
    public List<TriggerEvent> EventData;

    public TriggerContainer()
    {
        EventData = new List<TriggerEvent>();
    }
}

[Serializable]
public class TriggerEvent
{
    public int Id;
    public List<string> Dialogue;
    public TriggerHandler.EventType EventType;
    public TriggerHandler.EventId EventId;
    public bool ShowOnce;
    public bool ShowOnCompletedLevel;
    public bool ShowOnRestart;
    public bool HasDependency;
    public TriggerHandler.DependencyId DependencyId;
    public TriggerHandler.ForceOptions ForceShow;

    public TriggerEvent()
    {
        Id = 0;
        Dialogue = new List<string>();
        EventType = TriggerHandler.EventType.Dialogue;
        EventId = TriggerHandler.EventId.None;
        ShowOnce = false;
        ShowOnCompletedLevel = true;
        ShowOnRestart = true;
        HasDependency = false;
        DependencyId = TriggerHandler.DependencyId.None;
        ForceShow = TriggerHandler.ForceOptions.Never;
    }
}

[Serializable]
public class TriggerProgressionContainer
{
    public List<TriggerProgressionData> TriggerData;

    public TriggerProgressionContainer()
    {
        TriggerData = new List<TriggerProgressionData>();
    }
}

[Serializable]
public class TriggerProgressionData
{
    public int Id;
    public bool TooltipSeen;

    public TriggerProgressionData()
    {
        Id = 0;
        TooltipSeen = false;
    }
}