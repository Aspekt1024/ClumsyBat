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
        
        Vector2 pos = new Vector2(2, 2) * NodeGUIElements.GridSpacing;
        GUI.Label(new Rect(pos + new Vector2(15f, 0f), new Vector2(50, 20)), "Height");
        jumpForce = EditorGUI.FloatField(new Rect(pos + new Vector2(15f, 20f), new Vector2(35f, 15f)), jumpForce);
        jumpForce = GUI.VerticalSlider(new Rect(pos, new Vector2(2, 6) * NodeGUIElements.GridSpacing), jumpForce, maxJumpForce, minJumpForce);
        jumpForce -= jumpForce % 50f;
        jumpForce = Mathf.Clamp(jumpForce, minJumpForce, maxJumpForce);

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<JumpAction>();
        ((JumpAction)Action).JumpForce = jumpForce;
    }
}
