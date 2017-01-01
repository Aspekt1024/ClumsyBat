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
    }

    private CaveListType GetCaveObjectList(int Index)
    {
        CaveListType ObjectList;
        ObjectList.StalList = Level.Caves[Index].Stals;
        ObjectList.MushroomList = Level.Caves[Index].Shrooms;
        ObjectList.MothList = Level.Caves[Index].Moths;
        ObjectList.SpiderList = Level.Caves[Index].Spiders;
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
    }

    private void UpdateObjectSpeed(float Speed)
    {
        Shrooms.SetVelocity(Speed);
        Stals.SetVelocity(Speed);
        Moths.SetVelocity(Speed);
        Spiders.SetVelocity(Speed);
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
    
    private Mushroom[] GetShroomList(Transform Shrooms)
    {
        Mushroom[] ShroomList = new Mushroom[Shrooms.childCount];
        foreach (Transform Obj in Shrooms.transform)
        {
            Mushroom Shroom = Obj.GetComponentInChildren<Mushroom>();
            if (!Shroom)
            {
                Debug.Log("Error: No script set to Mushroom: " + Shrooms.name + "/" + Obj.name);
            }
            Shroom.DeactivateMushroom();
            ShroomList[int.Parse(Obj.name) - 1] = Shroom;
        }
        return ShroomList;
    }

    private Stalactite[] GetStalList(Transform Stals)
    {
        Stalactite[] Obstacles = new Stalactite[Stals.transform.childCount];
        foreach (Transform Obj in Stals.transform)
        {
            Stalactite Obstacle = Obj.GetComponentInChildren<Stalactite>();
            if (!Obstacle)
            {
                Debug.Log("Error: No script set to Obstacle: " + Stals.name + "/" + Obj.name);
            }
            Obstacle.DeactivateStal();
            Obstacles[int.Parse(Obj.name) - 1] = Obstacle;
        }
        return Obstacles;
    }

    private SpiderClass[] GetSpider(Transform Spiders)
    {
        SpiderClass[] SpiderList = new SpiderClass[Spiders.transform.childCount];
        foreach (Transform Obj in Spiders.transform)
        {
            SpiderClass Spider = Obj.GetComponentInChildren<SpiderClass>();
            if (!Spider)
            {
                Debug.Log("Error: No script set to " + Spider.name + "/" + Obj.name);
            }
            Spider.DeactivateSpider();
            SpiderList[int.Parse(Obj.name) - 1] = Spider;
        }
        return SpiderList;
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
