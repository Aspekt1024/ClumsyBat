using UnityEngine;
using UnityEditor;

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
        if (BossCreatorObject == null) return;

        BossCreatorObject.Actions = EditorHelpers.GetActionsFromNodes(Nodes);
        base.OnLostFocus();
        EditorUtility.SetDirty(BossCreatorObject);
    }

    public override void LoadEditor(ScriptableObject creatorObj)
    {
        BossCreatorObject = (BossCreator)creatorObj;    // TODO create Base class for BossCreator and BossState (e.g. BaseBossEditable)
        base.LoadEditor(creatorObj);
    }

    protected override void SetEditorTheme()
    {
        if (BossCreatorObject != null)
        {
            EditorLabel = "State machine : " + BossCreatorObject.BossName;
        }

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
