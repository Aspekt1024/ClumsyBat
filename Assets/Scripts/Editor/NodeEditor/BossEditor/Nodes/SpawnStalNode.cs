using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using StalTypes = SpawnStalAction.StalTypes;
using StalActions = SpawnStalAction.StalActions;
using StalSpawnDirection = SpawnStalAction.StalSpawnDirection;
using Ifaces = SpawnStalAction.Ifaces;
using StalSpawnType = SpawnStalAction.StalSpawnType;

public class SpawnStalNode : BaseNode {

    public StalTypes StalType;
    public float GreenMothChance;
    public float BlueMothChance;
    public float GoldMothChance;

    public StalActions StalAction;
    public int StalActionIndex;

    public StalSpawnDirection SpawnDirection;
    public int SpawnDirectionIndex;
    
    // private, made public for XML serialization
    public int NumStals = 1;
    public List<StalSpawnType> StalSpawns = new List<StalSpawnType>();

    private float verticalShift;

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
        AddOutput((int)Ifaces.Output);

        AddInput((int)Ifaces.GreenChance);
        AddInput((int)Ifaces.GoldChance);
        AddInput((int)Ifaces.BlueChance);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
        SetInterface((int)Ifaces.Output, 1);

        SetInterface((int)Ifaces.GreenChance, 3);
        SetInterface((int)Ifaces.GoldChance, 4);
        SetInterface((int)Ifaces.BlueChance, 5);

        if (StalType != StalTypes.Crystal)
        {
            HideInterface((int)Ifaces.GreenChance);
            HideInterface((int)Ifaces.GoldChance);
            HideInterface((int)Ifaces.BlueChance);
        }
        else if (StalType == StalTypes.Crystal)
        {
            ShowInterface((int)Ifaces.GreenChance);
            ShowInterface((int)Ifaces.GoldChance);
            ShowInterface((int)Ifaces.BlueChance);
        }
    }

    public override void Draw()
    {
        if (StalType == StalTypes.Stalactite)
            WindowTitle = "Stalactites";
        else if (StalType == StalTypes.Crystal)
            WindowTitle = "Crystals";

        Transform.Width = 180;
        Transform.Height = 150 + NumStals * 20;

        NodeGUI.Space();
        StalType = (StalTypes)NodeGUI.EnumPopupLayout("Stal type:", StalType);
        verticalShift = 0f;
        if (StalType == StalTypes.Crystal)
            SelectMothColor();

        StalAction = (StalActions)NodeGUI.EnumPopupLayout("Stal action:", StalAction);

        if (StalAction != StalActions.Drop)
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

    private void SelectMothColor()
    {
        verticalShift = 80f;
        Transform.Height += verticalShift;

        NodeInterface greenIface = GetInterface((int)Ifaces.GreenChance);
        if (greenIface.IsConnected())
            NodeGUI.LabelLayout("Green chance: " + greenIface.ConnectedInterface.Node.GetValueString(greenIface.ConnectedIfaceID));
        else
            GreenMothChance = NodeGUI.FloatFieldLayout(GreenMothChance, "Green chance:", 0.7f);

        NodeInterface goldIface = GetInterface((int)Ifaces.GoldChance);
        if (goldIface.IsConnected())
            NodeGUI.LabelLayout("Gold chance: " + goldIface.ConnectedInterface.Node.GetValueString(goldIface.ConnectedIfaceID));
        else
            GoldMothChance = NodeGUI.FloatFieldLayout(GoldMothChance, "Gold chance:", 0.7f);

        NodeInterface blueIface = GetInterface((int)Ifaces.BlueChance);
        if (blueIface.IsConnected())
            NodeGUI.LabelLayout("Blue chance: " + blueIface.ConnectedInterface.Node.GetValueString(blueIface.ConnectedIfaceID));
        else
            BlueMothChance = NodeGUI.FloatFieldLayout(BlueMothChance, "Blue chance:", 0.7f);

        NodeGUI.Space();
    }

    public override BaseAction GetAction()
    {
        return new SpawnStalAction()
        {
            StalAction = StalAction,
            SpawnDirection = SpawnDirection,
            stalSpawns = StalSpawns,
            StalType = StalType,
            GreenChance = GreenMothChance,
            GoldChance = GoldMothChance,
            BlueChance = BlueMothChance
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
            Vector2 startPos = new Vector2(15f, 140f + i * 20f + verticalShift);

            var spawn = StalSpawns[i];
            SetInterface(spawn.inputID, i + 7 + Mathf.RoundToInt(verticalShift / 20f));

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
            inputID = GetUniqueIfaceID(),
            
        };
        StalSpawns.Add(newSpawn);
        AddInput(newSpawn.inputID, InterfaceTypes.Object);
    }

    private void RemoveLastStalSpawn()
    {
        interfaces[interfaces.Count - 1].Disconnect();
        interfaces.Remove(interfaces[interfaces.Count -1]);
        StalSpawns.Remove(StalSpawns[StalSpawns.Count - 1]);
    }
    
    private int GetUniqueIfaceID()
    {
        int id = 5;
        bool unique = false;

        while (!unique)
        {
            unique = true;
            for (int i = 0; i < interfaces.Count; i++)
            {
                if (id == interfaces[i].ID)
                {
                    id++;
                    unique = false;
                    break;
                }
            }
        }
        return id;
    }

}
