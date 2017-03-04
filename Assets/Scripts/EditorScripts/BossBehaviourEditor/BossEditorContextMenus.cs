using UnityEngine;
using UnityEditor;

public class BossEditorContextMenus : BaseContextMenus {
    
    public BossEditorContextMenus(BaseEditor editorInstance) : base(editorInstance)
    {
        editor = editorInstance;
    }

    public override void ShowMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add New State"), false, ContextCallback, typeof(StateNode));
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
        if (mouseDownNode.GetType().Equals(typeof(StateNode)))
        {
            menu.AddItem(new GUIContent("Edit State"), false, ContextCallback, NodeMenuSelections.EditState);
            menu.AddSeparator("");
        }
        menu.AddItem(new GUIContent("Find Start"), false, ContextCallback, NodeMenuSelections.FindStart);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, NodeMenuSelections.DeleteNode);
        menu.ShowAsContext();
    }
}
