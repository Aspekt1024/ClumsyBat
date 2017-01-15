using UnityEngine;
public class CaveHandler : MonoBehaviour {
    
    private LevelContainer.CaveType[] _levelCaveList;
    private LevelObjectHandler _objectHandler;
    private CaveRandomiser _endlessCave;

    private Rigidbody2D _caveBody;
    private int _numCavePieces;
    private int _cavePieceCounter;
    
    private bool _bEndlessMode;

    public enum CaveStates
    {
        Start,
        Middle,
        End,
        Final
    }
    public CaveStates State;
    
    private float _caveZ;
    private float _tileSizeX;

    public void Setup(LevelContainer.CaveType[] caveList, bool bEndless, LevelObjectHandler objHandler)
    {
        _levelCaveList = caveList;
        _objectHandler = objHandler;
        _bEndlessMode = bEndless;

        _caveZ = Toolbox.Instance.ZLayers["Cave"];
        _tileSizeX = Toolbox.TileSizeX;

        if (_bEndlessMode)
        {
            _endlessCave = new CaveRandomiser();
            // TODO generate random cave
        }
        else
        {
            _numCavePieces = _levelCaveList.Length;
            GeneratePresetLevelCave();
            SetStartingObstacles();
        }
        _cavePieceCounter = 0;
    }
    
	private void Update ()
    {
        var caveX = _caveBody.position.x;
        if (caveX < -_tileSizeX * _cavePieceCounter)
        {
            _cavePieceCounter++;
            if (_cavePieceCounter > 1 && _cavePieceCounter < _numCavePieces) { _objectHandler.SetCaveObstacles(_cavePieceCounter); }
        }
        if (_cavePieceCounter < _numCavePieces) { return; }
        SetVelocity(0);
        State = CaveStates.Final;
    }

    public LevelObjectHandler.CaveListType GetRandomisedObstacleList()
    {
        return _endlessCave.RandomiseObstacleList(); // TODO more here.
    }

    public bool AtCaveEnd() { return State == CaveStates.Final; }


    public void SetVelocity(float speed)
    {
        if (State == CaveStates.Final) { return; }
        _caveBody.velocity = new Vector2(-speed, 0);
    }

    private void GeneratePresetLevelCave()
    {
        var levelObj = new GameObject("Level").transform;
        var caveParent = new GameObject("Caves");
        _caveBody = caveParent.AddComponent<Rigidbody2D>();
        _caveBody.isKinematic = true;
        caveParent.transform.SetParent(levelObj);
        
        for (int i = 0; i < _levelCaveList.Length; i++)
        {
            LevelContainer.CaveType cave = _levelCaveList[i];
            GameObject caveTop;
            if (cave.TopIndex == 1000)
            {
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveEntrance"), caveParent.transform);
                caveTop.name = "CaveEntrance";
            }
            else if (cave.TopIndex == 1001)
            {
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveExit"), caveParent.transform);
                caveTop.name = "CaveExit";
            }
            else
            {
                string caveBottomName = "CaveBottom" + (cave.bBottomSecretPath ? "Exit" : "") + (cave.BottomIndex + 1);
                string caveTopName = "CaveTop" + (cave.bTopSecretPath ? "Exit" : "") + (cave.TopIndex + 1);
                GameObject caveBottom = (GameObject)Instantiate(Resources.Load("Caves/" + caveBottomName), caveParent.transform);
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/" + caveTopName), caveParent.transform);
                caveBottom.name = caveBottomName;
                caveTop.name = caveTopName;
                caveBottom.transform.position = new Vector3(_tileSizeX * i, 0f, _caveZ);
            }
            caveTop.transform.position = new Vector3(_tileSizeX * i, 0f, _caveZ);
        }
    }

    private void SetStartingObstacles()
    {
        _objectHandler.SetCaveObstacles(0);
        _objectHandler.SetCaveObstacles(1);
    }
}
