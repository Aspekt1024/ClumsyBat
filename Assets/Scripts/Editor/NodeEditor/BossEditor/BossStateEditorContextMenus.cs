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
        // TODO get all ability nodes and generate them dynamically
        // Use the window title of each node
        menu.AddItem(new GUIContent("Abilities/Jump"), false, ContextCallback, typeof(JumpNode));
        menu.AddItem(new GUIContent("Abilities/Projectile"), false, ContextCallback, typeof(ProjectileNode));
        menu.AddItem(new GUIContent("Abilities/Stalactite"), false, ContextCallback, typeof(SpawnStalNode));
        menu.AddItem(new GUIContent("Abilities/Movement"), false, ContextCallback, typeof(WalkNode));
        menu.AddItem(new GUIContent("Abilities/Charge"), false, ContextCallback, typeof(ChargeNode));
        menu.AddItem(new GUIContent("Events/Player"), false, ContextCallback, typeof(PlayerEventNode));
        menu.AddItem(new GUIContent("Events/Moth"), false, ContextCallback, typeof(SpawnMothNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Player Reference"), false, ContextCallback, typeof(PlayerNode));
        menu.AddItem(new GUIContent("Boss Reference"), false, ContextCallback, typeof(BossNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Wait"), false, ContextCallback, typeof(WaitNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Start"), false, ContextCallback, typeof(StartNode));
        menu.AddItem(new GUIContent("Add Loop"), false, ContextCallback, typeof(LoopNode));
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
