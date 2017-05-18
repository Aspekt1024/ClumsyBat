using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using IODirection = NodeInterface.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class HealthNode : BaseNode {

    [SerializeField]
    private int health;

    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, 0);
        AddInterface(IODirection.Output, 1);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30f, 0);
        SetInterface(30f, 1);
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
        health = EditorGUILayout.IntField("HP:", health);

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        //Action = typeof(HealthAction);
        //((HealthAction)Action).Health = health;
    }

}
