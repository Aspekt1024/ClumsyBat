using UnityEngine;
using UnityEditor;
using System;

using Inputs = ProjectileAction.Inputs;
using Outputs = ProjectileAction.Outputs;

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
        AddInput((int)Inputs.Main);
        AddInput((int)Inputs.Projectile, InterfaceTypes.Object);
        AddInput((int)Inputs.Position, InterfaceTypes.Object);

        AddOutput((int)Outputs.Launched);
        AddOutput((int)Outputs.Landed);
        AddOutput((int)Outputs.HitPlayer);
        AddOutput((int)Outputs.Projectile, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInput(25, (int)Inputs.Main);
        SetInput(60, (int)Inputs.Projectile, "Projectile");
        SetInput(80, (int)Inputs.Position, "Position");

        SetOutput(25, (int)Outputs.Launched, "Launched");
        SetOutput(45, (int)Outputs.HitPlayer, "Player Hit");
        SetOutput(65, (int)Outputs.Landed, "Landed");
        SetOutput(95, (int)Outputs.Projectile, "Projectile");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Projectile";
        WindowRect.width = 200;
        WindowRect.height = 125;

        projectileSpeed = NodeGUIElements.FloatField(new Vector2(10, 30), projectileSpeed, "Speed:");
        AddSpaces(11);
        ShowPositionGUI();
        EditorGUIUtility.labelWidth = 100f;
        GUI.skin.label.alignment = TextAnchor.LowerRight;
        targetGround = EditorGUILayout.Toggle("Target Ground?", targetGround, GUI.skin.GetStyle("Toggle"));

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<ProjectileAction>();
        ((ProjectileAction)Action).TargetGround = targetGround;
        ((ProjectileAction)Action).TargetPos = targetPos;
        ((ProjectileAction)Action).ProjectileSpeed = projectileSpeed;
    }

    private void AddSpaces(int numSpaces)
    {
        for (int i = 0; i < numSpaces; i++)
        {
            EditorGUILayout.Space();
        }
    }

    private void ShowPositionGUI()
    {
        if (Event.current.type == EventType.Layout) // Layout and Repaint events must have the same controls. Update controls on layout.
            inputConnected = InputIsConnected((int)Inputs.Position);

        if (inputConnected)
        {
            EditorGUILayout.LabelField("   (Player)");
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
                AddSpaces(3);
                EditorGUIUtility.labelWidth = 40;
                EditorGUI.LabelField(new Rect(new Vector2(10, 90), new Vector2(40, 15)), "xPos:");
                targetPos.x = GUI.HorizontalSlider(new Rect(new Vector2(50, 90), new Vector2(70, 15)), targetPos.x, -6.315f, 6.2f);
            }
        }
    }
}
