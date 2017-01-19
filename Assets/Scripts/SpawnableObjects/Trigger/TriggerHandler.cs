﻿using UnityEngine;
using System.Collections;

public class TriggerHandler {

    public TriggerHandler()
    {
        SetupTriggerPool();
        TooltipControl = GameObject.Find("Scripts").GetComponent<TooltipHandler>();
    }
    
    private const int NumTriggersInPool = 4;
    private const string TriggerResourcePath = "Interactables/Trigger";
    private TooltipHandler TooltipControl;

    public struct TriggerType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
        public EventType EventType;
        public TooltipHandler.DialogueId EventID;
    }

    public enum EventType
    {
        Dialogue,
        StoryEvent
    }
    
    TriggerClass[] Triggers = null;
    int Index = 0;

    private TriggerClass GetTriggerFromPool()
    {
        TriggerClass Trigger = Triggers[Index];
        Index++;
        if (Index == Triggers.Length)
        {
            Index = 0;
        }
        return Trigger;
    }

    public void SetupTriggerPool()
    {
        TriggerClass[] TriggerList = new TriggerClass[NumTriggersInPool];
        for (int i = 0; i < NumTriggersInPool; i++)
        {
            GameObject TriggerObj = (GameObject)MonoBehaviour.Instantiate(Resources.Load(TriggerResourcePath));
            TriggerClass Trigger = TriggerObj.GetComponent<TriggerClass>();
            TriggerObj.transform.position = Toolbox.Instance.HoldingArea;
            Trigger.SetTHandler(this);
            TriggerList[i] = Trigger;
        }
        Triggers = TriggerList;
        Index = 0;
    }

    public void SetupTriggersInList(TriggerType[] TriggerList, float XOffset)
    {
        foreach (TriggerType Trigger in TriggerList)
        {
            float TriggerZLayer = Toolbox.Instance.ZLayers["Trigger"];
            TriggerClass NewTrigger = GetTriggerFromPool();
            NewTrigger.transform.position = new Vector3(Trigger.Pos.x + XOffset, Trigger.Pos.y, TriggerZLayer);
            NewTrigger.transform.localScale = Trigger.Scale;
            NewTrigger.transform.localRotation = Trigger.Rotation;
            NewTrigger.ActivateTrigger(Trigger.EventType, Trigger.EventID);
        }
    }

    public void SetVelocity(float Speed)
    {
        foreach (TriggerClass Trigger in Triggers)
        {
            Trigger.SetSpeed(Speed);
        }
    }

    public void ActivateDialogue(TooltipHandler.DialogueId EventID)
    {
        TooltipControl.ShowDialogue(EventID);
    }
}
