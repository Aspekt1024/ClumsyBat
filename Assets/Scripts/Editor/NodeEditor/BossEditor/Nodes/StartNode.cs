
public class StartNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInterface(NodeInterface.IODirection.Output, 0);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30f, 0);
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
        Action = new StartAction();
    }
}
