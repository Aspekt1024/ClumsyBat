using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the generation, positioning and movement of all cave pieces
/// </summary>
public class Cave : MonoBehaviour {

    // This handles the Cave object pools
    // TODO rename this 'Level' because it's no longer taking care of just caves?
    // We're going to create (or have created? how long has this comment been here?) a random level template to
    // store all possibilities of randomising the caves. 
    private struct CaveType
    {
        public bool bIsActive;
        public Rigidbody2D CaveBody;
        public Mushroom[] Shrooms;
        public Stalactite[] Stals;
        // TODO moths
    }
    
    // TODO replace with separate object pools
    // That way, Moths are managed by the MothPool class, Mushrooms by the MushroomPool class, etc
    // Also, this will clear up this Cave class, because it's managing too much right now.
    private struct CaveListType
    {
        public StalPool.StalType[] StalList;
        public ShroomPool.ShroomType[] MushroomList;
        public MothPool.MothType[] MothList;
    }
    
    private LevelContainer Level;
    
    private const float CaveZPos = 0f;
    private const float TileSize = 19.2f;
    private List<CaveType> TopCavePool = new List<CaveType>();
    private List<CaveType> BottomCavePool = new List<CaveType>();

    private struct CaveEndType
    {
        public GameObject Front;
        public GameObject Back;
        public PolygonCollider2D Coll;
        public Rigidbody2D FrontBody;
        public Rigidbody2D BackBody;
        public bool isActive;
    }
    private CaveEndType CaveEntrance;
    private CaveEndType CaveExit;

    private const int NumCaves = 2;
    private const int NumTopCaveTypes = 4;
    private const int NumBottomCaveTypes = 4;
    private int CaveIndexTopFirst;
    private int CaveIndexTopSecond;
    private int CaveIndexBottomFirst;
    private int CaveIndexBottomSecond;

    private Vector2 CaveVelocity = new Vector2(0, 0);

    private bool bEndlessMode = false;
    private int CaveIndex = 0;

    private ShroomPool Shrooms;
    private MothPool Moths;
    private StalPool Stals;

    void Start ()
    {
        SetupObjectPools();
        LoadLevel();
        SetupStartingCaves();
    }

    private void SetupObjectPools()
    {
        SetupCaveEnds();
        SetupCavePool();
        Shrooms = new ShroomPool();
        Stals = new StalPool();
        Moths = new MothPool();
    }

    void Update()
    {
        // Note: assumed that top and bottom are the same width and are at the same X position.
        // This can change, but doesn't have to...
        if (TopCavePool[CaveIndexTopSecond].CaveBody.transform.position.x <= 0)
        {
            SetNextCavePiece();
        }
    }

    private void MoveCaveEnds()
    {
        if (CaveEntrance.isActive)
        {
            CaveEntrance.Front.transform.position += new Vector3(Time.deltaTime * CaveVelocity.x, 0f, 0f);
            CaveEntrance.Back.transform.position += new Vector3(Time.deltaTime * CaveVelocity.x, 0f, 0f);
            if (CaveEntrance.Front.transform.position.x < -TileSize)
            {
                CaveEntrance.isActive = false;
                CaveEntrance.Front.transform.position = Toolbox.Instance.HoldingArea;
                CaveEntrance.Back.transform.position = Toolbox.Instance.HoldingArea;
            }
        }
        if (CaveExit.isActive)
        {
            CaveExit.Front.transform.position += new Vector3(Time.deltaTime * CaveVelocity.x, 0f, 0f);
            CaveExit.Back.transform.position += new Vector3(Time.deltaTime * CaveVelocity.x, 0f, 0f);
            if (CaveExit.Front.transform.position.x < -TileSize)
            {
                CaveExit.isActive = false;
                CaveExit.Front.transform.position = Toolbox.Instance.HoldingArea;
                CaveExit.Back.transform.position = Toolbox.Instance.HoldingArea;
            }
        }
    }

    private void SetNextCavePiece()
    {
        CaveIndex++;
        if (CaveIndex < 1) { return; }  // TODO this will never return, i think. Check.

        int NextTopCaveIndex = GetNextTopCaveIndex();
        int NextBottomCaveIndex = GetNextBottomCaveIndex();

        if (NextTopCaveIndex < 0)
        {
            PositionCaveExit(TopCavePool[CaveIndexTopSecond].CaveBody.transform.position.x);
            return;
        }

        DeactivateCavePiece(CaveIndexTopFirst, bTop:true);
        DeactivateCavePiece(CaveIndexBottomFirst, bTop:false);

        CaveIndexTopFirst = CaveIndexTopSecond;
        CaveIndexBottomFirst = CaveIndexBottomSecond;

        CaveIndexTopSecond = NextTopCaveIndex;
        CaveIndexBottomSecond = NextBottomCaveIndex;

        ActivateTopCavePiece(CaveIndexTopSecond);
        ActivateBottomCavePiece(CaveIndexBottomSecond);
        
        float TopCavePieceX = TopCavePool[CaveIndexTopFirst].CaveBody.transform.position.x;
        float BottomCavePieceX = BottomCavePool[CaveIndexBottomFirst].CaveBody.transform.position.x;
        float CavePieceY = 0f;
        float CavePieceZ = TopCavePool[CaveIndexTopFirst].CaveBody.transform.position.z;
        TopCavePool[CaveIndexTopSecond].CaveBody.transform.position = new Vector3(TileSize + TopCavePieceX, CavePieceY, CavePieceZ);
        BottomCavePool[CaveIndexBottomSecond].CaveBody.transform.position = new Vector3(TileSize + BottomCavePieceX, CavePieceY, CavePieceZ);
        
        SetCaveObstacles(CaveIndex);
    }

    private int GetNextTopCaveIndex()
    {
        int NextTopCaveType;
        if (bEndlessMode)
        {
            NextTopCaveType = Random.Range(0, NumTopCaveTypes);
        }
        else
        {
            if (CaveIndex >= Level.Caves.Length)
            {
                NextTopCaveType = -1;
            }
            else
            {
                NextTopCaveType = Level.Caves[CaveIndex].TopIndex;
            }
        }

        int NextTopCaveIndex = NumCaves * NextTopCaveType;
        if (NextTopCaveIndex == CaveIndexTopSecond && NextTopCaveType >= 0)
        {
            NextTopCaveIndex++;
        }
        return NextTopCaveIndex;
    }

    private int GetNextBottomCaveIndex()
    {
        int NextBottomCaveType;
        if (bEndlessMode)
        {
            NextBottomCaveType = Random.Range(0, NumBottomCaveTypes);
        }
        else
        {
            if (CaveIndex >= Level.Caves.Length)
            {
                NextBottomCaveType = -1;
            }
            else
            {
                NextBottomCaveType = Level.Caves[CaveIndex].BottomIndex;
            }
        }

        int NextBottomCaveIndex = NumCaves * NextBottomCaveType;
        if (NextBottomCaveIndex == CaveIndexBottomSecond)
        {
            NextBottomCaveIndex++;
        }
        return NextBottomCaveIndex;
    }

    private void SetCaveObstacles(int Index)
    {
        float XOffset = TileSize;
        if (Index == 0)
        {
            XOffset = 0f;
        }
        CaveListType ObjectList;
        if (bEndlessMode)
        {
            ObjectList = RandomiseObstacleList();
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

    private CaveListType RandomiseObstacleList()
    {
        StalPool.StalType[] StalList = new StalPool.StalType[0];
        ShroomPool.ShroomType[] ShroomList = new ShroomPool.ShroomType[0];
        MothPool.MothType[] MothList = new MothPool.MothType[0];

        const int NumStals = 8;
        int LowerIndex = 0;
        int UpperIndex = 3;

        bool bTop = (Random.Range(0, 2) == 1);

        int InARow = 1;

        int i = 0;
        i = Random.Range(LowerIndex, UpperIndex);
        while ((i < NumStals) && (i < NumStals))
        {
            if (!bTop && (i / 2 - Mathf.Floor(i / 2) < 0.4) && Random.Range(0f, 1f) <= 0.47f)
            {
                ShroomList[0].Pos = new Vector2(0, 0);
                ShroomList[0].Rotation = new Quaternion();
                ShroomList[0].Scale = new Vector2(1f, 1f);
                ShroomList[0].SpecialEnabled = false;
            }
            else
            {
                StalList[0].Pos = new Vector2(0, 0);
                StalList[0].Rotation = new Quaternion();
                StalList[0].Scale = new Vector2(1f, 1f);
                StalList[0].DropEnabled = true;
            }

            int t = Random.Range(0, 2);
            if ((t == 1 && bTop) || t == 0 && !bTop)
            {
                if (InARow == 2)
                {
                    InARow = 1;
                    LowerIndex = i + 3;
                    UpperIndex = i + 5;
                }
                else
                {
                    LowerIndex = i + 1;
                    UpperIndex = i + 2;
                    InARow++;
                }
            }
            else
            {
                bTop = !bTop;
                LowerIndex = i + 3;
                UpperIndex = i + 5;
                InARow = 1;
            }
            i = Random.Range(LowerIndex, UpperIndex);
        }
        
        CaveListType ObjectList;
        ObjectList.StalList = StalList;
        ObjectList.MushroomList = ShroomList;
        ObjectList.MothList = MothList;

        return ObjectList;
    }

    private void ActivateMushroom(CaveType Cave, int i)
    {
        Cave.Shrooms[i].ActivateMushroom();
    }

    private static void ActivateStals(CaveType Cave, int i)
    {
        if (Random.Range(0f, 1f) > 0.5f)
        {
            Cave.Stals[i].ActivateStal(true);
        }
        else
        {
            Cave.Stals[i].ActivateStal(false);
        }
    }

    private void SetupCaveEnds()
    {
        CaveEntrance.Front = (GameObject)Instantiate(Resources.Load("Caves/EntranceFront"));
        CaveEntrance.Back = (GameObject)Instantiate(Resources.Load("Caves/EntranceBack"));
        CaveEntrance.Coll = CaveEntrance.Back.GetComponent<PolygonCollider2D>();
        CaveEntrance.BackBody = CaveEntrance.Back.GetComponent<Rigidbody2D>();
        CaveEntrance.FrontBody = CaveEntrance.Front.GetComponent<Rigidbody2D>();

        CaveExit.Front = (GameObject)Instantiate(Resources.Load("Caves/ExitFront"));
        CaveExit.Back = (GameObject)Instantiate(Resources.Load("Caves/ExitBack"));
        CaveExit.Coll = CaveExit.Back.GetComponent<PolygonCollider2D>();
        CaveExit.BackBody = CaveExit.Back.GetComponent<Rigidbody2D>();
        CaveExit.FrontBody = CaveExit.Front.GetComponent<Rigidbody2D>();

        CaveEntrance.Front.transform.position = Toolbox.Instance.HoldingArea;
        CaveEntrance.Back.transform.position = Toolbox.Instance.HoldingArea;
        CaveExit.Front.transform.position = Toolbox.Instance.HoldingArea;
        CaveExit.Back.transform.position = Toolbox.Instance.HoldingArea;

        CaveEntrance.isActive = false;
        CaveExit.isActive = false;
    }

    private void SetupCavePool()
    {
        for (int CaveTypeNum = 1; CaveTypeNum <= NumTopCaveTypes; CaveTypeNum++)
        {
            for (int i = 0; i < NumCaves; i++)
            {
                GameObject Cave = (GameObject)Instantiate(Resources.Load("Caves/CaveTop" + CaveTypeNum.ToString()));
                Cave.name = "CaveTop" + CaveTypeNum.ToString() + "_" + i.ToString();
                Cave.transform.position = new Vector3(5*TileSize, 0f, CaveZPos);
                TopCavePool.Add(GetCaveAttributes(Cave));
            }
        }
        for (int CaveTypeNum = 1; CaveTypeNum <= NumBottomCaveTypes; CaveTypeNum++)
        {
            for (int i = 0; i < NumCaves; i++)
            {
                GameObject Cave = (GameObject)Instantiate(Resources.Load("Caves/CaveBottom" + CaveTypeNum.ToString()));
                Cave.name = "CaveBottom" + CaveTypeNum.ToString() + "_" + i.ToString();
                Cave.transform.position = new Vector3(5*TileSize, 0f, CaveZPos);
                BottomCavePool.Add(GetCaveAttributes(Cave));
            }
        }
    }

    public void SetVelocity(float Speed)
    {
        CaveVelocity = new Vector2(-Speed, 0);
        SetActiveCaveVelocity();
    }
    
    private void SetActiveCaveVelocity()
    { 
        foreach (CaveType Cave in TopCavePool)
        {
            if (Cave.bIsActive)
            {
                Cave.CaveBody.velocity = CaveVelocity;
            }
        }
        foreach (CaveType Cave in BottomCavePool)
        {
            if (Cave.bIsActive)
            {
                Cave.CaveBody.velocity = CaveVelocity;
            }
        }
        if (CaveEntrance.isActive)
        {
            CaveEntrance.FrontBody.velocity = CaveVelocity;
            CaveEntrance.BackBody.velocity = CaveVelocity;
        }
        if (CaveExit.isActive)
        {
            CaveExit.FrontBody.velocity = CaveVelocity;
            CaveExit.BackBody.velocity = CaveVelocity;
        }
        UpdateObjectSpeed(-CaveVelocity.x);
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
        PlaceCaveEntrance();
        if (bEndlessMode)
        {
            CaveIndexTopFirst = NumCaves * Random.Range(0, NumTopCaveTypes);
            CaveIndexBottomFirst = NumCaves * Random.Range(0, NumTopCaveTypes);
            CaveIndexTopSecond = NumCaves * Random.Range(0, NumTopCaveTypes);
            CaveIndexBottomSecond = NumCaves * Random.Range(0, NumTopCaveTypes);
        }
        else
        {
            //CaveIndexTopFirst = Level.Caves[0].TopIndex * 2;
            CaveIndexTopSecond = Level.Caves[0].TopIndex * 2;
            //CaveIndexBottomFirst = Level.Caves[0].BottomIndex * 2;
            CaveIndexBottomSecond = Level.Caves[0].BottomIndex * 2;
            SetCaveObstacles(0);
            //SetCaveObstacles(1);
        }

        //if (CaveIndexTopFirst == CaveIndexTopSecond) { CaveIndexTopSecond++; }
        if (CaveIndexBottomFirst == CaveIndexBottomSecond) { CaveIndexBottomSecond++; }

        PositionStartingCaves();
    }

    private void PlaceCaveEntrance()
    {
        CaveEntrance.isActive = true;
        CaveEntrance.Front.transform.position = new Vector3(0f, 0f, -3f);
        CaveEntrance.Back.transform.position = new Vector3(0f, 0f, 0f);
    }

    private void PositionCaveExit(float Xoffset)
    {
        CaveExit.isActive = true;
        CaveExit.Front.transform.position = new Vector3(Xoffset + TileSize, 0f, -3f);
        CaveExit.Back.transform.position = new Vector3(Xoffset + TileSize, 0f, 0f);
    }

    private void PositionStartingCaves()
    {
        //TopCavePool[CaveIndexTopFirst].CaveBody.transform.position = new Vector3(0f, 0f, TopCavePool[CaveIndexTopFirst].CaveBody.transform.position.z);
        TopCavePool[CaveIndexTopSecond].CaveBody.transform.position = new Vector3(TileSize, 0f, TopCavePool[CaveIndexTopSecond].CaveBody.transform.position.z);
        //BottomCavePool[CaveIndexBottomFirst].CaveBody.transform.position = new Vector3(0f, 0f, BottomCavePool[CaveIndexBottomFirst].CaveBody.transform.position.z);
        BottomCavePool[CaveIndexBottomSecond].CaveBody.transform.position = new Vector3(TileSize, 0f, BottomCavePool[CaveIndexBottomSecond].CaveBody.transform.position.z);

        //ActivateTopCavePiece(CaveIndexTopFirst);
        ActivateTopCavePiece(CaveIndexTopSecond);
        //ActivateBottomCavePiece(CaveIndexBottomFirst);
        ActivateBottomCavePiece(CaveIndexBottomSecond);
    }

    private void ActivateTopCavePiece(int index)
    {
        CaveType TheCave = TopCavePool[index];
        TheCave.bIsActive = true;
        TheCave.CaveBody.velocity = CaveVelocity;
        TopCavePool[index] = TheCave;
    }

    private void ActivateBottomCavePiece(int index)
    {
        CaveType TheCave = BottomCavePool[index];
        TheCave.bIsActive = true;
        TheCave.CaveBody.velocity = CaveVelocity;
        BottomCavePool[index] = TheCave;
    }

    private void DeactivateCavePiece(int index, bool bTop)
    {
        CaveType TheCave;
        if (bTop) {
            TheCave = TopCavePool[index];
        } else {
            TheCave = BottomCavePool[index];
        }

        TheCave.bIsActive = false;
        TheCave.CaveBody.velocity = new Vector2(0f, 0f);
        TheCave.CaveBody.transform.position = new Vector3(5*TileSize, 0f, TheCave.CaveBody.transform.position.z);
        
        foreach (Mushroom Shroom in TheCave.Shrooms)
        {
            Shroom.DeactivateMushroom();
        }
        foreach (Stalactite Stal in TheCave.Stals)
        {
            Stal.DeactivateStal();
        }
        foreach (Stalactite Stal in TheCave.Stals)
        {
            Stal.DeactivateStal();
        }

        if (bTop) {
            TopCavePool[index] = TheCave;
        } else {
            BottomCavePool[index] = TheCave;
        }
    }

    private CaveType GetCaveAttributes(GameObject Cave)
    {
        CaveType NewCave;
        NewCave.CaveBody = Cave.GetComponent<Rigidbody2D>();
        NewCave.bIsActive = false;
        
        Mushroom[] Shrooms = new Mushroom[0];
        Stalactite[] Stals = new Stalactite[0];
        foreach (Transform ChildElem in Cave.transform)
        {
            if (ChildElem.name == "Stalactites")
            {
                Stals = GetStalList(ChildElem);
            }
            else if (ChildElem.name == "Mushrooms")
            {
                Shrooms = GetShroomList(ChildElem);
            }
        }
        NewCave.Shrooms = Shrooms;
        NewCave.Stals = Stals;
        
        return NewCave;
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
