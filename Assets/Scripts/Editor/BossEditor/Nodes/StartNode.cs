using System;
using UnityEngine;

public class StartNode : BaseNode {

    protected override void AddInterfaces()
    {
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetOutput(25f);
    }

    public override void DrawWindow()
    {
        WindowRect.width = 80;
        WindowRect.height = 40;
        WindowTitle = "Start";

        SetInterfacePositions();
        DrawInterfaces();
    }

}
