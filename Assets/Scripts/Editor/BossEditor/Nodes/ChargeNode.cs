using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Outputs = ChargeAction.Outputs;

public class ChargeNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInput();
        AddOutput((int)Outputs.Charging);
        AddOutput((int)Outputs.HitWall);
        AddOutput((int)Outputs.Recovered);
    }

    private void SetInterfacePositions()
    {
        SetInput(30f);
        SetOutput(30f, (int)Outputs.Charging, "Charging");
        SetOutput(50f, (int)Outputs.HitWall, "Hit Wall");
        SetOutput(70f, (int)Outputs.Recovered, "Recovered");
    }

    public override void Draw()
    {
        Transform.Width = 110;
        Transform.Height = 82;
        WindowTitle = "Charge";

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<ChargeAction>();
    }

}
