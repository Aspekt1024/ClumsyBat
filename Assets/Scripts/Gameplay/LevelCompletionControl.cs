using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelCompletionDataControl : MonoBehaviour {

    public const int NumLevels = 14;
    public const int NumTooltips = 3;   // TODO remove this

    private CompletionDataContainer.LevelType[] LevelCompletion = new CompletionDataContainer.LevelType[NumLevels];
    private bool[] ToolTipCompletion = new bool[NumTooltips];   // TODO remove this

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.OpenOrCreate);

        CompletionDataContainer GameData = new CompletionDataContainer();
        GameData.Data.LevelData = LevelCompletion;
        GameData.Data.ToolTipData = ToolTipCompletion;

        bf.Serialize(file, GameData);
        file.Close();
    }
    
    public void ClearCompletionData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.Create);

        CompletionDataContainer BlankGameData = new CompletionDataContainer();
        BlankGameData.Data.LevelData = new CompletionDataContainer.LevelType[NumLevels];
        BlankGameData.Data.ToolTipData = new bool[NumTooltips];

        bf.Serialize(file, BlankGameData);
        file.Close();
        Load();
        Debug.Log("Completion Data Cleared");
    }

    public void ResetTooltips()
    {
        ToolTipCompletion = new bool[NumTooltips];
        Save();
    }

    public void ClearLevelProgress()
    {
        LevelCompletion = new CompletionDataContainer.LevelType[NumLevels];
        Save();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/CompletionData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.Open);
            CompletionDataContainer GameData = (CompletionDataContainer)bf.Deserialize(file);
            file.Close();

            LevelCompletion = GameData.Data.LevelData;
            ToolTipCompletion = GameData.Data.ToolTipData;
        }
    }



    public void SetCompleted(int Level, bool bMainPath, bool bSecretPath1, bool bSecretPath2)
    {
        LevelCompletion[Level-1].LevelCompleted = bMainPath;
        LevelCompletion[Level-1].SecretPath1 = bSecretPath1;
        LevelCompletion[Level-1].SecretPath2 = bSecretPath2;
    }

    public void UnlockLevels(int Level, bool bMainPath, bool bSecretPath1, bool bSecretPath2)
    {
        switch (Level)
        {
            case 3:
                if (bSecretPath1)
                {
                    LevelCompletion[10-1].LevelUnlocked = true;
                }
                break;
        }
        if (bMainPath && Level < NumLevels)
        {
            LevelCompletion[Level].LevelUnlocked = true;
        }
    }

    public bool IsUnlocked(int Level) { return LevelCompletion[Level-1].LevelUnlocked; }
    public bool IsCompleted(int Level) { return LevelCompletion[Level-1].LevelCompleted; }
    public bool SecretPath1Completed(int Level) { return LevelCompletion[Level-1].SecretPath1; }
    public bool SecretPath2Completed(int Level) { return LevelCompletion[Level-1].SecretPath2; }
    public int GetNumLevels() { return NumLevels; }
}

[Serializable]
class CompletionDataContainer
{
    [Serializable]
    public struct LevelType
    {
        public bool LevelCompleted;
        public bool SecretPath1;
        public bool SecretPath2;
        public bool LevelUnlocked;
        //public bool Star1;
        //public bool Star2;
        //public bool Star3;
    }

    [Serializable]
    public struct GameDataType
    {
        public LevelType[] LevelData;
        public bool[] ToolTipData;  // TODO remove this
    }
    public GameDataType Data;
}