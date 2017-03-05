
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

}
