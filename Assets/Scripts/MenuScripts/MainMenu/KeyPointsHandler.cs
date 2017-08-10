using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPointsHandler : MonoBehaviour {

    public GameObject MainMenuCamPoint;
    public GameObject LevelCamPoint;
    public GameObject DropdownAreaPoint;

    public GameObject EntryPoint;
    public GameObject EntryLandingPoint;
    public GameObject LevelEntryPoint;
    public GameObject LevelMenuMidPoint;
    public GameObject LevelMenuEndPoint;

    public GameObject LevelMapStart;
    
    private void Start()
    {
        foreach(SpriteRenderer kp in GameObject.Find("KeyPoints").GetComponentsInChildren<SpriteRenderer>())
        {
            kp.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}
