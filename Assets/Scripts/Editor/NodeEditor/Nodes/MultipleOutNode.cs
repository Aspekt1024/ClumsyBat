using System;
using System.Xml.Serialization;
using UnityEngine;


public class MultipleOutNode : BaseNode {

    public int NumOutputs = 2;

    protected override void AddInterfaces()
    {
        AddInput(0);
        AddOutput(1);
        AddOutput(2);
    }

    private void SetInterfacePositions()
    {
        SetInterface(0, 1);

        for (int i = 1; i < interfaces.Count; i++)
        {
            SetInterface(i, i);
        }
    }
    
    public override void Draw()
    {
        CheckForListCountChange();

        Transform.Width = 100;
        Transform.Height = 10 + 20 * interfaces.Count;
        WindowTitle = "M-Out";

        NumOutputs = Mathf.Max(NodeGUI.IntFieldLayout("Outputs:", NumOutputs, 0.7f), 2);

        SetInterfacePositions();
        DrawInterfaces();
    }
    
    private void CheckForListCountChange()
    {
        if (Event.current.type == EventType.KeyDown && NumOutputs != interfaces.Count - 1)
        {
            if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
            {
                AdjustOutputCount();
            }
        }
    }

    private void AdjustOutputCount()
    {
        while (interfaces.Count - 1 < NumOutputs)
        {
            AddOutput(interfaces.Count);
        }
        while (interfaces.Count - 1 > NumOutputs)
        {
            interfaces.Remove(interfaces[interfaces.Count-1]);
        }
    }
    
    public override BaseAction GetAction()
    {
        return new MultipleOutAction();
    }
}
