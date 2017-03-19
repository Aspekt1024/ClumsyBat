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
        WindowTitle = "Parabolic Projectile";
        WindowRect.width = 200;
        WindowRect.height = 125;

        EditorGUI.LabelField(new Rect(new Vector2(10, 30), new Vector2(45, 15)), "Speed:");
        projectileSpeed = EditorGUI.FloatField(new Rect(new Vector2(55, 30), new Vector2(40, 15)), projectileSpeed);
        AddSpaces(11);
        ShowPositionGUI();
        EditorGUIUtility.labelWidth = 100f;
        targetGround = EditorGUILayout.Toggle("Target Ground?", targetGround);

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
                EditorGUIUtility.labelWidth = 23f;
                EditorGUIUtility.fieldWidth = 30f;
                targetPos.x = EditorGUILayout.FloatField("  x:", targetPos.x);
                EditorGUIUtility.labelWidth = 15f;
                targetPos.y = EditorGUILayout.FloatField("y:", targetPos.y);
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
