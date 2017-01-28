using UnityEngine;
using System.Collections.Generic;

public class CavePool : MonoBehaviour {

    private const int NumCaves = 2;
    public const int NumTopCaveTypes = 5;
    public const int NumBottomCaveTypes = 5;
    public const int NumTopCaveExits = 1;
    public const int NumBottomCaveExits = 1;

    private int _caveIndexTopLeft;
    private int _caveIndexTopMid;
    private int _caveIndexTopRight;
    private int _caveIndexBottomLeft;
    private int _caveIndexBottomMid;
    private int _caveIndexBottomRight;

    private float _caveZPos;
    private Vector2 _caveVelocity = new Vector2(0f, 0f);
    
    private CaveHandler _caveHandler;

    private struct CaveType
    {
        public bool bIsActive;
        public bool bHasSecretPath;
        public Rigidbody2D CaveBody;
    }
    private CaveType _caveEntrance;
    private CaveType _caveExit;
    private readonly List<CaveType> _topPool = new List<CaveType>();
    private readonly List<CaveType> _bottomPool = new List<CaveType>();

    private Transform _caveParent;

    private void Awake()
    {
        _caveZPos = Toolbox.Instance.ZLayers["Cave"];
        _caveParent = GameObject.Find("Caves").GetComponent<Transform>();
        _caveHandler = FindObjectOfType<CaveHandler>();
        SetupCaveEnds();
        SetupCavePool();
	}

    private void Update()
    {
        if (_caveHandler.State != CaveHandler.CaveStates.End || !(_caveExit.CaveBody.position.x <= 0f)) return;
        _caveHandler.State = CaveHandler.CaveStates.Final;
        _caveExit.CaveBody.position = Vector2.zero;
        _caveExit.CaveBody.velocity = Vector2.zero;
    }

    public bool AtCaveEnd() { return _caveHandler.State == CaveHandler.CaveStates.Final; }

    public float GetPositionX()
    {
        float posX;
        if (_caveHandler.State == CaveHandler.CaveStates.End || _caveHandler.State == CaveHandler.CaveStates.Final)
        {
            posX = _caveExit.CaveBody.position.x;
        }
        else
        {
            posX = _topPool[_caveIndexTopRight].CaveBody.position.x;
        }
        return posX;
    }
    
    private void SetupCaveEnds()
    {
        GameObject newPiece = (GameObject)Instantiate(Resources.Load("Caves/CaveEntrance"), Toolbox.Instance.HoldingArea, new Quaternion(), _caveParent);
        _caveEntrance.CaveBody = newPiece.GetComponent<Rigidbody2D>();
        _caveEntrance.bHasSecretPath = false;
        _caveEntrance.bIsActive = false;

        newPiece = (GameObject)Instantiate(Resources.Load("Caves/CaveExit"), Toolbox.Instance.HoldingArea, new Quaternion(), _caveParent);
        _caveExit.CaveBody = newPiece.GetComponent<Rigidbody2D>();
        _caveExit.bHasSecretPath = false;
        _caveExit.bIsActive = false;
    }

    private void SetupCavePool()
    {
        for (int caveTypeNum = 1; caveTypeNum <= NumTopCaveTypes + NumTopCaveExits; caveTypeNum++)
        {
            for (int i = 0; i < NumCaves; i++)
            {
                int caveIndex = (caveTypeNum > NumTopCaveTypes ? (caveTypeNum - NumTopCaveTypes) : caveTypeNum);
                string cavePathStr = "Caves/CaveTop" + (caveTypeNum > NumTopCaveTypes ? "Exit" : "") + caveIndex.ToString();
                GameObject cave = (GameObject)Instantiate(Resources.Load(cavePathStr), Toolbox.Instance.HoldingArea, new Quaternion(), _caveParent);
                cave.name = "CaveTop" + caveTypeNum + "_" + i;
                _topPool.Add(GetCaveAttributes(cave));
            }
        }
        for (int caveTypeNum = 1; caveTypeNum <= NumBottomCaveTypes + NumBottomCaveExits; caveTypeNum++)
        {
            for (int i = 0; i < NumCaves; i++)
            {
                int caveIndex = (caveTypeNum > NumBottomCaveTypes ? (caveTypeNum - NumBottomCaveTypes) : caveTypeNum);
                string cavePathStr = "Caves/CaveBottom" + (caveTypeNum > NumBottomCaveTypes ? "Exit" : "") + caveIndex.ToString();
                GameObject cave = (GameObject)Instantiate(Resources.Load(cavePathStr), Toolbox.Instance.HoldingArea, new Quaternion(), _caveParent);
                cave.name = "CaveBottom" + caveTypeNum + "_" + i;
                _bottomPool.Add(GetCaveAttributes(cave));
            }
        }
    }

    private CaveType GetCaveAttributes(GameObject cave)
    {
        CaveType newCave = new CaveType
        {
            bIsActive = false,
            CaveBody = cave.GetComponent<Rigidbody2D>()
        };
        return newCave;
    }
    
    public void SetNextCavePiece(int nextTopType, int nextBottomType, bool bTopSecret, bool bBottomSecret)
    {
        if (nextTopType == -1 || nextTopType == 1001)   // TODO magic number. Bad.
        {
            _caveHandler.State = CaveHandler.CaveStates.End;
            PlaceCaveExit();
            return;
        }

        int nextTopIndex = NumCaves * (nextTopType + (bTopSecret ? NumTopCaveTypes : 0));
        int nextBottomIndex = NumCaves * (nextBottomType + (bBottomSecret ? NumBottomCaveTypes : 0));
        if (nextTopIndex == _caveIndexTopRight) { nextTopIndex++; }
        if (nextBottomIndex == _caveIndexBottomRight) { nextBottomIndex++; }

        DeactivateFirstPieces();

        // (*) << [Left] << [Mid] << [Right] << (Next)
        _caveIndexTopLeft = _caveIndexTopMid;
        _caveIndexBottomLeft = _caveIndexBottomMid;
        _caveIndexTopMid = _caveIndexTopRight;
        _caveIndexBottomMid = _caveIndexTopRight;
        _caveIndexTopRight = nextTopIndex;
        _caveIndexBottomRight = nextBottomIndex;

        ActivateCavePiece();
    }

    private void DeactivateFirstPieces()
    {
        if (_caveHandler.State == CaveHandler.CaveStates.Start)
        {
            _caveHandler.State = CaveHandler.CaveStates.Middle;
        }
        else if (_caveHandler.State == CaveHandler.CaveStates.Middle)
        {
            if (_caveEntrance.bIsActive)
            {
                _caveEntrance.bIsActive = false;
                _caveEntrance.CaveBody.velocity = Vector2.zero;
                _caveEntrance.CaveBody.position = Toolbox.Instance.HoldingArea;
            }
            else
            {
                DeactivateCavePiece(_caveIndexTopLeft, bTop: true);
                DeactivateCavePiece(_caveIndexBottomLeft, bTop: false);
            }
        }
    }

    private void ActivateCavePiece()
    {
        float topCavePieceX = _topPool[_caveIndexTopMid].CaveBody.position.x;
        float bottomCavePieceX = _bottomPool[_caveIndexBottomMid].CaveBody.position.x;
        if (_caveEntrance.bIsActive)
        {
            topCavePieceX = _caveEntrance.CaveBody.position.x;
            bottomCavePieceX = topCavePieceX;
        }
        float CavePieceY = 0f;
        float cavePieceZ = _caveZPos;

        CaveType nextTopCave = _topPool[_caveIndexTopRight];
        CaveType nextBottomCave = _bottomPool[_caveIndexBottomRight];
        nextTopCave.bIsActive = true;
        nextTopCave.CaveBody.position = new Vector3(Toolbox.TileSizeX + topCavePieceX, CavePieceY, cavePieceZ);
        nextTopCave.CaveBody.velocity = _caveVelocity;
        nextBottomCave.bIsActive = true;
        nextBottomCave.CaveBody.position = new Vector3(Toolbox.TileSizeX + bottomCavePieceX, CavePieceY, cavePieceZ);
        nextBottomCave.CaveBody.velocity = _caveVelocity;
        _topPool[_caveIndexTopRight] = nextTopCave;
        _bottomPool[_caveIndexBottomRight] = nextBottomCave;
    }

    private void DeactivateCavePiece(int pieceIndex, bool bTop)
    {
        var cave = bTop ? _topPool[pieceIndex] : _bottomPool[pieceIndex];
        cave.bIsActive = false;
        cave.CaveBody.velocity = Vector2.zero;
        cave.CaveBody.position = Toolbox.Instance.HoldingArea;

        if (bTop) {
            _topPool[pieceIndex] = cave;
        } else {
            _bottomPool[pieceIndex] = cave;
        }
    }
    
    public void SetVelocity(float speed)
    {
        if (_caveHandler.State == CaveHandler.CaveStates.Final) return;
        _caveVelocity = new Vector2(-speed, 0);
        SetActiveCaveVelocity();
    }

    private void SetActiveCaveVelocity()
    {
        foreach (CaveType cave in _topPool)
        {
            if (cave.bIsActive) { cave.CaveBody.velocity = _caveVelocity; }
        }
        foreach (CaveType cave in _bottomPool)
        {
            if (cave.bIsActive) { cave.CaveBody.velocity = _caveVelocity; }
        }

        if (_caveEntrance.bIsActive) { _caveEntrance.CaveBody.velocity = _caveVelocity; }
        if (_caveExit.bIsActive) { _caveExit.CaveBody.velocity = _caveVelocity; }
    }

    public void PlaceCaveEntrance()
    {
        _caveEntrance.bIsActive = true;
        _caveEntrance.CaveBody.position = new Vector3(0f, 0f, 0f);
        _caveEntrance.CaveBody.velocity = _caveVelocity;
    }

    public void PlaceCaveExit()
    {
        float xoffset = _bottomPool[_caveIndexBottomRight].CaveBody.position.x;
        _caveExit.bIsActive = true;
        _caveExit.CaveBody.position = new Vector3(xoffset + Toolbox.TileSizeX, 0f, 0f);
        _caveExit.CaveBody.velocity = _caveVelocity;
    }
}
