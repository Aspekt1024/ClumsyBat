using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopNode : BaseNode {

    public override void SetupNode()
    {
        AddInput();
    }

    private void SetInterfacePositions()
    {
        SetInput(25);
    }

    public override void DrawWindow()
    {
        WindowTitle = "Loop to Start";
        WindowRect.width = 120;
        WindowRect.height = 40;

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override void Activate()
    {
        foreach(var node in Toolbox.Instance.Boss.BossProps.Nodes)
        {
            if (node.WindowTitle == "Start")
            {
                node.Activate();
            }
        }
    }

}
