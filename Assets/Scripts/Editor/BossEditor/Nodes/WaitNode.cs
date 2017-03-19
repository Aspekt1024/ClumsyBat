using UnityEngine;
using UnityEditor;
using System;

public class WaitNode : BaseNode {

    public float WaitTime = 1f;
    
    protected override void AddInterfaces()
    {
        AddInput();
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(30);
        SetOutput(30);
    }

    public override void DrawWindow()
    {
        WindowTitle = "Wait";
        WindowRect.width = 70;
        WindowRect.height = 40;

        EditorGUIUtility.labelWidth = 70f;
        WaitTime = EditorGUI.FloatField(new Rect(new Vector2(15, 18), new Vector2(WindowRect.width - 40, 18)), WaitTime);
        EditorGUI.LabelField(new Rect(new Vector2(WindowRect.width - 25, 18), new Vector2(10, 18)), "s");

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<WaitAction>();
        ((WaitAction)Action).WaitTime = WaitTime;
    }
}
