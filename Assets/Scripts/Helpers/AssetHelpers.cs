using System;
using System.Collections.Generic;
using UnityEngine;

public static class AssetHelpers {

    public static Type[] GetScriptAssetsOfType<T>()
    {
        UnityEngine.Object[] scripts = Resources.FindObjectsOfTypeAll(typeof(T));

        List<Type> result = new List<Type>();

        foreach (var script in scripts)
        {
            if (!script.GetType().Equals(typeof(T)))
            {
                result.Add(script.GetType());
            }
        }
        return result.ToArray();
    }
}
