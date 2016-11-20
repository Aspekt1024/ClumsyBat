using UnityEngine;
using System.Collections;

public class CavePool : MonoBehaviour {
    
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

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
