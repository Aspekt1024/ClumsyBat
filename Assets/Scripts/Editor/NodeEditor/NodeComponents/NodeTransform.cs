using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTransform {

    public Rect WindowRect;
    public bool IsSelected;

    public float Width
    {
        get { return WindowRect.width; }
        set { WindowRect.width = value; }
    }

    public float Height
    {
        get { return WindowRect.height; }
        set { WindowRect.height = value; }
    }

    private BaseNode node;
    private bool IsDragged;

    private Vector2 dragOffset;
    private Rect offsetRect;

    public NodeTransform(BaseNode parent)
    {
        node = parent;
    }

    public void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ActionLeftMouseDown(e);
                }
                break;
            case EventType.mouseUp:
                if (IsDragged)
                {
                    WindowRect.position += dragOffset;
                    dragOffset = Vector2.zero;
                    IsDragged = false;
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && IsDragged)
                {
                    dragOffset += e.delta;
                    e.Use();
                    return;
                }
                break;
        }
    }

    private void ActionLeftMouseDown(Event e)
    {
        if (offsetRect.Contains(e.mousePosition))
        {
            IsDragged = true;

            if (e.shift)
                IsSelected = !IsSelected;
            else
            {
                if (!IsSelected)
                    NodeGUI.DeselectAllNodes();

                IsSelected = true;
            }

            GUI.changed = true;
        }
    }

    public Rect GetWindow(Vector2 canvasOffset)
    {
        WindowRect.position = SnapToGrid(WindowRect.position);
        offsetRect = new Rect(SnapToGrid(WindowRect.position + dragOffset) + canvasOffset, WindowRect.size);
        return offsetRect;
    }
    
    private Vector2 SnapToGrid(Vector2 pos)
    {
        Vector2 snappedPos = new Vector2
        {
            x = NodeGUI.GridSpacing * Mathf.Round(pos.x / NodeGUI.GridSpacing),
            y = NodeGUI.GridSpacing * Mathf.Round(pos.y / NodeGUI.GridSpacing)
        };
        return snappedPos;
    }
}
