using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;


/// <summary>
/// A state within a State Machine or Behaviour Tree
/// </summary>
public class State : BehaviourSet {
    
    public List<StateEvent> StateEvents = new List<StateEvent>();
    
    public void SetupActions(BossData bossData, GameObject bossReference)
    {
        BossActionLoadHandler.Load(this);
        foreach (var action in Actions)
        {
            action.GameSetup(this, bossData, bossReference);
        }
    }

    public void BeginState()
    {
        IsEnabled = true;
        StartingAction.Activate();
    }

    public override void LoopToStart()
    {
        if (IsEnabled)
            StartingAction.Activate();
    }

    public override void Tick(float deltaTime)
    {
        foreach (var action in Actions)
        {
            action.Tick(deltaTime);
        }
    }

    public int AddNewStateEvent()
    {
        int id = GetUniqueEventID();
        StateEvent newEvent = new StateEvent(id, this, "New Event", false);
        StateEvents.Add(newEvent);
        return id;
    }

    public void RemoveStateEvent(int stateEventID)
    {
        for (int i = 0; i < StateEvents.Count; i++)
        {
            if (StateEvents[i].ID == stateEventID)
            {
                StateEvents.Remove(StateEvents[i]);
                return;
            }
        }
    }
    
    public int GetStateEventIndex(int eventID)
    {
        for (int i = 0; i < StateEvents.Count; i++)
        {
            if (StateEvents[i].ID == eventID)
                return i;
        }
        return -1;
    }

    private int GetUniqueEventID()
    {
        int id = 1;
        for (int i = 0; i < StateEvents.Count; i++)
        {
            if (StateEvents[i].ID == id)
            {
                id++;
                i = -1;
            }
        }
        return id;
    }
}

[Serializable]
public class StateEvent
{
    public int ID;
    public string EventName = "New Event";
    public bool CancelsState;
    [XmlIgnore] public State ParentState;

    public StateEvent() { }

    public StateEvent(int id, State state, string name, bool cancels)
    {
        ID = id;
        ParentState = state;
        EventName = name;
        CancelsState = cancels;
    }
}
