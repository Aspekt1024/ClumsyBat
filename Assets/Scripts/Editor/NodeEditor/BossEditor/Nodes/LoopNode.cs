using UnityEngine;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class LoopNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, 0);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30f, 0);
    }

    public override void Draw()
    {
        WindowTitle = "Loop to Start";
        Transform.Width = 120;
        Transform.Height = 40;

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new LoopAction();
    }

}
