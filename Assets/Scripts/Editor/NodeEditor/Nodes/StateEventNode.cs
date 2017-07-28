using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Ifaces = StateEventAction.Ifaces;
using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class StateEventNode : BaseNode
{
    public int StateEventID;
    
    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
    }
    
    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
    }

    public override void Draw()
    {
        nodeType = NodeTypes.State;

        State parentState = (State)ParentEditor.BehaviourSet;
        int stateEventIndex = parentState.GetStateEventIndex(StateEventID);
        if (stateEventIndex < 0) return;

        StateEvent stateEvent = parentState.StateEvents[stateEventIndex];
        WindowTitle = stateEvent.EventName;
        Transform.Width = 150;
        Transform.Height = 80;

        stateEvent.EventName = NodeGUI.TextFieldLayout(stateEvent.EventName, "Event Name:");
        stateEvent.CancelsState = NodeGUI.ToggleLayout("Stops State?", stateEvent.CancelsState);

        parentState.StateEvents[stateEventIndex] = stateEvent;

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override void SetupNode(BehaviourSet behaviour)
    {
        base.SetupNode(behaviour);
        StateEventID = ((State)behaviour).AddNewStateEvent();
        ((BossEditor)ParentEditor).AddEventToStateNode(StateEventID);

        SaveStateAsset((State)ParentEditor.BehaviourSet);
    }

    public override void DeleteNode()
    {
        ((State)ParentEditor.BehaviourSet).RemoveStateEvent(StateEventID);
        ((BossEditor)ParentEditor).RemoveEventFromStateNode(StateEventID);
        base.DeleteNode();

        SaveStateAsset((State)ParentEditor.BehaviourSet);
    }
    
    private void SaveStateAsset(State state)
    {
        EditorUtility.SetDirty(state);
    }

    public override BaseAction GetAction()
    {
        State parentState = (State)ParentEditor.BehaviourSet;
        int stateEventIndex = parentState.GetStateEventIndex(StateEventID);
        StateEvent stateEvent = parentState.StateEvents[stateEventIndex];
        
        return new StateEventAction()
        {
            StateEventID = StateEventID,
            StopsState = stateEvent.CancelsState
        };
    }
}
