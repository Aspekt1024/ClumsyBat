
public class StartNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetOutput(30f);
    }

    public override void DrawWindow()
    {
        WindowRect.width = 80;
        WindowRect.height = 40;
        WindowTitle = "Start";

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<StartAction>();
    }
}
