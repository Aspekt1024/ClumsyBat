using UnityEngine;
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

        if (editor.HeldObject != null)
            editor.HeldObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, editor.HeldObject.transform.position.z);
        
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
        float rotation = Random.Range(-10f, 10f);
        editor.HeldObject.transform.Rotate(Vector3.forward, rotation);
    }

    public void ScaleUp() { editor.HeldObject.transform.localScale *= 1.1f; }
    public void ScaleDown() { editor.HeldObject.transform.localScale /= 1.1f; }
    public void RandomScale()
    {
        float scale = Random.Range(1/1.2f, 1.2f);
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

    public void SelectObject(GameObject obj)
    {
        return;
        if (obj.name == "MothTrigger")
        {
            obj = obj.GetComponentInParent<Moth>().gameObject;
        }

        // TODO only pick up object if the mouse is positioned over it
        if (editor.HeldObject != obj)
            editor.HeldObject = obj;
    }

    public void DropHeldObject()
    {
        if (editor.HeldObject != null)
        {
            //Selection.activeGameObject = null;
            editor.HeldObject = null;
        }
    }

    public void PickupObject()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj == null) return;

        if (obj.name == "MothTrigger")
        {
            obj = obj.GetComponentInParent<Moth>().gameObject;
        }
        editor.HeldObject = Selection.activeGameObject;
    }
}
