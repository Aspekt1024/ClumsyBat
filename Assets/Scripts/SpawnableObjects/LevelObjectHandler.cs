﻿using UnityEngine;

/// <summary>
/// Handles the generation, positioning and movement of all cave pieces
/// </summary>
public class LevelObjectHandler : MonoBehaviour {
    
    // We're going to create (or have created? how long has this comment been here?) a random level template to
    // store all possibilities of randomising the caves. 
    
    public struct CaveListType
    {
        public StalPool.StalType[] StalList;
        public ShroomPool.ShroomType[] MushroomList;
        public MothPool.MothType[] MothList;
        public SpiderPool.SpiderType[] SpiderList;
        public WebPool.WebType[] WebList;
        public TriggerHandler.TriggerType[] TriggerList;
    }
    
    private LevelContainer _level;

    private bool _bEndlessMode;
    
    private CaveHandler _cave;
    private ShroomPool _shrooms;
    private MothPool _moths;
    private StalPool _stals;
    private SpiderPool _spiders;
    private WebPool _webs;
    private TriggerHandler _triggers;
    
    void Start ()
    {
        LoadLevel();
        Debug.Log("Level " + Toolbox.Instance.Level + " loaded.");
        SetupObjectPools();
        FindObjectOfType<PlayerController>().StartGame();
    }

    public bool AtCaveEnd()
    {
        return _cave.AtCaveEnd();
    }

    private void SetupObjectPools()
    {
        GameObject caveObject = new GameObject("Caves");
        _cave = caveObject.AddComponent<CaveHandler>();

        _shrooms = new ShroomPool();
        _stals = new StalPool();
        _moths = new MothPool();
        _spiders = new SpiderPool();
        _webs = new WebPool();
        _triggers = new TriggerHandler();

        _cave.Setup(_level.Caves, _bEndlessMode, this);
    }

    void Update()
    {
        if (AtCaveEnd()) { SetVelocity(0); }
    }

    public void SetCaveObstacles(int index)
    {
        float xOffset = (index == 0 ? 0f : Toolbox.TileSizeX);

        CaveListType objectList;
        if (_bEndlessMode)
        {
            objectList = _cave.GetRandomisedObstacleList();
        }
        else
        {
            objectList = GetCaveObjectList(index);
        }
        
        if (objectList.MushroomList != null) { _shrooms.SetupMushroomsInList(objectList.MushroomList, xOffset); }
        if (objectList.StalList != null) { _stals.SetupStalactitesInList(objectList.StalList, xOffset); }
        if (objectList.MothList != null) { _moths.SetupMothsInList(objectList.MothList, xOffset); }
        if (objectList.SpiderList != null) { _spiders.SetupSpidersInList(objectList.SpiderList, xOffset); }
        if (objectList.WebList != null) { _webs.SetupWebsInList(objectList.WebList, xOffset); }
        if (objectList.TriggerList != null) { _triggers.SetupTriggersInList(objectList.TriggerList, xOffset); }
    }

    private CaveListType GetCaveObjectList(int index)
    {
        CaveListType objectList;
        objectList.StalList = _level.Caves[index].Stals;
        objectList.MushroomList = _level.Caves[index].Shrooms;
        objectList.MothList = _level.Caves[index].Moths;
        objectList.SpiderList = _level.Caves[index].Spiders;
        objectList.WebList = _level.Caves[index].Webs;
        objectList.TriggerList = _level.Caves[index].Triggers;
        return objectList;
    }

    public void SetVelocity(float speed)
    {
        if (AtCaveEnd())
        {
            _cave.SetVelocity(0);
            UpdateObjectSpeed(0);
        }
        else
        {
            _cave.SetVelocity(speed);
            UpdateObjectSpeed(speed);
        }
    }

    public void SetPaused(bool pauseGame)
    {
        // Caves?
        _shrooms.PauseGame(pauseGame);
        _stals.PauseGame(pauseGame);
        _moths.PauseGame(pauseGame);
        _spiders.PauseGame(pauseGame);
        _webs.PauseGame(pauseGame);
    }

    private void UpdateObjectSpeed(float speed)
    {
        _shrooms.SetSpeedX(speed);
        _stals.SetSpeedX(speed);
        _moths.SetSpeedX(speed);
        _spiders.SetSpeedX(speed);
        _webs.SetSpeedX(speed);
        _triggers.SetSpeedX(speed);
    }

    private void LoadLevel()
    {
        if (_bEndlessMode) { return; }
        TextAsset levelTxt = (TextAsset)Resources.Load("LevelXML/Level" + Toolbox.Instance.Level);
        _level = LevelContainer.LoadFromText(levelTxt.text);
    }
    
    public void SetMode(bool bIsEndless)
    {
        _bEndlessMode = bIsEndless;
    }
}
