using UnityEngine;
using System.Collections.Generic;

public class CavePool : MonoBehaviour {

    private const int NumPiecesOnScreen = 3;

    private const int NumCaves = 2;
    public const int NumTopCaveTypes = 5;
    public const int NumBottomCaveTypes = 5;
    public const int NumTopCaveExits = 1;
    public const int NumBottomCaveExits = 1;

    private int CaveIndexTopLeft;
    private int CaveIndexTopMid;
    private int CaveIndexTopRight;
    private int CaveIndexBottomLeft;
    private int CaveIndexBottomMid;
    private int CaveIndexBottomRight;

    private float CaveZPos;
    private Vector2 CaveVelocity = new Vector2(0f, 0f);

    //private LevelObjectHandler ObjectHandler;
    private CaveHandler CaveHandler;

    private struct CaveType
    {
        public bool bIsActive;
        public bool bHasSecretPath;
        public Rigidbody2D CaveBody;
    }
    private CaveType CaveEntrance;
    private CaveType CaveExit;
    private List<CaveType> TopPool = new List<CaveType>();
    private List<CaveType> BottomPool = new List<CaveType>();

    private Transform CaveParent = null;

    void Awake()
    {
        CaveZPos = Toolbox.Instance.ZLayers["Cave"];
        CaveParent = GameObject.Find("Caves").GetComponent<Transform>();
        //ObjectHandler = FindObjectOfType<LevelObjectHandler>();
        CaveHandler = FindObjectOfType<CaveHandler>();
        SetupCaveEnds();
        SetupCavePool();
	}

    void Update()
    {
        if (CaveHandler.State == CaveHandler.CaveStates.End && CaveExit.CaveBody.position.x <= 0f)
        {
            CaveHandler.State = CaveHandler.CaveStates.Final;
            CaveExit.CaveBody.position = Vector2.zero;
            CaveExit.CaveBody.velocity = Vector2.zero;
        }
    }

    public bool AtCaveEnd()
    {
        bool AtEnd = false;
        if (CaveHandler.State == CaveHandler.CaveStates.Final)
        {
            AtEnd = true;
        }
        return AtEnd;
    }

    public float GetPositionX()
    {
        float PosX;
        if (CaveHandler.State == CaveHandler.CaveStates.End || CaveHandler.State == CaveHandler.CaveStates.Final)
        {
            PosX = CaveExit.CaveBody.position.x;
        }
        else
        {
            PosX = TopPool[CaveIndexTopRight].CaveBody.position.x;
        }
        return PosX;
    }
    
    private void SetupCaveEnds()
    {
        GameObject NewPiece = (GameObject)Instantiate(Resources.Load("Caves/CaveEntrance"), Toolbox.Instance.HoldingArea, new Quaternion(), CaveParent);
        CaveEntrance.CaveBody = NewPiece.GetComponent<Rigidbody2D>();
        CaveEntrance.bHasSecretPath = false;
        CaveEntrance.bIsActive = false;

        NewPiece = (GameObject)Instantiate(Resources.Load("Caves/CaveExit"), Toolbox.Instance.HoldingArea, new Quaternion(), CaveParent);
        CaveExit.CaveBody = NewPiece.GetComponent<Rigidbody2D>();
        CaveExit.bHasSecretPath = false;
        CaveExit.bIsActive = false;
    }

    private void SetupCavePool()
    {
        for (int CaveTypeNum = 1; CaveTypeNum <= NumTopCaveTypes + NumTopCaveExits; CaveTypeNum++)
        {
            for (int i = 0; i < NumCaves; i++)
            {
                int CaveIndex = (CaveTypeNum > NumTopCaveTypes ? (CaveTypeNum - NumTopCaveTypes) : CaveTypeNum);
                string CavePathStr = "Caves/CaveTop" + (CaveTypeNum > NumTopCaveTypes ? "Exit" : "") + CaveIndex.ToString();
                GameObject Cave = (GameObject)Instantiate(Resources.Load(CavePathStr), Toolbox.Instance.HoldingArea, new Quaternion(), CaveParent);
                Cave.name = "CaveTop" + CaveTypeNum.ToString() + "_" + i.ToString();
                TopPool.Add(GetCaveAttributes(Cave));
            }
        }
        for (int CaveTypeNum = 1; CaveTypeNum <= NumBottomCaveTypes + NumBottomCaveExits; CaveTypeNum++)
        {
            for (int i = 0; i < NumCaves; i++)
            {
                int CaveIndex = (CaveTypeNum > NumBottomCaveTypes ? (CaveTypeNum - NumBottomCaveTypes) : CaveTypeNum);
                string CavePathStr = "Caves/CaveBottom" + (CaveTypeNum > NumBottomCaveTypes ? "Exit" : "") + CaveIndex.ToString();
                GameObject Cave = (GameObject)Instantiate(Resources.Load(CavePathStr), Toolbox.Instance.HoldingArea, new Quaternion(), CaveParent);
                Cave.name = "CaveBottom" + CaveTypeNum.ToString() + "_" + i.ToString();
                BottomPool.Add(GetCaveAttributes(Cave));
            }
        }
    }

    private CaveType GetCaveAttributes(GameObject Cave)
    {
        CaveType NewCave = new CaveType();
        NewCave.bIsActive = false;
        NewCave.CaveBody = Cave.GetComponent<Rigidbody2D>();
        return NewCave;
    }
    
    public void SetNextCavePiece(int NextTopType, int NextBottomType, bool bTopSecret, bool bBottomSecret)
    {
        if (NextTopType == -1 || NextTopType == 1001)   // TODO magic number. Bad.
        {
            CaveHandler.State = CaveHandler.CaveStates.End;
            PlaceCaveExit();
            return;
        }

        int NextTopIndex = NumCaves * (NextTopType + (bTopSecret ? NumTopCaveTypes : 0));
        int NextBottomIndex = NumCaves * (NextBottomType + (bBottomSecret ? NumBottomCaveTypes : 0));
        if (NextTopIndex == CaveIndexTopRight) { NextTopIndex++; }
        if (NextBottomIndex == CaveIndexBottomRight) { NextBottomIndex++; }

        DeactivateFirstPieces();

        // (*) << [Left] << [Mid] << [Right] << (Next)
        CaveIndexTopLeft = CaveIndexTopMid;
        CaveIndexBottomLeft = CaveIndexBottomMid;
        CaveIndexTopMid = CaveIndexTopRight;
        CaveIndexBottomMid = CaveIndexTopRight;
        CaveIndexTopRight = NextTopIndex;
        CaveIndexBottomRight = NextBottomIndex;

        ActivateCavePiece(CaveIndexTopRight, bTop: true);
        ActivateCavePiece(CaveIndexBottomRight, bTop: false);
    }

    private void DeactivateFirstPieces()
    {
        if (CaveHandler.State == CaveHandler.CaveStates.Start)
        {
            CaveHandler.State = CaveHandler.CaveStates.Middle;
        }
        else if (CaveHandler.State == CaveHandler.CaveStates.Middle)
        {
            if (CaveEntrance.bIsActive)
            {
                CaveEntrance.bIsActive = false;
                CaveEntrance.CaveBody.velocity = Vector2.zero;
                CaveEntrance.CaveBody.position = Toolbox.Instance.HoldingArea;
            }
            else
            {
                DeactivateCavePiece(CaveIndexTopLeft, bTop: true);
                DeactivateCavePiece(CaveIndexBottomLeft, bTop: false);
            }
        }
    }

    private void ActivateCavePiece(int PieceIndex, bool bTop)
    {
        float TopCavePieceX = TopPool[CaveIndexTopMid].CaveBody.position.x;
        float BottomCavePieceX = BottomPool[CaveIndexBottomMid].CaveBody.position.x;
        if (CaveEntrance.bIsActive)
        {
            TopCavePieceX = CaveEntrance.CaveBody.position.x;
            BottomCavePieceX = TopCavePieceX;
        }
        float CavePieceY = 0f;
        float CavePieceZ = CaveZPos;

        CaveType NextTopCave = TopPool[CaveIndexTopRight];
        CaveType NextBottomCave = BottomPool[CaveIndexBottomRight];
        NextTopCave.bIsActive = true;
        NextTopCave.CaveBody.position = new Vector3(Toolbox.TileSizeX + TopCavePieceX, CavePieceY, CavePieceZ);
        NextTopCave.CaveBody.velocity = CaveVelocity;
        NextBottomCave.bIsActive = true;
        NextBottomCave.CaveBody.position = new Vector3(Toolbox.TileSizeX + BottomCavePieceX, CavePieceY, CavePieceZ);
        NextBottomCave.CaveBody.velocity = CaveVelocity;
        TopPool[CaveIndexTopRight] = NextTopCave;
        BottomPool[CaveIndexBottomRight] = NextBottomCave;
    }

    private void DeactivateCavePiece(int PieceIndex, bool bTop)
    {
        CaveType Cave;
        if (bTop) {
            Cave = TopPool[PieceIndex];
        } else {
            Cave = BottomPool[PieceIndex];
        }
        Cave.bIsActive = false;
        Cave.CaveBody.velocity = Vector2.zero;
        Cave.CaveBody.position = Toolbox.Instance.HoldingArea;

        if (bTop) {
            TopPool[PieceIndex] = Cave;
        } else {
            BottomPool[PieceIndex] = Cave;
        }
    }
    
    public void SetVelocity(float Speed)
    {
        if (CaveHandler.State != CaveHandler.CaveStates.Final)
        {
            CaveVelocity = new Vector2(-Speed, 0);
            SetActiveCaveVelocity();
        }
    }

    private void SetActiveCaveVelocity()
    {
        foreach (CaveType Cave in TopPool)
        {
            if (Cave.bIsActive) { Cave.CaveBody.velocity = CaveVelocity; }
        }
        foreach (CaveType Cave in BottomPool)
        {
            if (Cave.bIsActive) { Cave.CaveBody.velocity = CaveVelocity; }
        }

        if (CaveEntrance.bIsActive) { CaveEntrance.CaveBody.velocity = CaveVelocity; }
        if (CaveExit.bIsActive) { CaveExit.CaveBody.velocity = CaveVelocity; }
    }

    public void PlaceCaveEntrance()
    {
        CaveEntrance.bIsActive = true;
        CaveEntrance.CaveBody.position = new Vector3(0f, 0f, 0f);
        CaveEntrance.CaveBody.velocity = CaveVelocity;
    }

    public void PlaceCaveExit()
    {
        float Xoffset = BottomPool[CaveIndexBottomRight].CaveBody.position.x;
        CaveExit.bIsActive = true;
        CaveExit.CaveBody.position = new Vector3(Xoffset + Toolbox.TileSizeX, 0f, 0f);
        CaveExit.CaveBody.velocity = CaveVelocity;
    }
}
