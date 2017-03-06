
public class LoopNode : BaseNode {

    public override void SetupNode(BossDataContainer dataContainer)
    {
        DataContainer = dataContainer;
        SaveThisNodeAsset();
        
        Action = CreateInstance<LoopAction>();
        SaveActionAsset();

        AddInput();
        
        UpdateActionInterfaces();
    }

    private void SetInterfacePositions()
    {
        CreateInput(25);
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
