using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BossEditor : BaseEditor {

    public BossCreator BossCreatorObject;

    [MenuItem("Window/Boss Editor")]
    private static void ShowEditor()
    {
        BossEditor editor = GetWindow<BossEditor>();
        editor.SetEditorTheme();
        editor.BossCreatorObject = null;
    }
    
    protected override void OnLostFocus()
    {
        if (ParentObject == null) return;

        base.OnLostFocus();
        ((BossCreator)ParentObject).Nodes = Nodes;
        EditorUtility.SetDirty(ParentObject);
    }

    public override void LoadEditor(ScriptableObject creatorObj)
    {
        ParentObject = creatorObj;
        BossCreatorObject = (BossCreator)creatorObj;    // TODO create Base class for BossCreator and BossState (e.g. BaseBossEditable)

        SetEditorTheme();

        if (BossCreatorObject.Nodes == null || BossCreatorObject.Nodes.Count == 0)
            Nodes = new List<BaseNode>();
        else
        {
            Nodes = BossCreatorObject.Nodes;
        }
    }

    private void SetEditorTheme()
    {
        EditorLabel = "State machine : " + BossCreatorObject.BossName;
        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1AvailableClicked");
        titleContent.text = "Boss Editor";
        colourTheme = ColourThemes.Green;
    }

    public override void AddNode(BaseNode newNode)
    {
        base.AddNode(newNode);

        if (newNode.GetType().Equals(typeof(StateNode)))
        {
            string dataFolder = EditorHelpers.GetAssetDataFolder(ParentObject);
            ((StateNode)newNode).CreateNewState(dataFolder, BossCreatorObject.BossName);
            BossCreatorObject.States.Add(((StateNode)newNode).State);
        }
    }
    
    public void EditState()
    {
        BossState state = ((StateNode)_currentNode).State;
        BossStateEditor editor = GetWindow<BossStateEditor>(desiredDockNextTo: typeof(SceneView));
        editor.LoadEditor(state);
    }
}
