using UnityEngine;
using UnityEditor;
using System;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = ProjectileAction.Ifaces;

public class ProjectileNode : BaseNode {

    [SerializeField]
    private bool targetGround;
    [SerializeField]
    private Vector2 targetPos;
    [SerializeField]
    private float projectileSpeed = 12.5f;

    private bool inputConnected;

    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, (int)Ifaces.Main);
        AddInterface(IODirection.Input, (int)Ifaces.ProjectileIn, InterfaceTypes.Object);
        AddInterface(IODirection.Input, (int)Ifaces.Position, InterfaceTypes.Object);

        AddInterface(IODirection.Output, (int)Ifaces.Launched);
        AddInterface(IODirection.Output, (int)Ifaces.Landed);
        AddInterface(IODirection.Output, (int)Ifaces.HitPlayer);
        AddInterface(IODirection.Output, (int)Ifaces.ProjectileOut, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Main, 1);
        SetInterface((int)Ifaces.ProjectileIn, 2, "Projectile Ref");
        SetInterface((int)Ifaces.Position, 6, "Position");

        SetInterface((int)Ifaces.Launched, 1, "Launched");
        SetInterface((int)Ifaces.HitPlayer, 2, "Player Hit");
        SetInterface((int)Ifaces.Landed, 3, "Landed");
        SetInterface((int)Ifaces.ProjectileOut, 10, "Projectile");
    }

    public override void Draw()
    {
        WindowTitle = "Projectile";
        Transform.Width = 200;
        Transform.Height = 230;
        
        NodeGUI.Space(3);
        projectileSpeed = NodeGUI.FloatFieldLayout(projectileSpeed, "Speed:");

        NodeGUI.Space(2);
        ShowPositionGUI();
        targetGround = NodeGUI.ToggleLayout("Target Ground?", targetGround);
        
        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new ProjectileAction()
        {
            TargetGround = targetGround,
            TargetPos = targetPos,
            ProjectileSpeed = projectileSpeed
        };
    }

    private void ShowPositionGUI()
    {
        if (Event.current.type == EventType.Layout) // Layout and Repaint events must have the same controls. Update controls on layout.
        {
            NodeInterface iface = GetInterface((int)Ifaces.Position);
            if (iface != null)
                inputConnected = GetInterface((int)Ifaces.Position).IsConnected();
            else
                inputConnected = false;
        }

        if (inputConnected)
        {
            NodeGUI.LabelLayout("(Player)");
        }
        else
        {
            if (!targetGround)
            {
                targetPos.x = NodeGUI.FloatFieldLayout(targetPos.x, "x:");
                targetPos.y = NodeGUI.FloatFieldLayout(targetPos.y, "y:");
            }
            else
            {
                targetPos.x = NodeGUI.HorizontalSliderLayout(targetPos.x, -6.315f, 6.2f, "xPos");
                NodeGUI.Space();
            }
        }
    }
}
