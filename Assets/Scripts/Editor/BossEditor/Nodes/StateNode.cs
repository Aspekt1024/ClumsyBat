using UnityEngine;
using UnityEditor;

public class StateNode : BaseNode {

    public BossState State;

    public override void SetupNode(BossDataContainer dataContainer)
    {
        DataContainer = dataContainer;
        SaveThisNodeAsset();

        Action = CreateInstance<MachineState>();
        SaveActionAsset();

        AddInput();
        AddOutput();

        UpdateActionInterfaces();

        CreateNewState();
    }

    private void SetInterfacePositions()
    {
        CreateInput(25f);
        CreateOutput(25f);
    }

    public override void DrawWindow()
    {
        WindowRect.width = 200;
        WindowRect.height = 100;
        WindowTitle = State.StateName;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 80;
        State.StateName = EditorGUILayout.TextField("State name:", State.StateName);

        SetInterfacePositions();
        DrawInterfaces();
    }

    private void CreateNewState()
    {
        State = CreateInstance<BossState>();

        State.BossName = DataContainer.BossName;
        ((MachineState)Action).State = State;
        EditorUtility.SetDirty(DataContainer);

        EditorHelpers.SaveActionAsset(State, DataContainer, "States", "State");
        EditorUtility.SetDirty(State);
    }

    public override void DeleteNode()
    {
        base.DeleteNode();

        // TODO find all nodes and actions belonging to this state and remove them from the assetdatabase.
        // Good luck!

        State = null;
    }

    public override void UpdateActionInterfaces()
    {
        ((MachineState)Action).State = State;
        base.UpdateActionInterfaces();
    }

}
