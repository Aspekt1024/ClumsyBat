﻿using System;
using System.Xml.Serialization;

public class StartNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddOutput(0);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30f, 0);
    }

    public override void Draw()
    {
        Transform.Width = 80;
        Transform.Height = 40;
        WindowTitle = "Start";

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new StartAction();
    }
}