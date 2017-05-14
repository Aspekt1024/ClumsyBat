using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Outputs = JumpAction.Outputs;

public class JumpNode : BaseNode {
    
    [SerializeField]
    private float jumpForce = 300f;

    private const float minJumpForce = 300f;
    private const float maxJumpForce = 1200f;

    protected override void AddInterfaces()
    {
        AddInput();

        AddOutput((int)Outputs.Jumped);
        AddOutput((int)Outputs.Top);
        AddOutput((int)Outputs.Landed);
    }

    private void SetInterfacePositions()
    {
        SetInput(30);
        SetOutput(30, (int)Outputs.Jumped, "next");
        SetOutput(50, (int)Outputs.Top, "Top");
        SetOutput(70, (int)Outputs.Landed, "Landed");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Jump";
        WindowRect.width = 15 * NodeGUIElements.GridSpacing;
        WindowRect.height = 9 * NodeGUIElements.GridSpacing;

        if (jumpForce < minJumpForce) jumpForce = minJumpForce;

        jumpForce = NodeGUIElements.VerticalSlider(this, new Rect(2, 2, 2, 6), jumpForce, minJumpForce, maxJumpForce, 50f, "Height");
        
        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<JumpAction>();
        ((JumpAction)Action).JumpForce = jumpForce;
    }
}
