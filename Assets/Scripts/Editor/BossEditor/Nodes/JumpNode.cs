using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using IODirection = NodeInterface.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = JumpAction.Ifaces;

public class JumpNode : BaseNode {
    
    [SerializeField]
    private float jumpForce = 300f;

    private const float minJumpForce = 300f;
    private const float maxJumpForce = 1200f;

    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, (int)Ifaces.Input);

        AddInterface(IODirection.Output, (int)Ifaces.Jumped);
        AddInterface(IODirection.Output, (int)Ifaces.Top);
        AddInterface(IODirection.Output, (int)Ifaces.Landed);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30, (int)Ifaces.Input);

        SetInterface(30, (int)Ifaces.Jumped, "next");
        SetInterface(50, (int)Ifaces.Top, "Top");
        SetInterface(70, (int)Ifaces.Landed, "Landed");
    }

    public override void Draw()
    {
        WindowTitle = "Jump";
        Transform.Width = 15 * NodeGUI.GridSpacing;
        Transform.Height = 9 * NodeGUI.GridSpacing;
        
        Vector2 pos = new Vector2(2, 2) * NodeGUI.GridSpacing;
        GUI.Label(new Rect(pos + new Vector2(15f, 0f), new Vector2(50, 20)), "Height");
        jumpForce = EditorGUI.FloatField(new Rect(pos + new Vector2(15f, 20f), new Vector2(35f, 15f)), jumpForce);
        jumpForce = GUI.VerticalSlider(new Rect(pos, new Vector2(2, 6) * NodeGUI.GridSpacing), jumpForce, maxJumpForce, minJumpForce);
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
