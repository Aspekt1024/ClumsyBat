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
        SetInput(25);
        SetOutput(25);
    }

    public override void DrawWindow()
    {
        WindowTitle = "Wait";
        WindowRect.width = 140;
        WindowRect.height = 40;

        EditorGUIUtility.labelWidth = 70f;
        WaitTime = EditorGUI.FloatField(new Rect(new Vector2(15, 18), new Vector2(100, 18)), "Seconds:", WaitTime);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction ConvertNodeToAction()
    {
        throw new NotImplementedException();
    }
}
