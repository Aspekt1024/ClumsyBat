using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelDataControl : MonoBehaviour {

    private LevelDataContainer.LevelType[] _levelCompletion = new LevelDataContainer.LevelType[GameData.NumLevels];
    private LevelProgressionHandler _levelProgresssionHandler;

    private void Awake()
    {
        _levelProgresssionHandler = new LevelProgressionHandler(this);
    }
    
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.OpenOrCreate);

        LevelDataContainer gameData = new LevelDataContainer();
        gameData.Data.LevelData = _levelCompletion;

        bf.Serialize(file, gameData);
        file.Close();
    }
    
    public void ClearCompletionData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.Create);

        LevelDataContainer blankGameData = new LevelDataContainer();
        blankGameData.Data.LevelData = new LevelDataContainer.LevelType[GameData.NumLevels];

        bf.Serialize(file, blankGameData);
        file.Close();
        Load();
        Debug.Log("Level Data Cleared");
    }

    public void ClearLevelProgress()
    {
        _levelCompletion = new LevelDataContainer.LevelType[GameData.NumLevels];
        Save();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/CompletionData.dat"))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + "/CompletionData.dat", FileMode.Open);
            var gameData = (LevelDataContainer)bf.Deserialize(file);
            file.Close();

            _levelCompletion = gameData.Data.LevelData;
        }
    }

    public void SetCompleted(LevelProgressionHandler.Levels levelId)
    {
        var level = (int)levelId;
        var lvlCompletion = GameData.Instance.GetLevelCompletion();
        _levelCompletion[level].LevelCompleted |= lvlCompletion.LevelCompleted;
        _levelCompletion[level].SecretPath1 |= lvlCompletion.SecretPath1;
        _levelCompletion[level].SecretPath2 |= lvlCompletion.SecretPath2;
        _levelCompletion[level].Achievement1 |= lvlCompletion.Achievement1;
        _levelCompletion[level].Achievement2 |= lvlCompletion.Achievement2;
        _levelCompletion[level].Achievement3 |= lvlCompletion.Achievement3;

    }

    public void UnlockLevel(LevelProgressionHandler.Levels levelId)
    {
        var level = (int)levelId;
        if (level < GameData.NumLevels)
        {
            _levelCompletion[level].LevelUnlocked = true;
        }
    }

    public bool IsUnlocked(int level) { return _levelCompletion[level].LevelUnlocked; }
    public bool IsCompleted(int level) { return _levelCompletion[level].LevelCompleted; }
    public bool SecretPath1Completed(int level) { return _levelCompletion[level].SecretPath1; }
    public bool SecretPath2Completed(int level) { return _levelCompletion[level].SecretPath2; }
    public bool AllMothsGathered(int level) { return _levelCompletion[level].Achievement1; }
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
        public bool Achievement1;
        public bool Achievement2;
        public bool Achievement3;
    }

    [Serializable]
    public struct GameDataType
    {
        public LevelType[] LevelData;
    }
    public GameDataType Data;
}