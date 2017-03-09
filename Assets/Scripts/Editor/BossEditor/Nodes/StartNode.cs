
public class StartNode : BaseNode {

    private int TestVar = 211;

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

    protected override void CreateAction()
    {
        Action = CreateInstance<StartAction>();
        ((StartAction)Action).TestVariable = TestVar;   // TODO remove after testing
    }
}
