using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorObjectHandler {
    
    private List<BaseObjectHandler> objHandlers;

    public LevelEditorObjectHandler()
    {
        objHandlers = new List<BaseObjectHandler>();
        objHandlers.Add(new CaveEditorHandler(this));
        objHandlers.Add(new MothEditorHandler(this));
    }

    public void GUIEvent()
    {
        Update();
    }
    
    private void Update ()
    {
        foreach(var objHandler in objHandlers)
        {
            objHandler.GUIUpdate();
        }
    }
}
