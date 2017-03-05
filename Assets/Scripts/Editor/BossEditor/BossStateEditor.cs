using UnityEngine;
using UnityEditor;

public class BossStateEditor : BaseEditor {
    
    public BossState BossStateObject;

    [MenuItem("Window/Boss State Editor")]
    private static void ShowEditor()
    {
        BossStateEditor editor = GetWindow<BossStateEditor>();
        editor.SetEditorTheme();
        editor.BossStateObject = null;
    }
    
    protected override void OnLostFocus()
    {
        if (BossStateObject == null) return;

        base.OnLostFocus();
        BossStateObject.Actions = EditorHelpers.GetActionsFromNodes(Nodes);
        EditorUtility.SetDirty(BossStateObject);
    }

    public override void LoadEditor(ScriptableObject obj)
    {
        BossStateObject = (BossState)obj;    // TODO create Base class for BossCreator and BossState (e.g. BaseBossEditable)
        base.LoadEditor(obj);
    }

    protected override void SetEditorTheme()
    {
        if (BossStateObject != null)
        {
            EditorLabel = string.Format("{0} - {1}", BossStateObject.BossName, BossStateObject.StateName);
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1Available");
        titleContent.text = "Boss State";
        colourTheme = ColourThemes.Blue;
    }
}
