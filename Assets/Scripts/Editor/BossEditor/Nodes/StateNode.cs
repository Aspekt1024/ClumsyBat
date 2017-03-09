using UnityEngine;
using UnityEditor;
using System;

public class StateNode : BaseNode {

    public BossState State;

    public override void SetupNode(BossDataContainer dataContainer)
    {
        State = CreateNewState(dataContainer);
        base.SetupNode(dataContainer);
    }
    
    private BossState CreateNewState(BossDataContainer baseContainer)
    {
        BossState newState = CreateInstance<BossState>();
        newState.StateName = "State";
        newState.BossName = baseContainer.BossName;
        newState.RootContainer = baseContainer;
        EditorHelpers.SaveNodeEditorAsset(newState, baseContainer, "", "State");
        return newState;
    }
    
protected override void AddInterfaces()
    {
        AddInput();
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(25f);
        SetOutput(25f);
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

    public override void DeleteNode()
    {
        // TODO State.DeleteAllNodes();
        base.DeleteNode();

        // TODO find all nodes belonging to this state and remove them from the assetdatabase.
        // Good luck!
    }

    public override BaseAction ConvertNodeToAction()
    {
        throw new NotImplementedException();
    }
}
