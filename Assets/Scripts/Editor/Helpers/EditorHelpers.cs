using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class EditorHelpers {

    private const string savesPath = "Assets/Scripts/Editor/NodeSaves";

    public static void SaveNodeEditorAsset(ScriptableObject obj, ScriptableObject parent, string subFolder = "", string assetName = "")
    {
        string dataPath = GetDataPath(parent);
        dataPath = AppendSubFolder(dataPath, subFolder);

        //Debug.Log("Creating asset " + obj + " for " + parent + " called " + assetName + " in " + dataPath);
        SaveObjectToFolder(obj, dataPath, assetName);
    }

    public static string GetDataPath(ScriptableObject parent)
    {
        string dataFolder = parent.name + "Data";
        CreateFolderIfNotExist(savesPath, dataFolder);
        return string.Format("{0}/{1}", savesPath, dataFolder);
    }

    // TODO used for saving near scriptable object for in-game data. this is in resources.
    public static string GetAssetDataFolder(ScriptableObject obj)
    {
        string assetPath = AssetDatabase.GetAssetPath(obj);
        assetPath = assetPath.Substring(0, assetPath.Length - (string.Format("/{0}.asset", obj.name)).Length);

        string dataFolder = obj.name + "Data";
        CreateFolderIfNotExist(assetPath, dataFolder);

        return string.Format("{0}/{1}", assetPath, dataFolder);
    }

    private static string AppendSubFolder(string dataPath, string subFolder)
    {
        if (subFolder.Length > 0)
        {
            CreateFolderIfNotExist(dataPath, subFolder);
            dataPath = string.Format("{0}/{1}", dataPath, subFolder);
        }
        return dataPath;
    }

    public static void CreateFolderIfNotExist(string assetPath, string dataFolder)
    {
        if (!AssetDatabase.IsValidFolder(string.Format("{0}/{1}", assetPath, dataFolder)))
            AssetDatabase.CreateFolder(assetPath, dataFolder);
    }

    private static void SaveObjectToFolder(ScriptableObject obj, string dataFolder, string assetName)
    {
        int assetNum = 1;
        
        while (AssetDatabase.LoadAssetAtPath<ScriptableObject>(string.Format("{0}/{1}{2}.asset", dataFolder, assetName, assetNum)) != null)
        {
            assetNum++;
        }
        string assetFileName = string.Format("{0}{1}.asset", assetName, assetNum);

        if (AssetDatabase.Contains(obj))
            MoveIfPathDifferent(obj, dataFolder, assetFileName);
        else
        {
            AssetDatabase.CreateAsset(obj, string.Format("{0}/{1}", dataFolder, assetFileName));
        }
    }

    private static void MoveIfPathDifferent(ScriptableObject obj, string dataFolder, string fileName)
    {
        string existingPath = AssetDatabase.GetAssetPath(obj);
        string existingFolder = existingPath.Substring(0, existingPath.Length - (string.Format("/{0}.asset", obj.name)).Length);

        if (dataFolder != existingFolder)
            AssetDatabase.MoveAsset(existingPath, string.Format("{0}/{1}", dataFolder, fileName));
    }
}
