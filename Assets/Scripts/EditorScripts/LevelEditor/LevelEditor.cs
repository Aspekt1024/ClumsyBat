using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditor : MonoBehaviour {

    public LevelContainer Level;
    public bool EditMode;
    public bool DebugMode;
    public LevelProgressionHandler.Levels LevelId;

    [HideInInspector]
    public GameObject HeldObject;
    [HideInInspector]
    public double timeClicked;   // Used for the LevelEditorInputHandler for double-clicking
    [HideInInspector]
    public Vector2 PickupOffset; // Mouse pos offset when picking up objects

    private void Start()
    {
        GameData.Instance.Level = LevelId;
        Toolbox.Instance.Debug = DebugMode;
        SceneManager.LoadScene("Levels");
    }
}
