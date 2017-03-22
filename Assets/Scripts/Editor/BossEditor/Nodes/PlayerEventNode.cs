using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using PlayerEvents = PlayerEventAction.PlayerEvents;

public class PlayerEventNode : BaseNode {

    [SerializeField]
    private PlayerEvents playerEvent;
    [SerializeField]
    private int selectedPlayerEventIndex;

    protected override void AddInterfaces()
    {
        AddInput();
    }

    private void SetInterfacePositions()
    {
        SetInput(30);
    }

    public override void DrawWindow()
    {
        WindowTitle = "Player Event";
        WindowRect.width = 120;
        WindowRect.height = 60;

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
