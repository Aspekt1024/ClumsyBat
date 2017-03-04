using UnityEditor;
using UnityEngine;

public static class EditorHelpers {
    
    public static string GetAssetDataFolder(ScriptableObject obj)
    {
        string assetPath = AssetDatabase.GetAssetPath(obj);
        assetPath = assetPath.Substring(0, assetPath.Length - (string.Format("/{0}.asset", obj.name)).Length);

        string dataFolder = obj.name + "Data";
        CreateFolderIfNotExist(assetPath, dataFolder);

        return string.Format("{0}/{1}/", assetPath, dataFolder);
    }

    public static void CreateFolderIfNotExist(string assetPath, string dataFolder)
    {
        if (!AssetDatabase.IsValidFolder(string.Format("{0}/{1}", assetPath, dataFolder)))
            AssetDatabase.CreateFolder(assetPath, dataFolder);
    }

    public static void SaveObjectToFolder(ScriptableObject sObj, string dataFolder, string assetName)
    {
        if (AssetDatabase.Contains(sObj)) return;

        int assetNum = 1;
        string assetPath = string.Format("{0}{1}", dataFolder, assetName);

        while (AssetDatabase.LoadAssetAtPath<BaseNode>(string.Format("{0}{1}.asset", assetPath, assetNum)) != null)
        {
            assetNum++;
        }

        AssetDatabase.CreateAsset(sObj, string.Format("{0}{1}.asset", assetPath, assetNum));
    }
}
