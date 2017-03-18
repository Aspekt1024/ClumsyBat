using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddOutput(0, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetOutput(25f, 0, "Object");
    }

    public override void DrawWindow()
    {
        WindowRect.width = 80;
        WindowRect.height = 40;
        WindowTitle = "Boss";

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<BossAction>();
    }
}
