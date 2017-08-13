using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Xml.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TriggerEventSerializer : MonoBehaviour {

    public List<TriggerEvent> TriggerEvents;

    private static TriggerEventSerializer triggerEventSerializer;
    public static TriggerEventSerializer Instance
    {
        get
        {
            if (!triggerEventSerializer)
            {
                triggerEventSerializer = FindObjectOfType<TriggerEventSerializer>();
                if (!triggerEventSerializer)
                    Debug.LogError("triggerEventSerializer does not exist. Attach one to a game object");
                else
                    triggerEventSerializer.Init();
            }
            return triggerEventSerializer;
        }
    }

    private const string resourceLoadPath = "EventData/TriggerEventData";
    private const string savePath = "Assets/Resources/EventData/TriggerEventData.xml";
    private const string progressionSavePath = "TriggerEventData.dat";

    public TriggerEvent CreateNewTriggerEvent()
    {
        Load();
        TriggerEvent triggerEvent = new TriggerEvent();
        triggerEvent.Id = GetUniqueId();
        TriggerEvents.Add(triggerEvent);
        Save();
        return triggerEvent;
    }

    public TriggerEvent GetTriggerEvent(int id)
    {
        foreach(TriggerEvent te in TriggerEvents)
        {
            if (te.Id == id)
                return te;
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
                TriggerEvents.Remove(te);
                break;
            }
        }
        Save();
    }

    public void SaveEventProgressionData()
    {
        var bf = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/" + progressionSavePath, FileMode.OpenOrCreate);

        var data = new TriggerContainer { EventData = TriggerEvents };

        bf.Serialize(file, data);
        file.Close();
    }

    public void Save()
    {
        TriggerContainer data = new TriggerContainer();
        data.EventData = Instance.TriggerEvents;

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
            Instance.TriggerEvents = new List<TriggerEvent>();
            Save();
        }
        else
        {
            using (var stream = new StringReader(dataText.text))
            {
                data = (TriggerContainer)serializer.Deserialize(stream);
            }
            Instance.TriggerEvents = data.EventData;
        }
    }

    public void LoadEventProgressionData()
    {
        if (File.Exists(Application.persistentDataPath + "/" + progressionSavePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + progressionSavePath, FileMode.Open);

            TriggerContainer data;
            try
            {
                data = (TriggerContainer)bf.Deserialize(file);
            }
            catch
            {
                data = new TriggerContainer();
                Debug.Log("Unable to load existing trigger event data set");
            }
            file.Close();

            TriggerEvents = data.EventData;
        }
    }

    public void ClearEventData()
    {
        //TriggerEvents = new List<TriggerEvent>();
        //Save();
    }

    private void Init()
    {
        TriggerEvents = new List<TriggerEvent>();
        Load();
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
    public float TooltipDuration;
    public bool PausesGame;
    public bool ShowOnce;
    public bool ShowOnCompletedLevel;
    public bool ShowOnRestart;

    public TriggerEvent()
    {
        Id = 0;
        Dialogue = new List<string>();
        EventType = TriggerHandler.EventType.Dialogue;
        TooltipDuration = 3f;
        PausesGame = false;
        ShowOnce = false;
        ShowOnCompletedLevel = false;
        ShowOnRestart = false;
    }
}