using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ifaces = StateEventAction.Ifaces;
using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class StateEventNode : BaseNode
{
    public int StateEventID;
    
    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, (int)Ifaces.Input);
    }
    
    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
    }

    public override void Draw()
    {
        Debug.Log(this + " " + StateEventID);
        BossState parentState = (BossState)ParentEditor.StateMachine;
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

    public override void SetupNode(StateMachine stateMachine)
    {
        base.SetupNode(stateMachine);
        StateEventID = ((BossState)stateMachine).AddNewStateEvent();
    }

    public override void DeleteNode()
    {
        ((BossState)ParentEditor.StateMachine).RemoveStateEvent(StateEventID);
        base.DeleteNode();
    }

    public override BaseAction GetAction()
    {
        return new StateEventAction()
        {
            StateEventID = StateEventID
        };
    }
}
