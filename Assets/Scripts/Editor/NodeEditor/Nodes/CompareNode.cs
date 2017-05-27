using System;
using System.Xml.Serialization;
using UnityEngine;

using Ifaces = CompareAction.Ifaces;
using Operations = CompareAction.Operations;

using InterfaceTypes = NodeInterface.InterfaceTypes;

public class CompareNode : BaseNode {

    // Keep public for serialisation, or use DataContract
    public Operations Operation;
    public float Input1;
    public float Input2;
    public float Input3;

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);

        AddInput((int)Ifaces.In1, InterfaceTypes.Object);
        AddInput((int)Ifaces.In2, InterfaceTypes.Object);
        AddInput((int)Ifaces.In3, InterfaceTypes.Object);

        AddOutput((int)Ifaces.OutputTrue);
        AddOutput((int)Ifaces.OutputFalse);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
        SetInterface((int)Ifaces.In1, 3);
        SetInterface((int)Ifaces.In2, 4);
        SetInterface((int)Ifaces.In3, 5);

        SetInterface((int)Ifaces.OutputTrue, 3, "True");
        SetInterface((int)Ifaces.OutputFalse, 4, "False");
    }

    public override void Draw()
    {
        Transform.Width = 120;
        WindowTitle = "Comparison";
        
        Operation = (Operations)NodeGUI.EnumPopupLayout("", Operation, 0.01f);

        DisplayInputValues();
        
        if (Operation != Operations.InRange && Operation != Operations.NotInRange)
        {
            HideInterface((int)Ifaces.In3);
            Transform.Height = 110;
        }
        else
        {
            ShowInterface((int)Ifaces.In3);
            Transform.Height = 130;
        }

        SetInterfacePositions();
        DrawInterfaces();
    }

    private void DisplayInputValues()
    {
        NodeGUI.Space(0.7f);

        Vector2 pos = NodeGUI.GetPosition();
        Rect rect = new Rect(pos.x + 2f, pos.y, 35f, NodeGUI.RowHeight);

        if (!GetInterface((int)Ifaces.In1).IsConnected())
            Input1 = NodeGUI.FloatField(new Rect(rect.position, rect.size), Input1, "", 0.01f);

        if (!GetInterface((int)Ifaces.In2).IsConnected())
            Input2 = NodeGUI.FloatField(new Rect(rect.position + new Vector2(0, NodeGUI.RowHeight) , rect.size), Input2, "", 0.01f);
        
        if (!GetInterface((int)Ifaces.In3).IsConnected() && !GetInterface((int)Ifaces.In3).IsHidden)
            Input3 = NodeGUI.FloatField(new Rect(rect.position + new Vector2(0, NodeGUI.RowHeight * 2), rect.size), Input3, "", 0.01f);
    }

    public override BaseAction GetAction()
    {
        return new CompareAction()
        {
            OperationType = Operation,
            Input1 = Input1,
            Input2 = Input2,
            Input3 = Input3
        };
    }
}
