﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class StartNode : BaseNode {
    
    public override void SetupNode()
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

    public override void Activate()
    {
        outputs[0].connectedNode.Activate();
    }
}
