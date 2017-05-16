
public class StartNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetOutput(30f);
    }

    public override void Draw()
    {
        Transform.Width = 80;
        Transform.Height = 40;
        WindowTitle = "Start";

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<StartAction>();
    }
}
