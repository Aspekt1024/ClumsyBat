using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JumpNode : BaseNode {

    public bool ScreenShakeOnLand;

    private enum Outputs
    {
        Jumped,
        Landed
    }

    public override void SetupNode()
    {
        AddInput();
        AddOutput((int)Outputs.Jumped);
        AddOutput((int)Outputs.Landed);
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);
        SetOutput(40, (int)Outputs.Jumped, "Jumped");
        SetOutput(60, (int)Outputs.Landed, "Landed");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Jump";
        WindowRect.width = 150;
        WindowRect.height = 200;



        SetInterfacePositions();
        DrawInterfaces();
    }

    public override void Activate()
    {
        FindObjectOfType<JumpPound>().Activate();
        CallNext((int)Outputs.Jumped);
    }
}
