using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CompletionDataControl {

    public const int NumLevels = 14;

    private CompletionDataContainer.LevelType[] LevelCompletion = new CompletionDataContainer.LevelType[NumLevels];

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.OpenOrCreate);

        CompletionDataContainer LevelData = new CompletionDataContainer();
        LevelData.Data = LevelCompletion;

        bf.Serialize(file, LevelData);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/CompletionData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.Open);
            CompletionDataContainer LevelData = (CompletionDataContainer)bf.Deserialize(file);
            file.Close();

            LevelCompletion = LevelData.Data;
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
    }
    public LevelType[] Data;

    // TODO level tooltips
}