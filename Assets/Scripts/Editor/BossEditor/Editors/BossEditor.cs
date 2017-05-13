using UnityEngine;
using UnityEditor;
using System;

public class BossEditor : BaseEditor {
    
    [MenuItem("Window/Boss Editor")]
    private static void ShowEditor()
    {
        BossEditor editor = GetWindow<BossEditor>();
        editor.SetEditorTheme();
        editor.BaseContainer = null;
    }

    public override void LoadEditor(BossDataContainer creatorObj)
    {
        BaseContainer = creatorObj;
        base.LoadEditor(creatorObj);
    }

    protected override void SetEditorTheme()
    {
        if (BaseContainer != null)
        {
            EditorLabel = "State machine : " + BaseContainer.BossName;
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1AvailableClicked");
        titleContent.text = "Boss Editor";
        colourTheme = ColourThemes.Green;
    }
    
    public void EditState()
    {
        // TODO this only works if the editor is already active...
        BossState state = ((StateNode)_currentNode).State;
        BossStateEditor editor = GetWindow<BossStateEditor>(desiredDockNextTo: typeof(SceneView));
        editor.LoadEditor(state);
    }

    protected override void LoadNodeData()
    {
        const string nodeDataName = "NodeData";
        string nodeDataFolder = EditorHelpers.GetDataPath(BaseContainer.RootContainer);
        string nodeDataPath = string.Format("{0}/{1}.asset", nodeDataFolder, nodeDataName);

        NodeData = AssetDatabase.LoadAssetAtPath<BossEditorNodeData>(nodeDataPath);

        if (NodeData == null)
            CreateNewNodeData(nodeDataPath);
    }
}
