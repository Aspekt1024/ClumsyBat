using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;


/// <summary>
/// A state within a State Machine or Behaviour Tree
/// </summary>
public class State : BehaviourSet {
    
    public string StateName = "State";
    public List<StateEvent> StateEvents = new List<StateEvent>();

    private StateAction action;

    public enum StateChangeTypes
    {
        Never, Health, NumLoops, Time
    }
    public StateChangeTypes StateChange;
    public int MoveOnHP;
    public float MoveAfterSeconds;
    public int MoveOnLoops;
    
    public bool DamagedByHypersonic;    // TODO remove these and use events
    public bool DamagedByStalactites;
    public bool DamagedByPlayer;
    
    private int numLoops;

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
        numLoops = 0;
        bEnabled = true;
        StartingAction.Activate();
    }

    public override void RequestLoopToStart()
    {
        numLoops++;
        if (StateChange == StateChangeTypes.NumLoops && numLoops > MoveOnLoops)
        {
            bEnabled = false;
            StartingAction.Activate();
        }
        else
        {
            StartingAction.Activate();
        }
    }

    public override void Tick(float deltaTime)
    {
        foreach (var action in Actions)
        {
            action.Tick(deltaTime);
        }
    }
    
    public void ActivateEvent(int stateEventID)
    {
        Debug.Log("event activated: " + stateEventID);
        // TODO cycle through all events
        // Call the output interface on the interface ID holding this event
    }

    public void RemoveStateEvent(int stateEventID)
    {
        // TODO remove connection from connections list in action
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
            {
                return i;
            }
        }
        return -1;
    }

    public int AddNewStateEvent()
    {
        int id = GetUniqueEventID();
        StateEvent newEvent = new StateEvent(id, this, "New Event", false);
        StateEvents.Add(newEvent);
        return id;
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
    [XmlIgnore] public State ParentState;       // Allows access for the editor GUI
    public StateAction ParentAction;    // TODO Holds the runtime interface

    public StateEvent() { }

    public StateEvent(int id, State state, string name, bool cancels)
    {
        ID = id;
        ParentState = state;
        EventName = name;
        CancelsState = cancels;
    }
}
