using System;
using System.Collections.Generic;
using UnityEngine;

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

    public override BaseAction ConvertNodeToAction()
    {
        StartAction startAction = new StartAction()
        {
            inputs = new List<BaseAction.InterfaceType>(),
            outputs = new List<BaseAction.InterfaceType>(),
            TestVariable = TestVar
        };

        BaseAction.InterfaceType output = new BaseAction.InterfaceType()
        {
            identifier = outputs[0].identifier,
            connectedAction = null,
            connectedInterfaceIndex = -1
        };
        
        startAction.outputs.Add(output);

        return startAction;
    }
}
