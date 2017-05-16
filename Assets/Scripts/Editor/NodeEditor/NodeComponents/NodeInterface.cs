using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class NodeInterface {
    
    public Vector2 Position;
    public NodeInterface ConnectedInterface;
    public int ConnectedInterfaceIndex;
    public int ID;
    public string Label;
    public InterfaceTypes Type;
    public IODirection Direction;
    
    private bool isDragged;

    public enum InterfaceTypes
    {
        Event, Object
    }

    public enum IODirection
    {
        Input, Output
    }

    private BaseNode node;

    public NodeInterface(BaseNode parent)
    {
        node = parent;
    }

    public void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                Vector2.Distance(e.mousePosition, node.WindowRect.position + Position);
                Debug.Log("clicked iface");
                //selectedOutputPos = new Vector2(Transform.WindowRect.width - 7f, outputs[i].yPos);
                //chosenOutput = i;
                isDragged = true;
                e.Use();
                break;
            case EventType.MouseUp:
                isDragged = false;
                // TODO connect
                break;
            case EventType.MouseDrag:
                if (isDragged)
                {
                    DrawDraggedConnection(e.mousePosition);
                    e.Use();
                }
                break;
        }
    }

    public void SetInterface(float yPos, string label)
    {
        Label = label;

        if (Direction == IODirection.Input)
            Position.x = 7f;
        else
            Position.x = node.WindowRect.width - 7f;
    }

    public void Draw()
    {
        Vector3 position = new Vector3(node.WindowRect.width - 7f, Position.y, 0f);
        if (Direction == IODirection.Input)
            position.x = 7f;

        DrawInterfaceAt(position);
        DrawLabel();
        DrawConnections();
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

        if (ConnectedInterface !=  null || isDragged)
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

        Vector2 position = new Vector2(node.WindowRect.width - 85f, Position.y - 9);
        if (Direction == IODirection.Input)
            position.x = 15f;

        EditorGUI.LabelField(new Rect(position, new Vector2(70, 18)), Label, gs);
    }

    private void DrawCircle(Vector3 position, float radius, Color color)
    {
        Handles.color = color;
        Handles.DrawSolidDisc(position, Vector3.back, radius);
    }
    
    public void DrawConnections()
    {
        if (Direction == IODirection.Output && ConnectedInterface != null)
        {
            Rect start = new Rect(node.WindowRect.position + Position, Vector2.one);
            Rect end = new Rect(ConnectedInterface.Position, Vector2.one);

            DrawConnection(start, end, Type);
        }
    }

    public void Disconnect()
    {
        if (ConnectedInterface == null) return;
        if (Direction == IODirection.Input)
            ConnectedInterface.ConnectedInterface = null;   // TODO if multiple connections are allowed, check ID
        else
            ConnectedInterface.ConnectedInterface = null;
    }

    private void DrawDraggedConnection(Vector2 mousePos)
    {
        if (Direction == IODirection.Input && ConnectedInterface == null) return;

        Rect startRect = new Rect(Direction == IODirection.Input ? ConnectedInterface.Position : Position, Vector2.one);
        Rect endRect = new Rect(mousePos, Vector2.one);
        
        DrawConnection(startRect, endRect, Type);
    }

    private void DrawConnection(Rect start, Rect end, InterfaceTypes outputType = NodeInterface.InterfaceTypes.Event)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0f);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0f);
        Color shadowCol = new Color(0.7f, 0.7f, 1f);
        if (outputType == InterfaceTypes.Object)
            shadowCol = new Color(1f, 0.7f, 0.7f);

        Vector3 tanScale = GetTanScale(startPos, endPos);
        Vector3 startTan = startPos + tanScale;
        Vector3 endTan = endPos - tanScale;

        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol * new Color(1f, 1f, 1f, 0.06f), null, (i + 1) * 7);
        }
        Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, 3);
    }
    
    private Vector3 GetTanScale(Vector3 startPos, Vector3 endPos)
    {
        Vector3 tanScale = new Vector3(50f, 0f, 0f);

        float dX = startPos.x - endPos.x;
        if (dX > 0)
        {
            float dY = startPos.y - endPos.y;
            if (dY < 0) dY = -dY;
            tanScale += new Vector3(dX, dY, 0f) / 2f;
        }
        return tanScale;
    }




    public void ConnectInput(int inputIndex, BaseNode outNode, int outputIndex)
    {
        //if (outNode == this) return;    // Can't connect to self

        //if (outNode.OutputIsConnected(outputIndex))
        //{
        //    var originalInterface = outNode.GetConnectedInterfaceFromOutput(outputIndex);
        //    originalInterface.ConnectedNode.DisconnectInput(originalInterface.ConnectedInterfaceIndex);
        //}

        //var input = inputs[inputIndex];
        //if (input.connectedNode != null)
        //    input.connectedNode.DisconnectOutput(input.connectedInterfaceIndex);

        //input.connectedNode = outNode;
        //input.connectedInterfaceIndex = outputIndex;
        //inputs[inputIndex] = input;

        //outNode.ConnectOutput(outputIndex, this, inputIndex);
    }

    private void ConnectOutput(int outputIndex, BaseNode node, int inputIndex)
    {
        //var output = outputs[outputIndex];
        //output.connectedNode = node;
        //output.connectedInterfaceIndex = inputIndex;
        //outputs[outputIndex] = output;
    }

    public bool IsConnected()
    {
        return ConnectedInterface != null;
    }

    public BaseNode GetNode()
    {
        return node;
    }
}
