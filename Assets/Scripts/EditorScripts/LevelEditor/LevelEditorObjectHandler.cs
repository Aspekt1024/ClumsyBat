using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class LevelEditorObjectHandler {
    
    public List<BaseObjectHandler> ObjHandlers;

    private int numSections;

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
            if (objHandler.IsType<CaveEditorHandler>())
            {
                numSections = ((CaveEditorHandler)objHandler).GetNumSections();
            }
        }
    }

    public int GetNumSections()
    {
        return numSections;
    }
}
