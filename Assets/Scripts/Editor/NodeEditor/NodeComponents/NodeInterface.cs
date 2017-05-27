using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;

using IODirection = ActionConnection.IODirection;

[Serializable]
public class NodeInterface {
    
    public Vector2 Position;
    public int ID;
    public string Label;
    public InterfaceTypes Type;
    public IODirection Direction;
    public int ConnectedNodeID;
    public int ConnectedIfaceID;

    [XmlIgnore] public bool IsHidden;
    [XmlIgnore] public BaseNode Node;
    [XmlIgnore] public NodeInterface ConnectedInterface;
    [XmlIgnore] public bool IsDragged;

    public enum InterfaceTypes { Event, Object }

    private Vector2 mousePos;

    public NodeInterface()
    {
        Node = null;
    }

    public NodeInterface(BaseNode parent)
    {
        Node = parent;
    }

    public void ProcessEvents(Event e)
    {
        mousePos = e.mousePosition;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (Vector2.Distance(mousePos, Node.GetOffsetPosition() + Position) < 10)
                {
                    StartDraggingInterface();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                IsDragged = false;
                if (Vector2.Distance(mousePos, Node.GetOffsetPosition() + Position) < 10)
                {
                    ConnectInterfacesIfValid();
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (IsDragged)
                    e.Use();
                break;
        }
    }

    private void StartDraggingInterface()
    {
        if (Direction == IODirection.Input)
        {
            if (ConnectedInterface == null) return;
            Node.ParentEditor.DraggedInterface = ConnectedInterface;
            Disconnect();
        }
        else
        {
            Node.ParentEditor.DraggedInterface = this;
        }

        Node.ParentEditor.DraggedInterface.IsDragged = true;
    }

    private void ConnectInterfacesIfValid()
    {
        NodeInterface iface2 = Node.ParentEditor.DraggedInterface;
        Node.ParentEditor.DraggedInterface = null;

        if (Direction == IODirection.Output)
            Disconnect();

        if (iface2 == null) return;

        iface2.IsDragged = false;

        if (Direction == iface2.Direction || Node == iface2.Node) return;

        Disconnect();
        iface2.Disconnect();
        ConnectedInterface = iface2;
        iface2.ConnectedInterface = this;
        
    }

    public void SetInterface(float yPos, string label)
    {
        Label = label;
        Position.y = yPos;

        if (Direction == IODirection.Input)
            Position.x = 7f;
        else
            Position.x = Node.WindowRect.width - 7f;
    }

    public void Draw()
    {
        if (IsHidden) return;

        Vector3 position = new Vector3(Node.WindowRect.width - 7f, Position.y, 0f);
        if (Direction == IODirection.Input)
            position.x = 7f;

        DrawInterfaceAt(position);
        DrawLabel();
    }
    
    private void DrawInterfaceAt(Vector3 position)
    {
        Color ringCol = Color.blue;
        Color connCol = new Color(0.5f, 0.5f, 1f);
        if (Type == InterfaceTypes.Object)
        {
            ringCol = Color.red;
            connCol = new Color(1f, 0.5f, 0.5f);
        }

        DrawCircle(position, 6f, ringCol);
        DrawCircle(position, 4f, Color.white);

        if (ConnectedInterface !=  null || IsDragged)
        {
            DrawCircle(position, 3f, connCol);
        }
    }

    private void DrawLabel()
    {
        if (Label == string.Empty) return;

        EditorGUIUtility.labelWidth = 70f;
        var gs = GUI.skin.GetStyle("Label");
        gs.alignment = Direction == IODirection.Input ? TextAnchor.UpperLeft : TextAnchor.UpperRight;

        Vector2 position = new Vector2(Node.WindowRect.width - 85f, Position.y - 9);
        if (Direction == IODirection.Input)
            position.x = 15f;

        EditorGUI.LabelField(new Rect(position, new Vector2(70, 18)), Label, gs);
    }

    private void DrawCircle(Vector3 position, float radius, Color color)
    {
        Handles.color = color;
        Handles.DrawSolidDisc(position, Vector3.back, radius);
    }

    public void Disconnect()
    {
        if (ConnectedInterface == null) return;
        if (Direction == IODirection.Input)
            ConnectedInterface.ConnectedInterface = null;   // TODO if multiple connections are allowed, check ID
        else
            ConnectedInterface.ConnectedInterface = null;

        ConnectedInterface = null;
    }
    
    public void DrawConnections()
    {
        if (Direction == IODirection.Output && ConnectedInterface != null)
        {
            Vector2 start = Node.GetOffsetPosition() + Position;
            Vector2 end = ConnectedInterface.Node.GetOffsetPosition() + ConnectedInterface.Position;

            DrawConnection(start, end, Type);
        }

        if (IsDragged)
            DrawDraggedConnection();
    }

    private void DrawDraggedConnection()
    {
        NodeInterface iface = Node.ParentEditor.DraggedInterface;
        if (iface == null) return;

        Vector2 startPos = iface.Position + iface.Node.GetOffsetPosition();
        DrawConnection(startPos, mousePos, Type);
    }

    private void DrawConnection(Vector2 startPos, Vector2 endPos, InterfaceTypes outputType = InterfaceTypes.Event)
    {
        Color shadowCol = new Color(0.7f, 0.7f, 1f);
        if (outputType == InterfaceTypes.Object)
            shadowCol = new Color(1f, 0.7f, 0.7f);

        Vector3 tanScale = GetTanScale(startPos, endPos);
        Vector3 startTan = new Vector3(startPos.x, startPos.y, -1) + tanScale;
        Vector3 endTan = new Vector3(endPos.x, endPos.y, -1) - tanScale;

        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol * new Color(1f, 1f, 1f, 0.06f), null, (i + 1) * 7);
        }
        Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, 3);
        Node.ParentEditor.Repaint();
    }
    
    private Vector3 GetTanScale(Vector3 startPos, Vector3 endPos)
    {
        Vector3 tanScale = new Vector3(50f, 0f, 0f);

        float dX = Mathf.Clamp(endPos.x - startPos.x, -300f, 300f);
        float dY = Mathf.Clamp(endPos.y - startPos.y, -300f, 300f);
        if (dX < 0)
        {
            tanScale += new Vector3(-dX / 2, dY/8 * Mathf.Clamp(-dX / 50f, 0f, 1f), 0f);
        }
        return tanScale;
    }
    
    public BaseNode GetNode()
    {
        return Node;
    }

    public bool IsConnected()
    {
        return ConnectedInterface != null;
    }
}
