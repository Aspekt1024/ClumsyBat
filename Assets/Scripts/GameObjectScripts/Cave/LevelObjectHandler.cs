using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the generation, positioning and movement of all cave pieces
/// </summary>
public class LevelObjectHandler : MonoBehaviour {

    // This handles the Cave object pools
    // TODO rename this 'Level' because it's no longer taking care of just caves?
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
    
    private LevelContainer Level;

    private CaveRandomiser EndlessCave;

    private bool bEndlessMode = false;
    private int CaveIndex = 0;

    private CavePool Caves;
    private ShroomPool Shrooms;
    private MothPool Moths;
    private StalPool Stals;
    private SpiderPool Spiders;
    private WebPool Webs;
    private TriggerHandler Triggers;

    void Start ()
    {   
        SetupObjectPools();
        LoadLevel();
        EndlessCave = new CaveRandomiser();
        SetupStartingCaves();

        Debug.Log("Level " + Toolbox.Instance.Level + " loaded.");
        GameObject.Find("Clumsy").GetComponent<PlayerController>().LevelStart();
    }

    public bool AtCaveEnd()
    {
        return Caves.AtCaveEnd();
    }

    private void SetupObjectPools()
    {
        GameObject CaveObject = new GameObject("Caves");
        Caves = CaveObject.AddComponent<CavePool>();

        Shrooms = new ShroomPool();
        Stals = new StalPool();
        Moths = new MothPool();
        Spiders = new SpiderPool();
        Webs = new WebPool();
        Triggers = new TriggerHandler();
    }

    void Update()
    {
        if (AtCaveEnd())
        {
            SetVelocity(0);
        }
        if (CaveIndex > Level.Caves.Length) { return; }
        if (Caves.GetPositionX() <= 0)
        {
            SetNextCavePiece();
        }
    }

    private void SetNextCavePiece()
    {
        CaveIndex++;
        int NextTopCaveType = GetNextTopCaveType();
        int NextBottomCaveType = GetNextBottomCaveType();

        bool NextTopIsSecret = false;
        bool NextBottomIsSecret = false;
        if (CaveIndex < Level.Caves.Length)
        {
            NextTopIsSecret = Level.Caves[CaveIndex].bTopSecretPath;
            NextBottomIsSecret = Level.Caves[CaveIndex].bBottomSecretPath;
        }
        Caves.SetNextCavePiece(NextTopCaveType, NextBottomCaveType, NextTopIsSecret, NextBottomIsSecret);

        if (CaveIndex < Level.Caves.Length || bEndlessMode)
        {
            SetCaveObstacles(CaveIndex);
        }
    }

    private int GetNextTopCaveType()
    {
        int NextTopCaveType;
        if (bEndlessMode) {
            NextTopCaveType = EndlessCave.GetRandomTopType();
        } else {
            if (CaveIndex >= Level.Caves.Length) {
                NextTopCaveType = -1;
            } else {
                NextTopCaveType = Level.Caves[CaveIndex].TopIndex;
            }
        }
        return NextTopCaveType;
    }

    private int GetNextBottomCaveType()
    {
        int NextBottomCaveType;
        if (bEndlessMode) {
            NextBottomCaveType = EndlessCave.GetRandomBottomType();
        } else {
            if (CaveIndex >= Level.Caves.Length) {
                NextBottomCaveType = -1;
            } else {
                NextBottomCaveType = Level.Caves[CaveIndex].BottomIndex;
            }
        }
        return NextBottomCaveType;
    }

    private void SetCaveObstacles(int Index)
    {
        float XOffset = Toolbox.TileSizeX;
        if (Index == 0)
        {
            XOffset = 0f;
        }
        CaveListType ObjectList;
        if (bEndlessMode)
        {
            ObjectList = EndlessCave.RandomiseObstacleList();
        }
        else
        {
            ObjectList = GetCaveObjectList(Index);
        }

        if (ObjectList.MushroomList != null) { Shrooms.SetupMushroomsInList(ObjectList.MushroomList, XOffset); }
        if (ObjectList.StalList != null) { Stals.SetupStalactitesInList(ObjectList.StalList, XOffset); }
        if (ObjectList.MothList != null) { Moths.SetupMothsInList(ObjectList.MothList, XOffset); }
        if (ObjectList.SpiderList != null) { Spiders.SetupSpidersInList(ObjectList.SpiderList, XOffset); }
        if (ObjectList.WebList != null) { Webs.SetupWebsInList(ObjectList.WebList, XOffset); }
        if (ObjectList.TriggerList != null) { Triggers.SetupTriggersInList(ObjectList.TriggerList, XOffset); }
    }

    private CaveListType GetCaveObjectList(int Index)
    {
        CaveListType ObjectList;
        ObjectList.StalList = Level.Caves[Index].Stals;
        ObjectList.MushroomList = Level.Caves[Index].Shrooms;
        ObjectList.MothList = Level.Caves[Index].Moths;
        ObjectList.SpiderList = Level.Caves[Index].Spiders;
        ObjectList.WebList = Level.Caves[Index].Webs;
        ObjectList.TriggerList = Level.Caves[Index].Triggers;
        return ObjectList;
    }

    public void SetVelocity(float Speed)
    {
        if (AtCaveEnd())
        {
            Caves.SetVelocity(0);
            UpdateObjectSpeed(0);
        }
        else
        {
            Caves.SetVelocity(Speed);
            UpdateObjectSpeed(Speed);
        }
    }

    public void SetPaused(bool PauseGame)
    {
        // Caves?
        Shrooms.SetPaused(PauseGame);
        Stals.SetPaused(PauseGame);
        Moths.SetPaused(PauseGame);
        Spiders.SetPaused(PauseGame);
        Webs.SetPaused(PauseGame);
    }

    private void UpdateObjectSpeed(float Speed)
    {
        Shrooms.SetVelocity(Speed);
        Stals.SetVelocity(Speed);
        Moths.SetVelocity(Speed);
        Spiders.SetVelocity(Speed);
        Webs.SetVelocity(Speed);
        Triggers.SetVelocity(Speed);
    }

    private void LoadLevel()
    {
        if (bEndlessMode) { return; }
        TextAsset LevelTxt = (TextAsset)Resources.Load("LevelXML/Level" + Toolbox.Instance.Level);
        Level = LevelContainer.LoadFromText(LevelTxt.text);
    }

    private void SetupStartingCaves()
    {
        Caves.PlaceCaveEntrance();

        if (bEndlessMode)
        {
            Caves.PlaceCaveEntrance();
            Caves.SetNextCavePiece(EndlessCave.GetRandomTopType(), EndlessCave.GetRandomBottomType(), false, false);
        }
        else
        {
            if (Level.Caves[0].TopIndex == 1000)
            {
                CaveIndex++;
                Caves.SetNextCavePiece(Level.Caves[1].TopIndex, Level.Caves[1].BottomIndex, Level.Caves[1].bTopSecretPath, Level.Caves[1].bBottomSecretPath);
                SetCaveObstacles(1);
            }
            else
            {
                Caves.SetNextCavePiece(Level.Caves[0].TopIndex, Level.Caves[0].BottomIndex, Level.Caves[0].bTopSecretPath, Level.Caves[0].bBottomSecretPath);
            }
            SetCaveObstacles(0);
        }
    }

    public void DestroyOnScreenHazards()
    {
        Stals.CheckAndDestroy();
        Shrooms.CheckAndDestroy();
        Spiders.CheckAndDestroy();
    }
    
    public void SetMode(bool bIsEndless)
    {
        bEndlessMode = bIsEndless;
    }
}
