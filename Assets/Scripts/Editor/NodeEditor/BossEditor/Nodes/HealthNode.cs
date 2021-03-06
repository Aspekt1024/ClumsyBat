﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Ifaces = HealthAction.Ifaces;
using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class HealthNode : BaseNode {

    public int Health;

    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, (int)Ifaces.Input);
        AddInterface(IODirection.Output, (int)Ifaces.Output);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
        SetInterface((int)Ifaces.Output, 1);
    }

    public override void Draw()
    {
        Transform.Width = 80;
        Transform.Height = 60;
        WindowTitle = "Health";

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 40f;
        Health = EditorGUILayout.IntField("HP:", Health);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new HealthAction()
        {
            Health = Health
        };
    }

}
