using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EndNode : BaseNode {
    

    public override void SetupNode()
    {
        AddInput(id:1);
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);
    }

    public override void DrawWindow()
    {
        WindowTitle = "End";
        WindowRect.width = 100;
        WindowRect.height = 80;

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override void Activate()
    {
    }
}