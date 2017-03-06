using UnityEngine;
using UnityEditor;

public class WaitNode : BaseNode {

    public float WaitTime = 1f;

    public override void SetupNode(BossDataContainer dataContainer)
    {
        DataContainer = dataContainer;
        SaveThisNodeAsset();
        
        Action = CreateInstance<WaitAction>();
        SaveActionAsset();

        AddInput();
        AddOutput();
        
        UpdateActionInterfaces();
    }

    private void SetInterfacePositions()
    {
        CreateInput(25);
        CreateOutput(25);
    }

    public override void DrawWindow()
    {
        WindowTitle = "Wait";
        WindowRect.width = 140;
        WindowRect.height = 40;

        EditorGUIUtility.labelWidth = 70f;
        WaitTime = EditorGUI.FloatField(new Rect(new Vector2(15, 18), new Vector2(100, 18)), "Seconds:", WaitTime);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override void UpdateActionInterfaces()
    {
        ((WaitAction)Action).WaitTime = WaitTime;
        base.UpdateActionInterfaces();
    }
}
