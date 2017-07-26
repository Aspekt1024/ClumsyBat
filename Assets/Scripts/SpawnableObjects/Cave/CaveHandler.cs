using System.Linq;
using UnityEngine;
public class CaveHandler : MonoBehaviour {
    
    private LevelContainer.CaveType[] _levelCaveList;
    private LevelObjectHandler _objectHandler;
    private CaveRandomiser _endlessCave;

    private Rigidbody2D _caveBody;
    private int _numCavePieces;
    private int _cavePieceCounter;
    private GameObject caveEnd;
    
    private bool _bEndlessMode;
    private bool _bGnomeEnd;

    private bool bPaused;

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
            SetObstacles();
        }
    }
    
	private void Update ()
    {
        if (Toolbox.Player.transform.position.x >= (_numCavePieces - 1f) * _tileSizeX)
        {
            Toolbox.PlayerCam.StopFollowing();
        }
    }

    public LevelObjectHandler.CaveListType GetRandomisedObstacleList()
    {
        return _endlessCave.RandomiseObstacleList(); // TODO more here.
    }
    
    public bool IsGnomeEnding() { return _bGnomeEnd; }

    public void PauseGame(bool paused)
    {
        bPaused = paused;
    }

    public GameObject GetEndCave()
    {
        return caveEnd;
    }

    public void SetVelocity(float speed)
    {
        if (State == CaveStates.Final) { return; }
        if (bPaused)
            _caveBody.velocity = Vector2.zero;
        else
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
            // TODO should probably build a factory for this
            if (cave.TopIndex == Toolbox.CaveStartIndex)
            {
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveEntrance"), caveParent.transform);
                caveTop.name = "CaveEntrance";
            }
            else if (cave.TopIndex == Toolbox.CaveEndIndex)
            {
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveExit"), caveParent.transform);
                caveTop.name = "CaveExit";
                caveEnd = caveTop;
            }
            else if (cave.TopIndex == Toolbox.CaveGnomeEndIndex)
            {
                _bGnomeEnd = true;
                caveTop = (GameObject) Instantiate(Resources.Load("Caves/CaveGnomeEnd"), caveParent.transform);
                caveTop.name = "CaveGnomeEnd";
                caveEnd = caveTop;
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

    private void SetObstacles()
    {
        for (int i = 0; i < _numCavePieces; i++)
        {
            _objectHandler.SetCaveObstacles(i);
        }
    }
}
