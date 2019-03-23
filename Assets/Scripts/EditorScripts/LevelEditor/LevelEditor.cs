using ClumsyBat.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
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
#if UNITY_EDITOR 
        var playScene = SceneManager.GetSceneByName("Play");
        if (playScene == null) return;
        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(playScene, true);
#endif
    }

}
