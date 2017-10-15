using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ifaces = RepeatAction.Ifaces;

public class RepeatNode : BaseNode {

    public int RepeatCount;
    public float DelayDuration;

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
        AddOutput((int)Ifaces.Action);
        AddOutput((int)Ifaces.Complete);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
        SetInterface((int)Ifaces.Action, 1, "Action");
        SetInterface((int)Ifaces.Complete, 2, "Complete");
    }

    public override void Draw()
    {
        WindowTitle = "Repeat";
        WindowRect.size = new Vector2(100, 120);

        NodeGUI.Space(2);

        if (RepeatCount < 2) RepeatCount = 2;
        RepeatCount = NodeGUI.IntFieldLayout("n =", RepeatCount);
        DelayDuration = NodeGUI.FloatFieldLayout(DelayDuration, "dly");

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new RepeatAction()
        {
            RepeatCount = RepeatCount,
            DelayDuration = DelayDuration
        };
    }
}
