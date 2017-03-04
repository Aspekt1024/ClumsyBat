using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BossStateEditor : BaseEditor {
    
    public BossState BossStateObject;

    [MenuItem("Window/Boss State Editor")]
    private static void ShowEditor()
    {
        BossStateEditor editor = GetWindow<BossStateEditor>();
        editor.SetTitle();
        editor.BossStateObject = null;
    }
    
    protected override void OnLostFocus()
    {
        if (BossStateObject == null) return;

        base.OnLostFocus();
        BossStateObject.Nodes = Nodes;
        EditorUtility.SetDirty(ParentObject);
    }
    
    public override void LoadEditor(ScriptableObject obj)
    {
        SetTitle();
        ParentObject = obj;
        BossStateObject = (BossState)obj;
        
        EditorLabel = string.Format("{0} - {1}", BossStateObject.BossName, BossStateObject.StateName);

        if (BossStateObject.Nodes == null || BossStateObject.Nodes.Count == 0)
            Nodes = new List<BaseNode>();
        else
            Nodes = BossStateObject.Nodes;
    }

    private void SetTitle()
    {
        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1Available");
        titleContent.text = "Boss State";
    }
}
