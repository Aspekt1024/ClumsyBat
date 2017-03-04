public class StartNode : BaseNode {
    
    public override void SetupNode()
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

    public override void Activate()
    {
        CallNext();
    }
}
