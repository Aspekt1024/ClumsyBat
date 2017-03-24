using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorObjectReferencer : MonoBehaviour
{
    public Transform CaveParent;
    public Transform MothParent;
    public Transform StalParent;
    public Transform ShroomParent;
    public Transform WebParent;
    public Transform SpiderParent;
    public Transform TriggerParent;
    public Transform NpcParent;

    private void Start()
    {
        CaveParent = GameObject.Find("Caves").transform;
        MothParent = GameObject.Find("Moths").transform;
        StalParent = GameObject.Find("Stalactites").transform;
        SpiderParent = GameObject.Find("Spiders").transform;
        TriggerParent = GameObject.Find("Triggers").transform;
        ShroomParent = GameObject.Find("Mushrooms").transform;
        NpcParent = GameObject.Find("Npcs").transform;
        WebParent = GameObject.Find("Webs").transform;
    }
}
