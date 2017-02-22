﻿
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorHelpers {
    
    public static MonoScript[] GetScriptAssetsOfType<T>()
    {
        MonoScript[] scripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));

        List<MonoScript> result = new List<MonoScript>();

        foreach (MonoScript m in scripts)
        {
            if (m.GetClass() != null &&
                m.GetClass().IsSubclassOf(typeof(T)) &&
                m.GetType() != typeof(Shader))
            {
                result.Add(m);
            }
        }
        return result.ToArray();
    }

    public static string[] ObjectArrayToStringArray(Object[] objArray)
    {
        string[] stringArray = new string[objArray.Length];
        for (int i = 0; i < objArray.Length; i++)
        {
            stringArray[i] = objArray[i].name;
        }
        return stringArray;
    }

    public static int GetIndexFromObject(Object[] objArray, Object obj)
    {
        for (int i = 0; i < objArray.Length; i++)
        {
            if (objArray[i] == obj)
                return i;
        }
        return -1;
    }
}
