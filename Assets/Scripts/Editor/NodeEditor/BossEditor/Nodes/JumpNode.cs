using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = JumpAction.Ifaces;

public class JumpNode : BaseNode {

    public float JumpForce = 300f;   // TODO because of this, we should be using the DataContractSerializer

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
        SetInterface((int)Ifaces.Input, 1);

        SetInterface((int)Ifaces.Jumped, 1, "next");
        SetInterface((int)Ifaces.Top, 2, "Top");
        SetInterface((int)Ifaces.Landed, 3, "Landed");
    }

    public override void Draw()
    {
        WindowTitle = "Jump";
        Transform.Width = 15 * NodeGUI.GridSpacing;
        Transform.Height = 10 * NodeGUI.GridSpacing;
        
        Vector2 pos = new Vector2(2, 3) * NodeGUI.GridSpacing;
        GUI.Label(new Rect(pos + new Vector2(15f, 0f), new Vector2(50, 20)), "Height");
        JumpForce = EditorGUI.FloatField(new Rect(pos + new Vector2(15f, 20f), new Vector2(35f, 15f)), JumpForce);
        JumpForce = GUI.VerticalSlider(new Rect(pos, new Vector2(2, 6) * NodeGUI.GridSpacing), JumpForce, maxJumpForce, minJumpForce);
        JumpForce -= JumpForce % 50f;
        JumpForce = Mathf.Clamp(JumpForce, minJumpForce, maxJumpForce);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new JumpAction()
        {
            JumpForce = JumpForce
        };
        //Action = new JumpAction();
        //((JumpAction)Action).JumpForce = jumpForce;
    }
}
