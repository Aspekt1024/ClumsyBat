using ClumsyBat;
using ClumsyBat.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditor : MonoBehaviour {

    public LevelContainer Level;
    public bool EditMode;
    public bool DebugMode;
    public LevelProgressionHandler.Levels LevelId;
    public int ScoreToBeat;

    [HideInInspector]
    public GameObject HeldObject = null;
    [HideInInspector]
    public double timeClicked;   // Used for the LevelEditorInputHandler for double-clicking
    [HideInInspector]
    public Vector2 PickupOffset; // Mouse pos offset when picking up objects

    private void Start()
    {
        GameStatics.LevelManager.Level = LevelId;
        Toolbox.Instance.Debug = DebugMode;
        SceneManager.LoadScene("Levels");
    }
}
