using UnityEngine;

public class LoopNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInput();
    }

    private void SetInterfacePositions()
    {
        SetInput(30);
    }

    public override void Draw()
    {
        WindowTitle = "Loop to Start";
        Transform.Width = 120;
        Transform.Height = 40;

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<LoopAction>();
    }

}
