using System;
using UnityEngine;

public abstract class BaseContextMenus {

    protected BaseEditor editor;
    protected BaseNode selectedNode;

    protected enum NodeMenuSelections
    {
        EditState,
        FindStart,
        DeleteNode
    }

    public BaseContextMenus(BaseEditor editorInstance)
    {
        editor = editorInstance;
    }

    public abstract void ShowMenu();
    public abstract void ShowNodeMenu(BaseNode mouseDownNode);

    protected void ContextCallback(object obj)
    {
        if (obj is Type)
            CreateNodeIfNodeType((Type)obj);
        else if (obj is NodeMenuSelections)
            ActionNodeMenuSelection((NodeMenuSelections)obj);
    }
    
    protected void CreateNodeIfNodeType(Type type)
    {
        if (!type.IsSubclassOf(typeof(BaseNode))) return;
        
        if (type.Equals(typeof(StartNode)) && editor.StartExists())
        {
            Debug.LogError("Start Node already exists. Cannot create another!");
            return;
        }

        BaseNode newNode = (BaseNode)Activator.CreateInstance(type);
        if (newNode != null)
            editor.AddNode(newNode);
    }
    
    private void ActionNodeMenuSelection(NodeMenuSelections selection)
    {
        switch (selection)
        {
            case NodeMenuSelections.EditState:
                ((BossEditor)editor).EditState();
                break;
            case NodeMenuSelections.FindStart:
                editor.MoveToStart();
                break;
            case NodeMenuSelections.DeleteNode:
                editor.RemoveNode(selectedNode);
                break;
        }
    }
}
