using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class InputNode : BaseInputNode {

    private InputType inputType;

    public enum InputType
    {
        Number, Randomisation
    }

    private string randomFrom = "";
    private string randomTo = "";

    private string inputValue = "";

    public InputNode()
    {
        windowTitle = "Input Node";
    }

    public override void DrawWindow()
    {
        base.DrawWindow();
        inputType = (InputType)EditorGUILayout.EnumPopup("Input type : ", inputType);

        if (inputType == InputType.Number)
        {
            inputValue = EditorGUILayout.TextField("Value", inputValue);
        }
        else if (inputType == InputType.Randomisation)
        {
            randomFrom = EditorGUILayout.TextField("Value", randomFrom);
            randomTo = EditorGUILayout.TextField("Value", randomTo);

            if (GUILayout.Button("Calculate random"))
            {
                CalculateRandom();
            }
        }
    }

    public override void DrawCurves()
    {
    }

    private void CalculateRandom()
    {
        float rFrom = 0;
        float rTo = 0;
        float.TryParse(randomFrom, out rFrom);  // Tryparse will look at user input and convert to a number (e.g. this is 10 >> 10)
        float.TryParse(randomTo, out rTo);

        int randFrom = (int)(rFrom * 10);
        int randTo = (int)(rTo * 10);

        int selected = UnityEngine.Random.Range(randFrom, randTo + 1);

        float selectedValue = selected / 10;
        inputValue = selectedValue.ToString();
    }

    public override string GetResult()
    {
        return inputValue.ToString();
    }
}
