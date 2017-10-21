﻿using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelDataControl : MonoBehaviour {
    
    private LevelDataContainer.LevelType[] _levelCompletion;

    public int NumLevels { get; private set; }

    public LevelDataControl()
    {
        NumLevels = Enum.GetNames(typeof(LevelProgressionHandler.Levels)).Length;
        _levelCompletion = new LevelDataContainer.LevelType[NumLevels];
    }

    private void OnEnable()
    {
        EventListener.OnLevelWon += OnLevelWon;
    }
    private void OnDisable()
    {
        EventListener.OnLevelWon -= OnLevelWon;
    }

    public void UnlockAllLevels()
    {
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main2].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main3].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main4].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main5].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main6].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main7].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main8].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main9].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main10].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main11].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main12].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main13].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main14].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main15].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Main16].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Boss1].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Boss2].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Boss3].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Boss4].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Boss5].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Boss6].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.Boss7].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.BossS1].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.BossS2].LevelUnlocked = true;
        _levelCompletion[(int)LevelProgressionHandler.Levels.BossS3].LevelUnlocked = true;
        Save();
    }

    private void OnLevelWon()
    {
        var levelCompletion = GameData.Instance.GetLevelCompletion();
        var level = GameData.Instance.Level;

        SetCompleted(level);
        if (levelCompletion.SecretPath1)
        {
            level = LevelProgressionHandler.GetSecretLevel1(level);
            UnlockLevel(level);
        }
        else if (levelCompletion.SecretPath2)
        {
            level = LevelProgressionHandler.GetSecretLevel2(level);
            UnlockLevel(level);
        }
        else if (levelCompletion.LevelCompleted)
        {
            level = LevelProgressionHandler.GetNextLevel(level);
            UnlockLevel(level);
        }

        GameData.Instance.Data.Stats.LevelsCompleted++;
        GameData.Instance.Data.SaveData();
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
        blankGameData.Data.LevelData = new LevelDataContainer.LevelType[NumLevels];

        bf.Serialize(file, blankGameData);
        file.Close();
        Load();
        Debug.Log("Level Data Cleared");
    }

    public void ClearLevelProgress()
    {
        _levelCompletion = new LevelDataContainer.LevelType[NumLevels];
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

        GameData.Instance.Achievements[0] = _levelCompletion[level].Achievement1 ? GameData.AchievementStatus.Achieved : GameData.AchievementStatus.Unachieved;
        GameData.Instance.Achievements[1] = _levelCompletion[level].Achievement2 ? GameData.AchievementStatus.Achieved : GameData.AchievementStatus.Unachieved;
        GameData.Instance.Achievements[2] = _levelCompletion[level].Achievement3 ? GameData.AchievementStatus.Achieved : GameData.AchievementStatus.Unachieved;

        _levelCompletion[level].LevelCompleted |= lvlCompletion.LevelCompleted;
        _levelCompletion[level].SecretPath1 |= lvlCompletion.SecretPath1;
        _levelCompletion[level].SecretPath2 |= lvlCompletion.SecretPath2;

        if (levelId.ToString().Contains("Boss"))
        {
            _levelCompletion[level].Achievement1 |= _levelCompletion[level].LevelCompleted;
            _levelCompletion[level].Achievement2 |= GameData.Instance.OnlyOneDamageTaken;
            _levelCompletion[level].Achievement3 |= GameData.Instance.IsUntouched;
        }
        else
        {
            _levelCompletion[level].Achievement1 |= GameData.Instance.Data.Stats.MothsEaten == GameData.Instance.NumMoths;
            _levelCompletion[level].Achievement2 |= GameData.Instance.IsUntouched;
            _levelCompletion[level].Achievement3 |= GameData.Instance.Data.Stats.Score >= GameData.Instance.ScoreToBeat;
        }

        if (_levelCompletion[level].Achievement1 && GameData.Instance.Achievements[0] == GameData.AchievementStatus.Unachieved) GameData.Instance.Achievements[0] = GameData.AchievementStatus.NewAchievement;
        if (_levelCompletion[level].Achievement2 && GameData.Instance.Achievements[1] == GameData.AchievementStatus.Unachieved) GameData.Instance.Achievements[1] = GameData.AchievementStatus.NewAchievement;
        if (_levelCompletion[level].Achievement3 && GameData.Instance.Achievements[2] == GameData.AchievementStatus.Unachieved) GameData.Instance.Achievements[2] = GameData.AchievementStatus.NewAchievement;
    }

    public void SetBestScore(int level, int score)
    {
        _levelCompletion[level].BestScore = score;
        UpdateHighScore();
    }

    private void UpdateHighScore()
    {
        long totalScore = 0;
        foreach (LevelDataContainer.LevelType level in _levelCompletion)
        {
            totalScore += level.BestScore;
        }
        PlayGamesScript.AddHighScore(totalScore);
    }

    public void UnlockLevel(LevelProgressionHandler.Levels levelId)
    {
        var level = (int)levelId;
        if (level < NumLevels)
        {
            _levelCompletion[level].LevelUnlocked = true;
        }
    }

    public LevelProgressionHandler.Levels GetHighestLevel()
    {
        // This ignores secret levels (i.e. this is the main path only)
        LevelProgressionHandler.Levels level = LevelProgressionHandler.Levels.Main1;
        while (level < LevelProgressionHandler.Levels.Boss5 && (_levelCompletion[(int)level].LevelUnlocked || level == LevelProgressionHandler.Levels.Main1))
        {
            LevelProgressionHandler.Levels nextLevel = LevelProgressionHandler.GetNextLevel(level);
            if (_levelCompletion[(int)nextLevel].LevelUnlocked)
                level = nextLevel;
            else
                break;
        }
        return level;
    }

    public bool IsUnlocked(int level) { return _levelCompletion[level].LevelUnlocked; }
    public bool IsCompleted(int level) { return _levelCompletion[level].LevelCompleted; }
    public bool SecretPath1Completed(int level) { return _levelCompletion[level].SecretPath1; }
    public bool SecretPath2Completed(int level) { return _levelCompletion[level].SecretPath2; }
    public bool LevelCompletedAchievement(int level) { return _levelCompletion[level].Achievement1; }
    public bool AllMothsGathered(int level) { return _levelCompletion[level].Achievement2; }
    public bool NoDamageTaken(int level) { return _levelCompletion[level].Achievement3; }
    public int GetBestScore(int level) { return _levelCompletion[level].BestScore; }
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
        public int BestScore;
    }

    [Serializable]
    public struct GameDataType
    {
        public LevelType[] LevelData;
    }
    public GameDataType Data;
}