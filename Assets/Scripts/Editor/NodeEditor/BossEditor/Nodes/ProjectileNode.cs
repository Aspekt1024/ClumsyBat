using UnityEngine;
using UnityEditor;
using System;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = ProjectileAction.Ifaces;

// TODO rename to ProjectileNode- this will do both parabolic and straight
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
        SetInterface(25, (int)Ifaces.Main);
        SetInterface(60, (int)Ifaces.ProjectileIn, "Projectile");
        SetInterface(80, (int)Ifaces.Position, "Position");

        SetInterface(25, (int)Ifaces.Launched, "Launched");
        SetInterface(45, (int)Ifaces.HitPlayer, "Player Hit");
        SetInterface(65, (int)Ifaces.Landed, "Landed");
        SetInterface(95, (int)Ifaces.ProjectileOut, "Projectile");
    }

    public override void Draw()
    {
        WindowTitle = "Projectile";
        Transform.Width = 200;
        Transform.Height = 125;

        projectileSpeed = NodeGUI.FloatFieldLayout(projectileSpeed, "Speed:");
        NodeGUI.Space(4);
        ShowPositionGUI();

        // TODO toggle
        EditorGUIUtility.labelWidth = 100f;
        GUI.skin.label.alignment = TextAnchor.LowerRight;
        targetGround = EditorGUILayout.Toggle("Target Ground?", targetGround, GUI.skin.GetStyle("Toggle"));

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

                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.fieldWidth = 30f;
                EditorGUIUtility.labelWidth = 20f;
                GUI.Label(new Rect(10, 87, 20, 20), "x:");
                targetPos.x = EditorGUILayout.FloatField("    ", targetPos.x);
                EditorGUIUtility.labelWidth = 15f;
                GUI.Label(new Rect(62, 87, 20, 20), "y:");
                targetPos.y = EditorGUILayout.FloatField("    ", targetPos.y);
                EditorGUIUtility.labelWidth = 70f;
                EditorGUILayout.LabelField("");
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                NodeGUI.Space(2);
                NodeGUI.LabelLayout("xPos:");
                EditorGUIUtility.labelWidth = 40;
                EditorGUI.LabelField(new Rect(new Vector2(10, 90), new Vector2(40, 15)), "xPos:");
                targetPos.x = GUI.HorizontalSlider(new Rect(new Vector2(50, 90), new Vector2(70, 15)), targetPos.x, -6.315f, 6.2f);
            }
        }
    }
}
