
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
        foreach(var node in bossBehaviour.BossProps.CurrentState.Nodes)
        {
            if (node.GetType().Equals(typeof(StartNode)))
                node.Activate();
        }
    }

}
