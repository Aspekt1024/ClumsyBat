using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using IODirection = NodeInterface.IODirection;

public abstract class BaseNode {
    
    public string WindowTitle = "Untitled";
    public Rect WindowRect;
    public NodeTransform Transform;
    public BaseEditor ParentEditor;
    public List<NodeInterface> interfaces = new List<NodeInterface>();
    
    [HideInInspector]
    public BossDataContainer DataContainer; // TODO remove/rename... generic, not boss
    
    public BaseAction Action;
    
    private Rect NodeRect;
    private Vector2 selectedOutputPos;
    
    protected abstract void AddInterfaces();
    
    public Vector2 GetOffsetPosition()
    {
        return ParentEditor.CanvasOffset + ParentEditor.CanvasDrag + WindowRect.position;
    }

    public virtual void SetupNode(BossDataContainer dataContainer)
    {
        DataContainer = dataContainer;
        AddInterfaces();
        SaveThisNodeAsset();
    }

    protected void SaveThisNodeAsset()
    {
        string subFolder = "";
        if (DataContainer.IsType<BossState>())
        {
            subFolder = DataContainer.name + "Data";
        }
        // TODO save
        //EditorHelpers.SaveNodeEditorAsset(this, DataContainer.RootContainer, subFolder, GetType().ToString());
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
        NodeRect = Transform.GetWindow(canvasOffset);
        
        if (Transform.IsSelected)
            GUI.skin = (GUISkin)EditorGUIUtility.Load("NodeWindowSkin.guiskin");
        else
            GUI.skin = (GUISkin)EditorGUIUtility.Load("NodeNormalSkin.guiskin");
        
        GUI.Box(NodeRect, WindowTitle);

        if (Transform.IsSelected)
            DrawSelectedOutline();

        GUI.BeginGroup(NodeRect);
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
        Transform.WindowRect = new Rect(position.x, position.y, Transform.WindowRect.width, Transform.WindowRect.height);
    }

    public bool Contains(Vector2 pos)
    {
        return new Rect(NodeRect.position, Transform.WindowRect.size).Contains(pos);
    }
    
    protected void AddInterface(IODirection ioType, int id, NodeInterface.InterfaceTypes ifaceType = NodeInterface.InterfaceTypes.Event)
    {
        NodeInterface iface = new NodeInterface(this);
        iface.ID = id;
        iface.Type = ifaceType;
        iface.Direction = ioType;
        interfaces.Add(iface);
    }

    private void DrawSelectedOutline()
    {
        Vector2 p1 = GetOffsetPosition();
        Vector2 p2 = p1 + Vector2.right * WindowRect.width;
        Vector2 p3 = p2 + Vector2.up * WindowRect.height;
        Vector2 p4 = p1 + Vector2.up * WindowRect.height;

        const int innerThickness = 4;
        const int outerThickness = 10;

        Handles.color = Color.white;
        for (int i = 0; i < outerThickness; i++)
        {
            Color rectColor = new Color(0.4f, 0.6f, 0.8f, Mathf.Pow((1 - i / (float)outerThickness), 3f) * 0.7f);
            Rect rect = new Rect(p1 - Vector2.one * (i + 2), WindowRect.size + Vector2.one * 2 * (i + 2));
            Handles.DrawSolidRectangleWithOutline(rect, Color.clear, rectColor);
        }

        for (int i = 0; i < innerThickness; i++)
        {
            Color rectColor = new Color(0.8f, 0.8f, 1f, Mathf.Pow(1 - i / (float)innerThickness, 3) * 0.7f);
            Rect rect = new Rect(p1 + Vector2.one * (i - 2), WindowRect.size - Vector2.one * 2 * (i - 2));
            Handles.DrawSolidRectangleWithOutline(rect, Color.clear, rectColor);
        }
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

    protected void SetInterface(float yPos, int id, string label = "")
    {
        foreach (var iface in interfaces)
        {
            if (iface.ID != id) continue;
            iface.SetInterface(yPos, label);
        }
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

    public BaseAction GetAction()
    {
        if (Action == null)
            CreateAction();

        return Action;
    }
    
    protected abstract void CreateAction();


    #region Assign node interfaces to Action

    public virtual void ConvertInterfaces()
    {
        foreach (var iface in interfaces)
        {
            if (iface.ConnectedInterface == null) continue;

            var actionInput = ConvertInterface(iface);
            //Action.Interfaces.Add(actionInput);   // TODO serialization
        }
    }

    private static BaseAction.InterfaceType ConvertInterface(NodeInterface iface)
    {
        // TODO serialization
        return new BaseAction.InterfaceType()
        {
            identifier = iface.ID,
            //connectedAction = iface.ConnectedNode.Action
        };
    }
    #endregion
}
