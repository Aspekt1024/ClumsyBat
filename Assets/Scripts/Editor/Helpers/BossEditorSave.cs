using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BossEditorSave {

    private BossDataContainer dataContainer;
    private BossEditorNodeData nodeData;
    private string dataPath;
    
    public void CreateActionFile(BossDataContainer baseContainer, BossEditorNodeData bossNodeData)
    {
        dataContainer = baseContainer;
        nodeData = bossNodeData;
        dataPath = GetActionsDataPath();

        AssetDatabase.DeleteAsset(dataPath);
        SetStartingAction();
        if (dataContainer.StartingAction == null) return;

        AssetDatabase.CreateAsset(dataContainer.StartingAction, dataPath);
        SaveActionData();
    }

    private void SetStartingAction()
    {
        // The start node is the parent file object
        foreach (var node in nodeData.Nodes)
        {
            if (node.IsType<StartNode>())
                dataContainer.StartingAction = (StartAction)node.GetAction();
        }
    }

    private string GetActionsDataPath()
    {
        string dataFolder = EditorHelpers.GetAssetDataFolder(dataContainer.RootContainer);
        if (dataContainer.IsType<BossState>())
        {
            string subFolder = "StateData";
            EditorHelpers.CreateFolderIfNotExist(dataFolder, subFolder);
            string assetName = ((BossState)dataContainer).StateName.Replace(" ", "");
            dataPath = string.Format("{0}/{1}/{2}.asset", dataFolder, subFolder, assetName);
        }
        else
        {
            dataPath = string.Format("{0}/StateMachine.asset", dataFolder);
        }
        return dataPath;
    }

    private void SaveActionData()
    {
        GetNodeActions();
        ConvertNodeInterfaces();
        SaveActions();
        CommitAssetDatabase();
    }

    private void GetNodeActions()
    {
        dataContainer.Actions = new List<BaseAction>();
        foreach (var node in nodeData.Nodes)
        {
            var action = node.GetAction();
            dataContainer.Actions.Add(action);
        }
    }

    private void ConvertNodeInterfaces()
    {
        foreach(var node in nodeData.Nodes)
        {
            node.ConvertInterfaces();
        }
    }

    private void SaveActions()
    {
        foreach (var action in dataContainer.Actions)
        {
            if (AssetDatabase.Contains(action))
            {
                EditorUtility.SetDirty(action); // TODO set dirty is legacy
            }
            else
            {
                AssetDatabase.AddObjectToAsset(action, dataPath);
            }
        }
    }

    private void CommitAssetDatabase()
    {
        foreach(var node in nodeData.Nodes)
        {
            EditorUtility.SetDirty(node);
        }

        EditorUtility.SetDirty(nodeData);
        EditorUtility.SetDirty(dataContainer);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
