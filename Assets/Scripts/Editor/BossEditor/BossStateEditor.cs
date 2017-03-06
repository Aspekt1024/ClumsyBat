using UnityEngine;
using UnityEditor;

public class BossStateEditor : BaseEditor {
    
    [MenuItem("Window/Boss State Editor")]
    private static void ShowEditor()
    {
        BossStateEditor editor = GetWindow<BossStateEditor>();
        editor.SetEditorTheme();
        editor.ParentObject = null;
    }

    public override void LoadEditor(BossDataContainer obj)
    {
        ParentObject = (BossState)obj;    // TODO create Base class for BossCreator and BossState (e.g. BaseBossEditable)
        base.LoadEditor(obj);
    }

    protected override void SetEditorTheme()
    {
        if (ParentObject != null)
        {
            EditorLabel = string.Format("{0} - {1}", ParentObject.BossName, ((BossState)ParentObject).StateName);
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1Available");
        titleContent.text = "Boss State";
        colourTheme = ColourThemes.Blue;
    }
}
