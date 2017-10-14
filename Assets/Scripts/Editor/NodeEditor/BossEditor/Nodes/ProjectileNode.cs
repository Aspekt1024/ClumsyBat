using UnityEngine;
using UnityEditor;
using System;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = ProjectileAction.Ifaces;

public class ProjectileNode : BaseNode {
    
    public bool IsTargettingGround;
    public Vector2 TargetPos;
    public float ProjectileSpeed = 12.5f;
    public ProjectileAction.ProjectileTypes ProjectileType;
    public Moth.MothColour MothColour;

    private bool positionInputIsConnected;

    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, (int)Ifaces.Main);
        AddInterface(IODirection.Input, (int)Ifaces.Position, InterfaceTypes.Object);

        AddInterface(IODirection.Output, (int)Ifaces.Launched);
        AddInterface(IODirection.Output, (int)Ifaces.Landed);
        AddInterface(IODirection.Output, (int)Ifaces.HitPlayer);
        AddInterface(IODirection.Output, (int)Ifaces.Projectile, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Main, 1);
        SetInterface((int)Ifaces.Position, 8, "Position");

        SetInterface((int)Ifaces.Launched, 1, "Launched");
        SetInterface((int)Ifaces.HitPlayer, 2, "Player Hit");
        SetInterface((int)Ifaces.Landed, 3, "Landed");
        SetInterface((int)Ifaces.Projectile, 8, "Projectile");
    }

    public override void Draw()
    {
        WindowTitle = "Projectile";
        Transform.Width = 200;
        Transform.Height = 250;
        
        NodeGUI.Space(3);
        ProjectileType = (ProjectileAction.ProjectileTypes)NodeGUI.EnumPopupLayout("Type", ProjectileType);

        if (ProjectileType == ProjectileAction.ProjectileTypes.MothCrystal)
        {
            MothColour = (Moth.MothColour)NodeGUI.EnumPopupLayout("Colour", MothColour);
        }
        else
        {
            NodeGUI.Space();
        }

        ProjectileSpeed = NodeGUI.FloatFieldLayout(ProjectileSpeed, "Speed:");

        NodeGUI.Space(2);
        IsTargettingGround = NodeGUI.ToggleLayout("Target Ground?", IsTargettingGround);
        ShowPositionGUI();
        
        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new ProjectileAction()
        {
            TargetGround = IsTargettingGround,
            TargetPos = TargetPos,
            ProjectileSpeed = ProjectileSpeed,
            ProjectileType = ProjectileType,
            MothColour = MothColour
        };
    }

    private void ShowPositionGUI()
    {
        if (Event.current.type == EventType.Layout) // Layout and Repaint events must have the same controls. Update controls on layout.
        {
            NodeInterface iface = GetInterface((int)Ifaces.Position);
            if (iface != null)
                positionInputIsConnected = GetInterface((int)Ifaces.Position).IsConnected();
            else
                positionInputIsConnected = false;
        }

        if (positionInputIsConnected)
        {
            string targetLabel = "Targetting " + GetInterface((int)Ifaces.Position).ConnectedInterface.Node.WindowTitle;
            NodeGUI.LabelLayout(targetLabel);
        }
        else
        {
            if (!IsTargettingGround)
            {
                TargetPos.x = NodeGUI.FloatFieldLayout(TargetPos.x, "x:");
                TargetPos.y = NodeGUI.FloatFieldLayout(TargetPos.y, "y:");
            }
            else
            {
                TargetPos.x = NodeGUI.HorizontalSliderLayout(TargetPos.x, -6.315f, 6.2f, "xPos");
                NodeGUI.Space();
            }
        }
    }
}
