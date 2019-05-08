using ClumsyBat;
using ClumsyBat.Objects;
using System.Collections.Generic;
using UnityEngine;

public class CaveHandler : MonoBehaviour {
    
    private Rigidbody2D caveBody;

    private GameObject startPiece;
    private GameObject endPiece;
    private List<GameObject> topCavePieces = new List<GameObject>();
    private List<GameObject> bottomCavePieces = new List<GameObject>();

    private int numCavePieces;
    private int _cavePieceCounter;
    private GameObject caveEnd;

    private GameObject caveParent;
    
    private bool bPaused;

    public enum CaveStates
    {
        Start,
        Middle,
        End,
        Final
    }
    public CaveStates State;
    
    private float caveZLayer;
    private float tileSizeX;

    private void Start()
    {
        caveZLayer = Toolbox.Instance.ZLayers["Cave"];
        tileSizeX = Toolbox.TileSizeX;

        var levelObj = GameObject.Find("Level").transform;
        caveParent = new GameObject("Caves");
        caveParent.transform.SetParent(levelObj);

        caveBody = caveParent.AddComponent<Rigidbody2D>();
        caveBody.isKinematic = true;
    }

    public void SetupCave(LevelContainer.CaveType[] caveList)
    {
        numCavePieces = caveList.Length;
        
        for (int i = 0; i < caveList.Length; i++)
        {
            GeneratePresetLevelCave(caveList[i], i);
        }
    }
    
	private void Update ()
    {
        // TODO rework this - even a trigger would be better
        //if (GameStatics.GameManager. GameStatics.Player.Model.Transform.position.x >= (numCavePieces - 1f) * tileSizeX)
        //{
        //    GameStatics.Camera.StopFollowing();
        //}
    }

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
            caveBody.velocity = Vector2.zero;
        else
            caveBody.velocity = new Vector2(-speed, 0);
    }

    private void GeneratePresetLevelCave(LevelContainer.CaveType cave, int index)
    {
        GameObject caveTop;

        if (cave.TopIndex == Toolbox.CaveStartIndex)
        {
            if (startPiece == null)
            {
                startPiece = (GameObject)Instantiate(Resources.Load("Caves/CaveEntrance"), caveParent.transform);
                startPiece.name = "CaveEntrance";
            }
            caveTop = startPiece;
        }
        else if (cave.TopIndex == Toolbox.CaveEndIndex)
        {
            if (endPiece == null)
            {
                endPiece = (GameObject)Instantiate(Resources.Load("Caves/CaveExit"), caveParent.transform);
                endPiece.name = "CaveExit";
            }
            caveTop = caveEnd = endPiece;
        }
        else
        {
            string caveBottomName = "CaveBottom" + (cave.bBottomSecretPath ? "Exit" : "") + (cave.BottomIndex + 1);
            string caveTopName = "CaveTop" + (cave.bTopSecretPath ? "Exit" : "") + (cave.TopIndex + 1);
            GameObject caveBottom = (GameObject)Instantiate(Resources.Load("Caves/" + caveBottomName), caveParent.transform);
            caveTop = (GameObject)Instantiate(Resources.Load("Caves/" + caveTopName), caveParent.transform);
            caveBottom.name = caveBottomName;
            caveTop.name = caveTopName;
            caveBottom.transform.position = new Vector3(tileSizeX * index, 0f, caveZLayer);

            topCavePieces.Add(caveTop);
            bottomCavePieces.Add(caveBottom);

            SecretPath path = caveBottom.GetComponentInChildren<SecretPath>();
            if (path == null) path = caveTop.GetComponentInChildren<SecretPath>();
            if (path != null)
            {
                if (!cave.bSecretPathHasBlock) Destroy(path.gameObject);
                path.RequiresBlueMoth = cave.bSecretPathRequiresMoth;
            }
        }
        caveTop.transform.position = new Vector3(tileSizeX * index, 0f, caveZLayer);
    }

    public void ClearExistingCave()
    {
        for (int i = topCavePieces.Count - 1; i >= 0; i--)
        {
            Destroy(topCavePieces[i]);
        }
        for (int i = bottomCavePieces.Count - 1; i >= 0; i--)
        {
            Destroy(bottomCavePieces[i]);
        }

        topCavePieces.Clear();
        bottomCavePieces.Clear();

        if (startPiece != null)
        {
            startPiece.transform.position = new Vector3(0, 10000, 0);
        }

        if (endPiece != null)
        {
            endPiece.transform.position = new Vector3(0, 10000, 0);
        }
    }
}
