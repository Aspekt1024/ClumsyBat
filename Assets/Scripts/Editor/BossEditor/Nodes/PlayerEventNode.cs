using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using IODirection = NodeInterface.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using PlayerEvents = PlayerEventAction.PlayerEvents;

public class PlayerEventNode : BaseNode {

    [SerializeField]
    private PlayerEvents playerEvent;
    [SerializeField]
    private int selectedPlayerEventIndex;

    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, 0);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30, 0);
    }

    public override void Draw()
    {
        WindowTitle = "Player Event";
        Transform.Width = 120;
        Transform.Height = 60;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        var playerEventArray = Enum.GetValues(typeof(PlayerEvents));
        var playerEventStringArray = EditorHelpers.GetEnumStringArray(typeof(PlayerEvents));
        selectedPlayerEventIndex = EditorGUILayout.Popup(selectedPlayerEventIndex, playerEventStringArray);
        playerEvent = (PlayerEvents)playerEventArray.GetValue(selectedPlayerEventIndex);

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<PlayerEventAction>();
        ((PlayerEventAction)Action).PlayerEvent = playerEvent;
    }
}
