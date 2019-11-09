using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ifaces = RandomOutAction.Ifaces;
using RandomOutTypes = RandomOutAction.RandomOutTypes;

public class RandomOutNode : BaseNode
{
    public int NumOutputs = 2;
    public RandomOutTypes RandomType;
    public List<float> OutputWeights = new List<float>();

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
            SetInterface(i, i + 3);
        }
    }
    
    public override void SetupNode(BehaviourSet behaviour)
    {
        base.SetupNode(behaviour);
        OutputWeights.Add(1);
        OutputWeights.Add(1);
    }

    public override void Draw()
    {
        CheckForListCountChange();

        Transform.Width = 140;
        Transform.Height = 70 + 20 * interfaces.Count;
        WindowTitle = "Random Out";

        RandomType = (RandomOutTypes)NodeGUI.EnumPopupLayout("Type", RandomType);
        NumOutputs = Mathf.Max(NodeGUI.IntFieldLayout("Outputs:", NumOutputs, 0.7f), 2);

        if (RandomType == RandomOutTypes.Weighted)
        {
            ShowOutputs();
        }
        
        SetInterfacePositions();
        DrawInterfaces();
    }

    private void ShowOutputs()
    {
        for (int i = 0; i < OutputWeights.Count; i++)
        {
            Vector2 startPos = new Vector2(38f, 83f + i * 20f);
            NodeGUI.Label(new Rect(startPos.x, startPos.y, 50f, 25f), "weight:");
            OutputWeights[i] = NodeGUI.FloatField(new Rect(startPos.x + 50f, startPos.y, 35f, 20f), OutputWeights[i], "", 0.01f);
        }
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
            OutputWeights.Add(1);
        }
        while (interfaces.Count - 1 > NumOutputs)
        {
            interfaces.Remove(interfaces[interfaces.Count - 1]);
            OutputWeights.RemoveAt(OutputWeights.Count - 1);
        }
    }

    public override BaseAction GetAction()
    {
        return new RandomOutAction()
        {
            OutputWeights = OutputWeights,
            RandomType = RandomType
        };
    }
}
