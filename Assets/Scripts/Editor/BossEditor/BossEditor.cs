using UnityEngine;
using UnityEditor;

public class BossEditor : BaseEditor {
    
    [MenuItem("Window/Boss Editor")]
    private static void ShowEditor()
    {
        BossEditor editor = GetWindow<BossEditor>();
        editor.SetEditorTheme();
        editor.ParentObject = null;
    }

    public override void LoadEditor(BossDataContainer creatorObj)
    {
        ParentObject = creatorObj;
        base.LoadEditor(creatorObj);
    }

    protected override void SetEditorTheme()
    {
        if (ParentObject != null)
        {
            EditorLabel = "State machine : " + ParentObject.BossName;
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1AvailableClicked");
        titleContent.text = "Boss Editor";
        colourTheme = ColourThemes.Green;
    }
    
    public void EditState()
    {
        BossState state = ((StateNode)_currentNode).State;
        BossStateEditor editor = GetWindow<BossStateEditor>(desiredDockNextTo: typeof(SceneView));
        editor.LoadEditor(state);
    }
}
