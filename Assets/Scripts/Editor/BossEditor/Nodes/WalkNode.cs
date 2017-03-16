using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Outputs = WalkAction.Outputs;

public class WalkNode : BaseNode {

    protected override void AddInterfaces()
    {
        AddInput();

        AddOutput((int)Outputs.StartWalk);
        AddOutput((int)Outputs.EndWalk);
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);
        SetOutput(40, (int)Outputs.StartWalk, "Begin");
        SetOutput(60, (int)Outputs.EndWalk, "End");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Walk";
        WindowRect.width = 150;
        WindowRect.height = 80;


        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<WalkAction>();
    }
}
