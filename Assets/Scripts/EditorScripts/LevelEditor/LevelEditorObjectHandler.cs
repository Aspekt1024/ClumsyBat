using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorObjectHandler {
    
    public List<BaseObjectHandler> ObjHandlers;

    public LevelEditorObjectHandler()
    {
        ObjHandlers = new List<BaseObjectHandler>();
        ObjHandlers.Add(new CaveEditorHandler(this));
        ObjHandlers.Add(new MothEditorHandler(this));
    }

    public void GetCaveList()
    {

    }

    public void GUIEvent()
    {
        Update();
    }
    
    private void Update ()
    {
        foreach(var objHandler in ObjHandlers)
        {
            objHandler.GUIUpdate();
        }
    }
}
