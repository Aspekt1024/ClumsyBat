﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTransform {
    
    public bool IsSelected;
    public bool IsDragged;

    public float Width
    {
        get { return node.WindowRect.width; }
        set { node.WindowRect.width = value; }
    }

    public float Height
    {
        get { return node.WindowRect.height; }
        set { node.WindowRect.height = value; }
    }

    private BaseNode node;

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
            case EventType.MouseUp:
                if (IsDragged && e.button == 0)
                {
                    node.WindowRect.position += dragOffset;
                    dragOffset = Vector2.zero;
                    IsDragged = false;
                    e.Use();
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
                {
                    node.ParentEditor.DeselectAllNodes();
                }

                IsSelected = true;
            }

            GUI.changed = true;
        }
    }

    public Rect GetWindow(Vector2 canvasOffset)
    {
        node.WindowRect.position = SnapToGrid(node.WindowRect.position);

        Vector2 draggedPosition = SnapToGrid(node.WindowRect.position + dragOffset);
        if (draggedPosition != node.WindowRect.position)
        {
            Vector2 delta = draggedPosition - node.WindowRect.position;
            node.ParentEditor.MoveAllSelectedNodes(delta);
            dragOffset -= delta;
        }

        offsetRect = new Rect(draggedPosition + canvasOffset, node.WindowRect.size);
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
