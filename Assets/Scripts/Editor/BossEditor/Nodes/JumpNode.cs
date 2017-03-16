﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Outputs = JumpAction.Outputs;

public class JumpNode : BaseNode {
    
    [SerializeField]
    private float jumpForce = 200;

    protected override void AddInterfaces()
    {
        AddInput();

        AddOutput((int)Outputs.Jumped);
        AddOutput((int)Outputs.Top);
        AddOutput((int)Outputs.Landed);
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);
        SetOutput(50, (int)Outputs.Jumped, "Jumped");
        SetOutput(70, (int)Outputs.Top, "Top");
        SetOutput(90, (int)Outputs.Landed, "Landed");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Jump";
        WindowRect.width = 180;
        WindowRect.height = 105;

        if (jumpForce < 300f) jumpForce = 300f;

        EditorGUIUtility.labelWidth = 70;
        jumpForce = EditorGUILayout.FloatField("Force:", jumpForce);
        
        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<JumpAction>();
        ((JumpAction)Action).JumpForce = jumpForce;
    }
}
