using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class LevelEditorActions
{
    public LevelEditorObjectHandler objectHandler = new LevelEditorObjectHandler();
    private LevelEditor editor;

    public void ProcessEvent(Vector3 mousePosition, LevelEditor editorRef)
    {
        if (SceneManager.GetActiveScene().name != "LevelEditor") return;
        editor = editorRef;

        if (editor.HeldObject != null)
            editor.HeldObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, editor.HeldObject.transform.position.z);

        // TODO decouple inputs
        if (Event.current.type == EventType.keyUp)
        {
            if (editor.HeldObject != null)
                ProcessHeldKeyUp();
            else
                ProcessFreeKeyUp();
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            if (editor.HeldObject != null)
                ProcessHeldMouseUp();
            else
                ProcessFreeMouseUp();
        }
        else
        {
            objectHandler.GUIEvent();
        }
    }

    private void ProcessHeldKeyUp()
    {
        bool bUnused = false;
        switch (Event.current.keyCode)
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
                editor.HeldObject = objectHandler.SpawnObject<MothEditorHandler>();
                break;
            case KeyCode.Keypad2:
                editor.HeldObject = objectHandler.SpawnObject<StalEditorHandler>();
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
        switch (Event.current.button)
        {
            case 1:
                editor.HeldObject = null;
                break;
        }
    }

    private void ProcessFreeMouseUp()
    {

    }

    private void RotateLeft()
    {

    }

    private void RotateRight()
    {

    }
}
