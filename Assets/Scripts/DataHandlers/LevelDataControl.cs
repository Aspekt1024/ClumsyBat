using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelDataControl : MonoBehaviour {

    public const int NumLevels = 14;

    private LevelDataContainer.LevelType[] LevelCompletion = new LevelDataContainer.LevelType[NumLevels];

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.OpenOrCreate);

        LevelDataContainer GameData = new LevelDataContainer();
        GameData.Data.LevelData = LevelCompletion;

        bf.Serialize(file, GameData);
        file.Close();
    }
    
    public void ClearCompletionData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.Create);

        LevelDataContainer BlankGameData = new LevelDataContainer();
        BlankGameData.Data.LevelData = new LevelDataContainer.LevelType[NumLevels];

        bf.Serialize(file, BlankGameData);
        file.Close();
        Load();
        Debug.Log("Level Data Cleared");
    }

    public void ClearLevelProgress()
    {
        LevelCompletion = new LevelDataContainer.LevelType[NumLevels];
        Save();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/CompletionData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.Open);
            LevelDataContainer GameData = (LevelDataContainer)bf.Deserialize(file);
            file.Close();

            LevelCompletion = GameData.Data.LevelData;
        }
    }

    public void SetCompleted(int Level, LevelDataContainer.LevelType LevelData)
    {
        LevelCompletion[Level-1].LevelCompleted &= LevelData.LevelCompleted;
        LevelCompletion[Level-1].SecretPath1 &= LevelData.SecretPath1;
        LevelCompletion[Level - 1].SecretPath2 &= LevelData.SecretPath2;
        LevelCompletion[Level - 1].Star1 &= LevelData.Star1;
        LevelCompletion[Level - 1].Star2 &= LevelData.Star2;
        LevelCompletion[Level - 1].Star3 &= LevelData.Star3;

    }

    public void UnlockLevels(int Level, LevelDataContainer.LevelType LevelData)
    {
        switch (Level)
        {
            case 3:
                if (LevelData.SecretPath1)
                {
                    LevelCompletion[10-1].LevelUnlocked = true; // TODO define which secret paths point where
                }
                break;
        }
        if (LevelData.LevelCompleted && Level < NumLevels)
        {
            LevelCompletion[Level].LevelUnlocked = true;
        }
    }

    public bool IsUnlocked(int Level) { return LevelCompletion[Level-1].LevelUnlocked; }
    public bool IsCompleted(int Level) { return LevelCompletion[Level-1].LevelCompleted; }
    public bool SecretPath1Completed(int Level) { return LevelCompletion[Level-1].SecretPath1; }
    public bool SecretPath2Completed(int Level) { return LevelCompletion[Level-1].SecretPath2; }
    public bool AllMothsGathered(int Level) { return LevelCompletion[Level - 1].Star1; }
    public int GetNumLevels() { return NumLevels; }
}

[Serializable]
public class LevelDataContainer
{
    [Serializable]
    public struct LevelType
    {
        public bool LevelCompleted;
        public bool LevelUnlocked;
        public bool SecretPath1;
        public bool SecretPath2;
        public bool Star1;
        public bool Star2;
        public bool Star3;
    }

    [Serializable]
    public struct GameDataType
    {
        public LevelType[] LevelData;
    }
    public GameDataType Data;
}