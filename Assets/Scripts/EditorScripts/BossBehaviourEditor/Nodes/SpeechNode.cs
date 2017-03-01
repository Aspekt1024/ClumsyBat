﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SpeechNode : BaseNode {
    
    public override void SetupNode()
    {
        AddInput();
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);
        SetOutput(WindowRect.height / 2);
    }

    public override void DrawWindow()
    {
        WindowRect.width = 200;
        WindowRect.height = 150;
        WindowTitle = "Speech";

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override void Activate()
    {
        FindObjectOfType<JumpPound>().Activate();
    }

}
