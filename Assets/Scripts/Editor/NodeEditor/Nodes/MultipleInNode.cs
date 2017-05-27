using System;
using System.Xml.Serialization;
using UnityEngine;

public class MultipleInNode : BaseNode {

    public int NumInputs = 2;

    protected override void AddInterfaces()
    {
        AddOutput(0);
        AddInput(1);
        AddInput(2);
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
        WindowTitle = "M-In";

        NumInputs = Mathf.Max(NodeGUI.IntFieldLayout("Inputs:", NumInputs, 0.7f), 2);

        SetInterfacePositions();
        DrawInterfaces();
    }


    private void CheckForListCountChange()
    {
        if (Event.current.type == EventType.keyDown && NumInputs != interfaces.Count - 1)
        {
            if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
            {
                AdjustInputCount();
            }
        }
    }

    private void AdjustInputCount()
    {
        while (interfaces.Count - 1 < NumInputs)
        {
            AddInput(interfaces.Count);
        }
        while (interfaces.Count - 1 > NumInputs)
        {
            interfaces.Remove(interfaces[interfaces.Count - 1]);
        }
    }
    
    public override BaseAction GetAction()
    {
        return new MultipleInAction();
    }
}
