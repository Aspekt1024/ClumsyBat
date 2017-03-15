using UnityEngine;
using UnityEditor;

public class BossStateEditorContextMenus : BaseContextMenus {

    public BossStateEditorContextMenus(BaseEditor editorInstance) : base(editorInstance)
    {
        editor = editorInstance;
    }

    public override void ShowMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("JumpPound/Jump"), false, ContextCallback, typeof(JumpNode));
        menu.AddItem(new GUIContent("Projectile Abilities/Parabolic"), false, ContextCallback, typeof(ParabolicProjectileNode));
        menu.AddItem(new GUIContent("Stalactite Abilities/Spawn & Drop"), false, ContextCallback, typeof(SpawnStalNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Death Node"), false, ContextCallback, typeof(BaseNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Wait Node"), false, ContextCallback, typeof(WaitNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Start Node"), false, ContextCallback, typeof(StartNode));
        menu.AddItem(new GUIContent("Add Loop Node"), false, ContextCallback, typeof(LoopNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Find Start"), false, ContextCallback, NodeMenuSelections.FindStart);
        menu.ShowAsContext();
    }

    public override void ShowNodeMenu(BaseNode mouseDownNode)
    {
        selectedNode = mouseDownNode;

        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Find Start"), false, ContextCallback, NodeMenuSelections.FindStart);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, NodeMenuSelections.DeleteNode);
        menu.ShowAsContext();
    }
}
