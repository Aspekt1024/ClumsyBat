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
    }
    
    private LevelContainer Level;

    private CaveRandomiser EndlessCave;

    private bool bEndlessMode = false;
    private int CaveIndex = 0;

    private CavePool Caves;
    private ShroomPool Shrooms;
    private MothPool Moths;
    private StalPool Stals;

    void Start ()
    {   
        SetupObjectPools();
        LoadLevel();
        EndlessCave = new CaveRandomiser();
        SetupStartingCaves();
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
    }

    void Update()
    {
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
        Caves.SetNextCavePiece(NextTopCaveType, NextBottomCaveType);
        if (CaveIndex < Level.Caves.Length)
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
    }

    private CaveListType GetCaveObjectList(int Index)
    {
        CaveListType ObjectList;
        ObjectList.StalList = Level.Caves[Index].Stals;
        ObjectList.MushroomList = Level.Caves[Index].Shrooms;
        ObjectList.MothList = Level.Caves[Index].Moths;
        return ObjectList;
    }

    public void SetVelocity(float Speed)
    {
        Caves.SetVelocity(Speed);
        UpdateObjectSpeed(Speed);
    }

    private void LoadLevel()
    {
        if (bEndlessMode) { return; }
        Debug.Log("loading Level: " + Toolbox.Instance.Level);
        TextAsset LevelTxt = (TextAsset)Resources.Load("LevelXML/Level" + Toolbox.Instance.Level);
        Level = LevelContainer.LoadFromText(LevelTxt.text);
    }

    private void SetupStartingCaves()
    {
        Caves.PlaceCaveEntrance();

        if (bEndlessMode)
        {
            Caves.SetNextCavePiece(EndlessCave.GetRandomTopType(), EndlessCave.GetRandomBottomType());
        }
        else
        {
            Caves.SetNextCavePiece(Level.Caves[0].TopIndex, Level.Caves[0].BottomIndex);
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

    private void UpdateObjectSpeed(float Speed)
    {
        Shrooms.SetVelocity(Speed);
        Stals.SetVelocity(Speed);
        Moths.SetVelocity(Speed);
    }

    public void DestroyOnScreenHazards()
    {
        Stals.CheckAndDestroy();
        Shrooms.CheckAndDestroy();
    }
    
    public void SetMode(bool bIsEndless)
    {
        bEndlessMode = bIsEndless;
    }
}
