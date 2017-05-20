using UnityEngine;
using UnityEditor;

public class BossStateEditorContextMenus : BaseContextMenus {

    public BossStateEditorContextMenus(BaseEditor editorInstance) : base(editorInstance)
    {
        editor = editorInstance;
    }

    public override void ShowMenu()
    {
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
