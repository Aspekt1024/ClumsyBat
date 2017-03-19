using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HealthNode : BaseNode {

    [SerializeField]
    private int health;

    protected override void AddInterfaces()
    {
        AddInput();
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(30f);
        SetOutput(30f);
    }

    public override void DrawWindow()
    {
        WindowRect.width = 80;
        WindowRect.height = 60;
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
        Action = CreateInstance<HealthAction>();
        ((HealthAction)Action).Health = health;
    }

}
