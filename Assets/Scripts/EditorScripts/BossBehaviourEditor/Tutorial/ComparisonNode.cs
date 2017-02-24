using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ComparisonNode : BaseInputNode {

    public enum ComparisonType
    {
        Greater, Less, Equal
    }
    private ComparisonType comparisonType;

    private BaseInputNode input1;
    private Rect input1Rect;
    private BaseInputNode input2;
    private Rect input2Rect;

    private string compareText = "";

    public ComparisonNode()
    {
        windowTitle = "Comparison Node";
        hasInputs = true;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;
        comparisonType = (ComparisonType)EditorGUILayout.EnumPopup("Comparison Type", comparisonType);

        string input1Title = "None";
        string input2Title = "None";

        if (input1)
            input1Title = input1.GetResult();
        if (input2)
            input2Title = input1.GetResult();

        GUILayout.Label("Input 1: " + input1Title);
        GUILayout.Label("Input 2: " + input2Title);

        if (e.type == EventType.Repaint)
        {
            input1Rect = GUILayoutUtility.GetLastRect();
            input2Rect = GUILayoutUtility.GetLastRect();
        }
    }
    
    public override void SetInput(BaseInputNode input, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        if (input1Rect.Contains(clickPos))
            input1 = input;
        else if (input2Rect.Contains(clickPos))
            input2 = input;
    }

    public override string GetResult()
    {
        float input1Value = 0;
        float input2Value = 0;

        if (input1)
        {
            string input1Raw = input1.GetResult();
            float.TryParse(input1Raw, out input1Value);
        }
        else if (input2)
        {
            string input2Raw = input2.GetResult();
            float.TryParse(input2Raw, out input2Value);
        }

        string result = "false";

        switch(comparisonType)
        {
            case ComparisonType.Equal:
                if (input1Value == input2Value)
                    result = "true";
                break;
            case ComparisonType.Greater:
                if (input1Value > input2Value)
                    result = "true";
                break;
            case ComparisonType.Less:
                if (input1Value < input2Value)
                    result = "true";
                break;
        }
        return result;
    }

    public override BaseInputNode ClickedOnInput(Vector2 pos)
    {
        BaseInputNode retVal = null;

        pos.x -= windowRect.x;
        pos.y -= windowRect.y;

        if (input1Rect.Contains(pos))
        {
            retVal = input1;
            input1 = null;
        }
        else if (input2Rect.Contains(pos))
        {
            retVal = input2;
            input2 = null;
        }

        return retVal;
    }

    public override void NodeDeleted(BaseNodeOld node)
    {
        if (node.Equals(input1))
            input1 = null;
        if (node.Equals(input2))
            input2 = null;
    }

    public override void DrawCurves()
    {
        if (input1)
        {
            Rect rect = windowRect;
            rect.x += input1Rect.x;
            rect.y += input1Rect.y + input1Rect.height / 2;
            rect.width = 1;
            rect.height = 1;
            NodeEditor.DrawNodeCurve(input1.windowRect, rect);
        }
        else if (input2)
        {
            Rect rect = windowRect;
            rect.x += input2Rect.x;
            rect.y += input2Rect.y + input2Rect.height / 2;
            rect.width = 1;
            rect.height = 1;
            NodeEditor.DrawNodeCurve(input1.windowRect, rect);
        }
    }
}
