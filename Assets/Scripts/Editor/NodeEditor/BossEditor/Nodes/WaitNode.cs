using UnityEngine;
using UnityEditor;
using System;

using Iface = WaitAction.Ifaces;

public class WaitNode : BaseNode {

    public float WaitTime = 1f;
    
    protected override void AddInterfaces()
    {
        AddInput((int)Iface.Input);
        AddOutput((int)Iface.Output);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30, (int)Iface.Input);
        SetInterface(30, (int)Iface.Output);
    }

    public override void Draw()
    {
        WindowTitle = "Wait";
        Transform.Width = 70;
        Transform.Height = 40;

        EditorGUIUtility.labelWidth = 70f;
        WaitTime = EditorGUI.FloatField(new Rect(new Vector2(15, 18), new Vector2(Transform.Width - 40, 18)), WaitTime);
        EditorGUI.LabelField(new Rect(new Vector2(Transform.Width - 25, 18), new Vector2(10, 18)), "s");

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new WaitAction()
        {
            WaitTime = WaitTime
        };
    }
}
