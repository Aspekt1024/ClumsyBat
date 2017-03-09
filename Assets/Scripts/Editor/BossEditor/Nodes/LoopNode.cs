
using System;

public class LoopNode : BaseNode {

    protected override void AddInterfaces()
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

    public override BaseAction ConvertNodeToAction()
    {
        return new LoopAction();
    }

}
