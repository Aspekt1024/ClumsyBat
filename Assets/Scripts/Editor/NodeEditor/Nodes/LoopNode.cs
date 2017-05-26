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
        SetInterface(0, 1);
    }

    public override void Draw()
    {
        WindowTitle = "Loop to Start";
        Transform.Width = 120;
        Transform.Height = 50;

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new LoopAction();
    }

}
