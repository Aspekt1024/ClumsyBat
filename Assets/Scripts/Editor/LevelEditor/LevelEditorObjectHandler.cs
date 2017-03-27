using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class LevelEditorObjectHandler {
    
    public List<BaseObjectHandler> ObjHandlers;
    
    public LevelEditorObjectHandler()
    {
        ObjHandlers = new List<BaseObjectHandler>();

        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        foreach(Type type in types)
        {
            if (type.IsSubclassOf(typeof(BaseObjectHandler)))
            {
                ObjHandlers.Add((BaseObjectHandler)Activator.CreateInstance(type, this));
            }
        }
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

    public int GetNumSections()
    {
        foreach (var objHandler in ObjHandlers)
        {
            if (objHandler.IsType<CaveEditorHandler>())
                return ((CaveEditorHandler)objHandler).GetNumSections();
        }
        return 0;
    }

    public GameObject SpawnObject<T>() where T : BaseObjectHandler
    {
        BaseObjectHandler handler = null;
        foreach (var h in ObjHandlers)
        {
            if (h.IsType<T>())
            {
                handler = h;
                break;
            }
        }
        if (handler == null) return null;
        
        return handler.CreateNewObject();
    }
}
