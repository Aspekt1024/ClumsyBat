using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using StalActions = SpawnStalAction.StalActions;
using StalSpawnDirection = SpawnStalAction.StalSpawnDirection;
using Ifaces = SpawnStalAction.Ifaces;
using StalSpawnType = SpawnStalAction.StalSpawnType;

public class SpawnStalNode : BaseNode {

    public StalActions selectedStalAction;
    public int selectedStalActionIndex;

    public StalSpawnDirection SpawnDirection;
    public int selectedSpawnDirectionIndex;
    
    // private, made public for XML serialization
    public int NumStals = 1;
    public List<StalSpawnType> StalSpawns = new List<StalSpawnType>();

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
        AddOutput((int)Ifaces.Output);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
        SetInterface((int)Ifaces.Output, 1);
    }

    public override void Draw()
    {
        WindowTitle = "Stalactites";
        Transform.Width = 180;
        Transform.Height = 130 + NumStals * 20;

        NodeGUI.Space();
        selectedStalAction = (StalActions)NodeGUI.EnumPopupLayout("Stal action:", selectedStalAction);

        if (selectedStalAction != StalActions.Drop)
        {
            SpawnDirection = (StalSpawnDirection)NodeGUI.EnumPopupLayout("Direction:", SpawnDirection);
            DisplayStalList();
        }
        else
        {
            Transform.Height = 80f;    // TODO this just hides the stal pos input interface set if it exists
        }

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new SpawnStalAction()
        {
            StalAction = selectedStalAction,
            SpawnDirection = SpawnDirection,
            stalSpawns = StalSpawns
        };
    }

    public override void SetupNode(BehaviourSet behaviour)
    {
        base.SetupNode(behaviour);
        AddNewStalSpawn();
    }

    private void DisplayStalList()
    {
        CheckForListCountChange();
        NumStals = Mathf.Max(1, NodeGUI.IntFieldLayout("Num Stals:", NumStals));

        for (int i = 0; i < StalSpawns.Count; i++)
        {
            Vector2 startPos = new Vector2(15f, 120f + i * 20f);

            var spawn = StalSpawns[i];
            SetInterface(spawn.inputID, i + 6);

            NodeInterface iface = GetInterface(spawn.inputID);
            if (iface.IsConnected())
            {
                NodeGUI.Label(new Rect(startPos.x, startPos.y, 150f, 20f), "At " + iface.ConnectedInterface.GetNode().WindowTitle);
            }
            else
            {
                NodeGUI.Label(new Rect(startPos.x, startPos.y, 30f, 20f), "rng:");
                EditorGUI.MinMaxSlider(new Rect(startPos.x + 30, startPos.y + 3f, 65f, 20), ref spawn.xPosStart, ref spawn.xPosEnd, -6.2f, 6.2f);
            }
            NodeGUI.Label(new Rect(startPos.x + 100f, startPos.y, 40f, 20f), "dly:");
            spawn.delay = NodeGUI.FloatField(new Rect(startPos.x + 125f, startPos.y, 27f, 20f), spawn.delay, "", 0.01f);
            StalSpawns[i] = spawn;
        }
    }

    private void CheckForListCountChange()
    {
        if (Event.current.type == EventType.keyDown && NumStals != StalSpawns.Count)
        {
            if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                AdjustStalSpawnListCount();
        }
    }

    private void AdjustStalSpawnListCount()
    {
        while (StalSpawns.Count < NumStals)
        {
            AddNewStalSpawn();
        }
        while (StalSpawns.Count > NumStals)
        {
            RemoveLastStalSpawn();
        }
    }

    private void AddNewStalSpawn()
    {
        var newSpawn = new StalSpawnType()
        {
            inputID = GetUniqueIfaceID()
        };
        StalSpawns.Add(newSpawn);
        AddInput(newSpawn.inputID, InterfaceTypes.Object);
    }

    private void RemoveLastStalSpawn()
    {
        interfaces[interfaces.Count].Disconnect();
        interfaces.Remove(interfaces[interfaces.Count -1]);
        StalSpawns.Remove(StalSpawns[StalSpawns.Count - 1]);
    }
    
    private int GetUniqueIfaceID()
    {
        int id = 2;
        for (int i = 0; i < interfaces.Count; i++)
        {
            if (id == interfaces[i].ID)
            {
                id++;
                i = -1;
            }
        }
        return id;
    }

}
