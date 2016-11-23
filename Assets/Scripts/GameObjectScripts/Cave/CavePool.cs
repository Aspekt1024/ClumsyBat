using UnityEngine;
using System.Collections.Generic;

public class CavePool : MonoBehaviour {
    
    private const int NumCaves = 2;
    public const int NumTopCaveTypes = 4;
    public const int NumBottomCaveTypes = 4;

    private int CaveIndexTopFirst;
    private int CaveIndexTopSecond;
    private int CaveIndexBottomFirst;
    private int CaveIndexBottomSecond;

    private const float CaveZPos = 0f;
    private Vector2 CaveVelocity = new Vector2(0f, 0f);

    private List<CaveType> TopPool = new List<CaveType>();
    private List<CaveType> BottomPool = new List<CaveType>();

    private enum CaveStates
    {
        Start,
        Middle,
        End,
        Final
    }

    private CaveStates CaveState;

    private struct CaveType
    {
        public bool bIsActive;
        public Rigidbody2D CaveBody;
    }
    private CaveType CaveEntrance;
    private CaveType CaveExit;
    
    void Awake()
    {
        SetupCaveEnds();
        SetupCavePool();
	}

    void Update()
    {
        if (CaveState == CaveStates.End)
        {
            if (CaveExit.CaveBody.position.x <= 0)
            {
                CaveState = CaveStates.Final;
                CaveExit.CaveBody.position = Vector2.zero;
                CaveExit.CaveBody.velocity = Vector2.zero;
            }
        }
    }

    public bool AtCaveEnd()
    {
        bool AtEnd = false;
        if (CaveState == CaveStates.Final)
        {
            AtEnd = true;
        }
        return AtEnd;
    }

    public float GetPositionX()
    {
        float PosX;
        if (CaveState == CaveStates.End || CaveState == CaveStates.Final)
        {
            PosX = CaveExit.CaveBody.position.x;
        }
        else
        {
            PosX = TopPool[CaveIndexTopSecond].CaveBody.position.x;
        }
        return PosX;
    }
    
    private void SetupCaveEnds()
    {
        GameObject NewPiece = (GameObject)Instantiate(Resources.Load("Caves/CaveEntrance"));
        CaveEntrance.CaveBody = NewPiece.GetComponent<Rigidbody2D>();
        CaveEntrance.CaveBody.position = Toolbox.Instance.HoldingArea;

        NewPiece = (GameObject)Instantiate(Resources.Load("Caves/CaveExit"));
        CaveExit.CaveBody = NewPiece.GetComponent<Rigidbody2D>();
        CaveExit.CaveBody.position = Toolbox.Instance.HoldingArea;

        CaveEntrance.bIsActive = false;
        CaveExit.bIsActive = false;
    }

    private void SetupCavePool()
    {
        for (int CaveTypeNum = 1; CaveTypeNum <= NumTopCaveTypes; CaveTypeNum++)
        {
            for (int i = 0; i < NumCaves; i++)
            {
                GameObject Cave = (GameObject)Instantiate(Resources.Load("Caves/CaveTop" + CaveTypeNum.ToString()));
                Cave.name = "CaveTop" + CaveTypeNum.ToString() + "_" + i.ToString();
                Cave.transform.position = new Vector3(5 * Toolbox.TileSizeX, 0f, CaveZPos);
                TopPool.Add(GetCaveAttributes(Cave));
            }
        }
        for (int CaveTypeNum = 1; CaveTypeNum <= NumBottomCaveTypes; CaveTypeNum++)
        {
            for (int i = 0; i < NumCaves; i++)
            {
                GameObject Cave = (GameObject)Instantiate(Resources.Load("Caves/CaveBottom" + CaveTypeNum.ToString()));
                Cave.name = "CaveBottom" + CaveTypeNum.ToString() + "_" + i.ToString();
                Cave.transform.position = new Vector3(5 * Toolbox.TileSizeX, 0f, CaveZPos);
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

    public void SetNextCavePiece(int NextTopType, int NextBottomType)
    {
        if (NextTopType == -1)
        {
            CaveState = CaveStates.End;
            PlaceCaveExit();
            return;
        }

        int NextTopIndex = NumCaves * NextTopType;
        int NextBottomIndex = NumCaves * NextBottomType;
        if (NextTopIndex == CaveIndexTopSecond) { NextTopIndex++; }
        if (NextBottomIndex == CaveIndexBottomSecond) { NextBottomIndex++; }

        DeactivateFirstPieces();

        CaveIndexTopFirst = CaveIndexTopSecond;
        CaveIndexBottomFirst = CaveIndexBottomSecond;
        CaveIndexTopSecond = NextTopIndex;
        CaveIndexBottomSecond = NextBottomIndex;

        ActivateCavePiece(CaveIndexTopSecond, bTop: true);
        ActivateCavePiece(CaveIndexBottomSecond, bTop: false);
    }

    private void DeactivateFirstPieces()
    {
        if (CaveState == CaveStates.Start)
        {
            CaveState = CaveStates.Middle;
        }
        else if (CaveState == CaveStates.Middle)
        {
            if (CaveEntrance.bIsActive)
            {
                CaveEntrance.bIsActive = false;
                CaveEntrance.CaveBody.velocity = Vector2.zero;
                CaveEntrance.CaveBody.position = Toolbox.Instance.HoldingArea;
            }
            else
            {
                DeactivateCavePiece(CaveIndexTopFirst, bTop: true);
                DeactivateCavePiece(CaveIndexBottomFirst, bTop: false);
            }
        }
    }

    private void ActivateCavePiece(int PieceIndex, bool bTop)
    {
        float TopCavePieceX = TopPool[CaveIndexTopFirst].CaveBody.position.x;
        float BottomCavePieceX = BottomPool[CaveIndexBottomFirst].CaveBody.position.x;
        if (CaveEntrance.bIsActive)
        {
            TopCavePieceX = CaveEntrance.CaveBody.position.x;
            BottomCavePieceX = TopCavePieceX;
        }
        float CavePieceY = 0f;
        float CavePieceZ = CaveZPos;

        CaveType NextTopCave = TopPool[CaveIndexTopSecond];
        CaveType NextBottomCave = BottomPool[CaveIndexBottomSecond];
        NextTopCave.bIsActive = true;
        NextTopCave.CaveBody.position = new Vector3(Toolbox.TileSizeX + TopCavePieceX, CavePieceY, CavePieceZ);
        NextTopCave.CaveBody.velocity = CaveVelocity;
        NextBottomCave.bIsActive = true;
        NextBottomCave.CaveBody.position = new Vector3(Toolbox.TileSizeX + BottomCavePieceX, CavePieceY, CavePieceZ);
        NextBottomCave.CaveBody.velocity = CaveVelocity;
        TopPool[CaveIndexTopSecond] = NextTopCave;
        BottomPool[CaveIndexBottomSecond] = NextBottomCave;
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
        if (CaveState != CaveStates.Final)
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
        float Xoffset = BottomPool[CaveIndexBottomSecond].CaveBody.position.x;
        CaveExit.bIsActive = true;
        CaveExit.CaveBody.position = new Vector3(Xoffset + Toolbox.TileSizeX, 0f, 0f);
        CaveExit.CaveBody.velocity = CaveVelocity;

    }
}
