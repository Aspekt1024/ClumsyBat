﻿using UnityEngine;
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

    public override void Draw()
    {
        WindowTitle = "Wait";
        Transform.Width = 70;
        Transform.Height = 40;

        EditorGUIUtility.labelWidth = 70f;
        WaitTime = EditorGUI.FloatField(new Rect(new Vector2(15, 18), new Vector2(Transform.Width - 40, 18)), WaitTime);
        EditorGUI.LabelField(new Rect(new Vector2(Transform.Width - 25, 18), new Vector2(10, 18)), "s");

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<WaitAction>();
        ((WaitAction)Action).WaitTime = WaitTime;
    }
}
