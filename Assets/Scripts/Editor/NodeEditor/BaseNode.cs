using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;

using IODirection = ActionConnection.IODirection;

public abstract class BaseNode {

    public string WindowTitle = "Untitled";
    public int ID;
    public List<NodeInterface> interfaces = new List<NodeInterface>();
    public Rect WindowRect;
    
    [XmlIgnore] public NodeTransform Transform;
    [XmlIgnore] public BaseEditor ParentEditor;
    [XmlIgnore] public float SelectedBorderAlpha;
    
    private Rect NodeRect;
    private Vector2 selectedOutputPos;
    private NodeRuntimeBorders runtimeBorder;

    public abstract BaseAction GetAction();
    protected abstract void AddInterfaces();

    public virtual void SetupNode(BehaviourSet behaviour)
    {
        AddInterfaces();
    }

    public void ProcessEvents(Event e)
    {
        foreach (NodeInterface iface in interfaces)
        {
            iface.ProcessEvents(e);
        }
        Transform.ProcessEvents(e);
    }
    
    public void DrawNodeWindow(Vector2 canvasOffset)
    {
        if (runtimeBorder == null) runtimeBorder = new NodeRuntimeBorders(this);
        runtimeBorder.Update();

        NodeRect = Transform.GetWindow(canvasOffset);
        
        if (Transform.IsSelected)
            GUI.skin = (GUISkin)EditorGUIUtility.Load("NodeWindowSkin.guiskin");
        else
            GUI.skin = (GUISkin)EditorGUIUtility.Load("NodeNormalSkin.guiskin");
        
        GUI.Box(NodeRect, WindowTitle);
        
        if (SelectedBorderAlpha > 0 && Application.isPlaying)
            DrawOutline(new Color(0.9f, 0.6f, 0.6f), SelectedBorderAlpha);
        else if (Transform.IsSelected)
            DrawOutline(new Color(0.4f, 0.6f, 0.8f));

        GUI.BeginGroup(NodeRect);
        NodeGUI.SetWindow(NodeRect.size);    // TODO completely replace GUI.BeginGroup with NodeGUI... eventually.
        Draw();
        GUI.EndGroup();
    }

    public virtual void Draw()  // protected?
    {
        DrawInterfaces();
    }
    
    public virtual void InitialiseNode(Vector2 position, BaseEditor editor)
    {
        ParentEditor = editor;
        Transform = new NodeTransform(this);
        WindowRect = new Rect(position.x, position.y, WindowRect.width, WindowRect.height);
    }

    public bool Contains(Vector2 pos)
    {
        return new Rect(NodeRect.position, WindowRect.size).Contains(pos);
    }

    protected void AddInput(int id, NodeInterface.InterfaceTypes ifaceType = NodeInterface.InterfaceTypes.Event)
    {
        AddInterface(IODirection.Input, id, ifaceType);
    }

    protected void AddOutput(int id, NodeInterface.InterfaceTypes ifaceType = NodeInterface.InterfaceTypes.Event)
    {
        AddInterface(IODirection.Output, id, ifaceType);
    }

    protected void AddInterface(IODirection ioType, int id, NodeInterface.InterfaceTypes ifaceType = NodeInterface.InterfaceTypes.Event)
    {
        NodeInterface iface = new NodeInterface(this);
        iface.ID = id;
        iface.Type = ifaceType;
        iface.Direction = ioType;
        interfaces.Add(iface);
    }

    private void DrawOutline(Color c, float alpha = 1f)
    {
        const int innerThickness = 4;
        const int outerThickness = 10;

        Handles.color = Color.white;
        Vector2 pos = GetOffsetPosition();

        for (int i = 0; i < outerThickness; i++)
        {
            Color rectColor = new Color(c.r, c.g, c.b, Mathf.Pow((1 - i / (float)outerThickness), 3f) * 0.7f * alpha);
            Rect rect = new Rect(pos - Vector2.one * (i + 2), WindowRect.size + Vector2.one * 2 * (i + 2));
            Handles.DrawSolidRectangleWithOutline(rect, Color.clear, rectColor);
        }

        for (int i = 0; i < innerThickness; i++)
        {
            Color rectColor = new Color(c.r, c.g, c.b, Mathf.Pow(1 - i / (float)innerThickness, 3) * 0.7f * alpha);
            Rect rect = new Rect(pos + Vector2.one * (i - 2), WindowRect.size - Vector2.one * 2 * (i - 2));
            Handles.DrawSolidRectangleWithOutline(rect, Color.clear, rectColor);
        }
    }

    public Vector2 GetOffsetPosition()
    {
        return ParentEditor.CanvasOffset + ParentEditor.CanvasDrag + WindowRect.position;
    }

    protected void DrawInterfaces()
    {
        foreach (NodeInterface iface in interfaces)
        {
            iface.Draw();
        }
    }

    public void DrawConnections()
    {
        // Must be drawn outside of GUI.BeginGroup()
        foreach (NodeInterface iface in interfaces)
        {
            iface.DrawConnections();
        }
    }

    protected void SetInterface(int id, int yPos, string label = "")
    {
        const float yPosSpacing = 20f;
        const float yStartPos = 35f;
        foreach (var iface in interfaces)
        {
            if (iface.ID != id) continue;
            iface.SetInterface((yPos - 1) * yPosSpacing + yStartPos, label);
        }
    }

    protected void HideInterface(int id)
    {
        GetInterface(id).IsHidden = true;
    }

    protected void ShowInterface(int id)
    {
        GetInterface(id).IsHidden = false;
    }

    public virtual void DeleteNode()
    {
        foreach (var iface in interfaces)
        {
            iface.Disconnect();
        }
    }

    public NodeInterface GetInterface(int id)
    {
        foreach (var iface in interfaces)
        {
            if (iface.ID == id)
                return iface;
        }
        return null;
    }

    public bool IsType<T>() where T : BaseNode
    {
        return GetType().Equals(typeof(T));
    }
}
