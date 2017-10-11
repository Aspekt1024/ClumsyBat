using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using PlayerEvents = PlayerEventAction.PlayerEvents;

public class PlayerEventNode : BaseNode {
    
    public PlayerEvents playerEvent;
    public int selectedPlayerEventIndex;

    protected override void AddInterfaces()
    {
        AddInput(0);
    }

    private void SetInterfacePositions()
    {
        SetInterface(0, 1);
    }

    public override void Draw()
    {
        WindowTitle = "Player Event";
        Transform.Width = 140;
        Transform.Height = 60;
        
        playerEvent = (PlayerEvents)NodeGUI.EnumPopupLayout("Event:", playerEvent);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new PlayerEventAction()
        {
            PlayerEvent = playerEvent,
        };
    }
}
