using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeEditorMenu {

    private BaseEditor editor;
    private List<NodeSystem> systems;
    private NodeSystem mainSystem;
    private int activeSystemID;

    public NodeEditorMenu(BaseEditor editorRef)
    {
        editor = editorRef;
        systems = new List<NodeSystem>();
        activeSystemID = -1;
    }

    public void SaveCurrentMenuState()
    {
        if (activeSystemID < 0) return;

        if (!Application.isPlaying)
            NodeEditorSaveHandler.Save(editor);

        if (activeSystemID == 0)
        {
            mainSystem.CanvasOffset = editor.CanvasOffset;
        }
        else
        {
            for (int i = 0; i < systems.Count; i++)
            {
                if (systems[i].ID == activeSystemID)
                    systems[i].CanvasOffset = editor.CanvasOffset;
            }
        }
    }

    public void UpdateSystemModel()
    {
        if (editor.StateMachine.IsType<BossStateMachine>())
        {
            SetMainSystem();
            activeSystemID = 0;
        }
        else
        {
            Debug.Log("fix this being called when state is opened instead of state machine");
            int id = GetNewID();
            StoreSystem(id);
            GetMainSystem();
            ActivateSystemWithID(id);
        }
    }

    public void Draw()
    {
        GUI.skin = (GUISkin)EditorGUIUtility.Load("HeaderSkin.guiskin");
        DrawMenuBox();
        DrawMainSystemButton();
        DrawSubSystemButtons();
        DrawHeading();
        DrawCloseButton();
    }

    private void DrawMenuBox()
    {
        Handles.color = Color.white;

        if (editor.StateMachine.IsType<BossStateMachine>())
            Handles.color = Color.red * 0.7f;

        Handles.DrawSolidRectangleWithOutline(new Rect(0, 0, editor.position.width, 60f), Color.black, Color.grey);
    }

    private void DrawMainSystemButton()
    {
        Rect rect = new Rect(10f, 30f, 100f, 25f);
        string sysName = "Root";

        if (activeSystemID == 0)
        {
            GUI.Box(rect, sysName);
        }
        else if (GUI.Button(rect, sysName))
        {
            SaveCurrentMenuState();
            ActivateMainSystem();
        }
    }

    private void DrawSubSystemButtons()
    {
        for (int i = 0; i < systems.Count; i++)
        {
            Rect rect = new Rect(115f + i * 110f, 30f, 100f, 25f);
            string sysName = systems[i].Machine.name;
            if (activeSystemID == systems[i].ID)
            {
                GUI.Box(rect, sysName);
            }
            else if (GUI.Button(rect, sysName))
            {
                if (activeSystemID == systems[i].ID) break;
                SaveCurrentMenuState();
                ActivateSystem(systems[i]);
            }
        }
    }

    private void DrawHeading()
    {
        var cStyle = new GUIStyle();
        cStyle.normal.textColor = Color.white;
        cStyle.fontSize = 20;
        cStyle.fontStyle = FontStyle.Bold;
        EditorGUI.LabelField(new Rect(3f, 3f, editor.position.width, 30f), editor.EditorLabel, cStyle);
    }

    private void DrawCloseButton()
    {
        if (activeSystemID <= 0) return;

        Rect rect = new Rect(editor.position.width - 55f, 65f, 50f, 17f);
        if (GUI.Button(rect, "Close"))
        {
            NodeEditorSaveHandler.Save(editor);

            int systemToRemove = activeSystemID;
            ActivateMainSystem();

            for (int i = 0; i < systems.Count; i++)
            {
                if (systems[i].ID == systemToRemove)
                {
                    systems.Remove(systems[i]);
                    break;
                }
            }
        }

    }

    private int GetNewID()
    {
        int id = 1;
        bool idNotUnique = true;

        while (idNotUnique)
        {
            idNotUnique = false;
            foreach (var system in systems)
            {
                if (id == system.ID)
                {
                    id++;
                    idNotUnique = true;
                    break;
                }
            }
        }
        return id;
    }

    private void StoreSystem(int id)
    {
        foreach(var system in systems)
        {
            if (system.Machine == editor.StateMachine)
            {
                return;
            }
        }

        NodeSystem newSystem = new NodeSystem(id, editor.Nodes, editor.CanvasOffset, editor.StateMachine);
        systems.Add(newSystem);
    }

    private void ActivateMainSystem()
    {
        ActivateSystem(mainSystem);
    }

    private void ActivateSystemWithID(int id)
    {
        foreach (var system in systems)
        {
            if (system.ID != id) continue;
            ActivateSystem(system);
            return;
        }
        Debug.Log("Unable to find system with id " + id);
    }

    private void ActivateSystem(NodeSystem system)
    {
        editor.Nodes = system.Nodes;
        editor.StateMachine = system.Machine;
        editor.CanvasOffset = system.CanvasOffset;
        activeSystemID = system.ID;
    }

    private void GetMainSystem()
    {
        editor.LoadEditor(editor.StateMachine.RootStateMachine);
        SetMainSystem();
    }

    private void SetMainSystem()
    {
        mainSystem = new NodeSystem(0, editor.Nodes, editor.CanvasOffset, editor.StateMachine);
    }
}

public class NodeSystem
{
    public int ID;
    public List<BaseNode> Nodes;
    public Vector2 CanvasOffset;
    public StateMachine Machine;

    public NodeSystem(int id, List<BaseNode> nodes, Vector2 offset, StateMachine stateMachine)
    {
        ID = id;
        Nodes = nodes;
        CanvasOffset = offset;
        Machine = stateMachine;
    }
}
