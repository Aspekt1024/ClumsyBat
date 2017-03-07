using UnityEngine;
using UnityEditor;

public class BossStateEditor : BaseEditor {
    
    [MenuItem("Window/Boss State Editor")]
    private static void ShowEditor()
    {
        BossStateEditor editor = GetWindow<BossStateEditor>();
        editor.SetEditorTheme();
        editor.BaseContainer = null;
    }

    public override void LoadEditor(BossDataContainer obj)
    {
        BaseContainer = obj;
        base.LoadEditor(obj);
    }

    protected override void SetEditorTheme()
    {
        if (BaseContainer != null)
        {
            EditorLabel = string.Format("{0} - {1}", BaseContainer.BossName, ((BossState)BaseContainer).StateName);
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1Available");
        titleContent.text = "Boss State";
        colourTheme = ColourThemes.Blue;
    }
}
