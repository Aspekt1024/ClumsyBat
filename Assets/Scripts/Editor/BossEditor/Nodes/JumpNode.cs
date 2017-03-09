using System;
using System.Collections.Generic;

using Outputs = JumpAction.Outputs;

public class JumpNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInput();

        AddOutput((int)Outputs.Jumped);
        AddOutput((int)Outputs.Top);
        AddOutput((int)Outputs.Landed);
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);
        SetOutput(40, (int)Outputs.Jumped, "Jumped");
        SetOutput(60, (int)Outputs.Top, "Top");
        SetOutput(80, (int)Outputs.Landed, "Landed");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Jump";
        WindowRect.width = 150;
        WindowRect.height = 100;

        
        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction ConvertNodeToAction()
    {
        return new JumpAction();
    }
}
