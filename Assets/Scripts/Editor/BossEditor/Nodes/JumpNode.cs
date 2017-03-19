using System;
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
        SetInput(50);
        SetOutput(50, (int)Outputs.Jumped, "Jumped");
        SetOutput(70, (int)Outputs.Top, "Top");
        SetOutput(90, (int)Outputs.Landed, "Landed");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Jump";
        WindowRect.width = 120;
        WindowRect.height = 105;

        if (jumpForce < 300f) jumpForce = 300f;

        EditorGUIUtility.labelWidth = 50;
        jumpForce = EditorGUILayout.FloatField("Force:", jumpForce);
        jumpForce = GUI.VerticalSlider(new Rect(new Vector2(25, 40), new Vector2(15, 60)), jumpForce, 1200, 300);
        jumpForce = Mathf.Round(jumpForce / 10) * 10f;
        
        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<JumpAction>();
        ((JumpAction)Action).JumpForce = jumpForce;
    }
}
