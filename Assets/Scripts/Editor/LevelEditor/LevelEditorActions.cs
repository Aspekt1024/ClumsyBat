﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LevelEditorActions
{
    public LevelEditorObjectHandler ObjectHandler = new LevelEditorObjectHandler();
    
    private LevelEditorInputHandler inputHandler = new LevelEditorInputHandler();
    private LevelEditor editor;

    public void ProcessEvent(Vector3 mousePosition, LevelEditor editorRef)
    {
        if (SceneManager.GetActiveScene().name != "LevelEditor") return;

        editor = editorRef;
        
        inputHandler.ProcessInput(editor, this);

        PositionHeldObject(mousePosition);
        
        if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint)
        {
            ObjectHandler.GUIEvent();
        }
    }
    
    public void RotateLeft() { editor.HeldObject.transform.Rotate(Vector3.forward, 10f); }
    public void RotateRight() { editor.HeldObject.transform.Rotate(Vector3.forward, -10f); }
    public void Flip() { editor.HeldObject.transform.Rotate(Vector3.forward, 180f); }
    public void RandomRotation()
    {
        float rotation = UnityEngine.Random.Range(-10f, 10f);
        editor.HeldObject.transform.Rotate(Vector3.forward, rotation);
    }

    public void ScaleUp() { editor.HeldObject.transform.localScale *= 1.1f; }
    public void ScaleDown() { editor.HeldObject.transform.localScale /= 1.1f; }
    public void RandomScale()
    {
        float scale = UnityEngine.Random.Range(1/1.2f, 1.2f);
        editor.HeldObject.transform.localScale *= scale;
    }

    public void ResetRotationAndScale()
    {
        editor.HeldObject.transform.localRotation = new Quaternion();
        editor.HeldObject.transform.localScale = Vector3.one;
    }

    public void LevelEditorInspector()
    {
        Selection.activeObject = editor.gameObject;
    }
    
    public void DropHeldObject()
    {
        editor.HeldObject = null;
        editor.PickupOffset = Vector2.zero;
    }

    public void DestroyHeldObject()
    {
        GameObject.DestroyImmediate(editor.HeldObject);
        editor.HeldObject = null;
    }

    public void PickupObject(Vector2 screenPos)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (!hit) return;

        GameObject obj = hit.collider.gameObject;

        if (obj.name == "MothTrigger")
        {
            obj = obj.GetComponentInParent<Moth>().gameObject;
        }

        editor.PickupOffset = ray.origin - obj.transform.position;
        editor.HeldObject = obj;
        Selection.activeObject = obj;
    }

    private void PositionHeldObject(Vector2 mousePos)
    {
        if (editor.HeldObject == null) return;

        float xPos = mousePos.x - editor.PickupOffset.x;
        float yPos = mousePos.y - editor.PickupOffset.y;
        editor.HeldObject.transform.position = new Vector3(xPos, yPos, editor.HeldObject.transform.position.z);
    }

    public void AddCavePiece()
    {
        if (Selection.activeGameObject == null) return;
        string caveName = Selection.activeGameObject.name;
        if (!caveName.Contains("Cave") && !caveName.Contains("Entrance")) return;

        float xPos = Selection.activeGameObject.transform.position.x;
        foreach (var handler in ObjectHandler.ObjHandlers)
        {
            handler.ShiftRightIfAfterThreshold(xPos + LevelEditorConstants.TileSizeX / 2f);
            if (handler.IsType<CaveEditorHandler>())
            {
                ((CaveEditorHandler)handler).CreateNewTopCave(xPos + LevelEditorConstants.TileSizeX);
                ((CaveEditorHandler)handler).CreateNewBottomCave(xPos + LevelEditorConstants.TileSizeX);
            }
        }
    }

    public void RemoveCavePiece()
    {
        if (Selection.activeGameObject == null) return;
        string caveName = Selection.activeGameObject.name;
        if (!caveName.Contains("Cave") || caveName == "Caves") return;
        
        float xPos = Selection.activeGameObject.transform.position.x;
        foreach (var handler in ObjectHandler.ObjHandlers)
        {
            if (handler.IsType<CaveEditorHandler>())
            {
                GameObject[] cavePieces = ((CaveEditorHandler)handler).GetCavesAtIndex(xPos);
                Object.DestroyImmediate(cavePieces[0]);
                Object.DestroyImmediate(cavePieces[1]);
            }
            else
            {
                handler.DeleteIfWithinRange(xPos - LevelEditorConstants.TileSizeX / 2f, xPos + LevelEditorConstants.TileSizeX / 2f);
            }
            handler.ShiftLeftIfAfterThreshold(xPos + LevelEditorConstants.TileSizeX / 2f);
        }
    }

    public void SetCaveType(int caveType)
    {
        if (Selection.activeGameObject == null) return;
        string caveName = Selection.activeGameObject.name;

        float xPos = Selection.activeGameObject.transform.position.x;

        CaveEditorHandler caveHandler = null;
        foreach (var handler in ObjectHandler.ObjHandlers)
        {
            if (handler.IsType<CaveEditorHandler>())
                caveHandler = ((CaveEditorHandler)handler);
        }

        if (caveHandler == null) return;

        GameObject newCave = null;
        if (caveName.Contains("CaveTop"))
            newCave = caveHandler.CreateNewTopCave(xPos, caveType);
        else if (caveName.Contains("CaveBottom"))
            newCave = caveHandler.CreateNewBottomCave(xPos, caveType);
        else
            return;

        Object.DestroyImmediate(Selection.activeGameObject);
        Selection.activeGameObject = newCave;

    }
}
