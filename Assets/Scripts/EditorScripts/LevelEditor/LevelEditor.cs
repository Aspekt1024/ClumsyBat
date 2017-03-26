using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class LevelEditor : MonoBehaviour {

    public LevelContainer Level;
    public bool EditMode;
    public bool DebugMode;
    public LevelProgressionHandler.Levels LevelId;

    public LevelEditorObjectHandler objectHandler;
    private GameObject heldObject;
    
    public void ProcessEvent()
    {
        if (SceneManager.GetActiveScene().name != "LevelEditor") return;
        
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
        mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0f;
        
        if (heldObject != null)
            heldObject.transform.position = mousePosition;
        
        if (Event.current.type == EventType.keyUp)
        {
            if (heldObject != null)
                ProcessHeldKeyUp();
            else
                ProcessFreeKeyUp();
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            if (heldObject != null) 
                ProcessHeldMouseUp();
            else
                ProcessFreeMouseUp();
        }
        else if (Event.current.type == EventType.Layout)
        {
            if (objectHandler == null)
                objectHandler = new LevelEditorObjectHandler();
            objectHandler.GUIEvent();
        }
    }
    
    private void ProcessHeldKeyUp()
    {
        Debug.Log("keyup");
        bool bUnused = false;
        switch(Event.current.keyCode)
        {
            case KeyCode.A:
                RotateLeft();
                break;
            case KeyCode.D:
                RotateRight();
                break;
            default:
                bUnused = true;
                break;
        }
        if (!bUnused)
            Event.current.Use();
    }

    private void ProcessFreeKeyUp()
    {
        bool bUnused = false;
        switch (Event.current.keyCode)
        {
            case KeyCode.Keypad1:
                // TODO setup spawner
                heldObject = Object.Instantiate(Resources.Load<GameObject>("Collectibles/Moth"));
                break;
            case KeyCode.Keypad2:
                heldObject = Object.Instantiate(Resources.Load<GameObject>("Obstacles/EditorStalactite"));
                break;
            default:
                bUnused = true;
                break;
        }
        if (!bUnused)
            Event.current.Use();
    }

    private void ProcessHeldMouseUp()
    {
        bool bUnused = false;
        
        switch (Event.current.button)
        {
            case 1:
                heldObject = null;
                break;
            default:
                bUnused = true;
                break;
        }

    }

    private void ProcessFreeMouseUp()
    {

        bool bUnused = false;

        bUnused = true;

        if (!bUnused)
            Event.current.Use();
    }

    private void RotateLeft()
    {

    }

    private void RotateRight()
    {

    }

    public void LoadLevel()
    {
        SaveLevelHandler saveHandler = new SaveLevelHandler();
        saveHandler.Load(objectHandler, LevelId);
    }

    public void SaveLevel()
    {
        SaveLevelHandler saveHandler = new SaveLevelHandler();
        saveHandler.Save(objectHandler, LevelId);
    }
}
