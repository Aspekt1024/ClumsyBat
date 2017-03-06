using UnityEngine;

public class StartNode : BaseNode {

    public override void SetupNode(BossDataContainer dataContainer)
    {
        DataContainer = dataContainer;
        SaveThisNodeAsset();

        Action = CreateInstance<StartAction>();
        SaveActionAsset();

        AddOutput();

        UpdateActionInterfaces();
    }

    private void SetInterfacePositions()
    {
        CreateOutput(25f);
    }

    public override void DrawWindow()
    {
        WindowRect.width = 80;
        WindowRect.height = 40;
        WindowTitle = "Start";

        SetInterfacePositions();
        DrawInterfaces();
    }

}
