using UnityEngine;
using System.Collections.Generic;

using Levels = LevelProgressionHandler.Levels;
using ClumsyBat;

[ExecuteInEditMode]
public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { }

    public Vector3 HoldingArea { get; set; }
    public float PlayerDashSpeed { get; set; }
    public const float TileSizeX = 19.2f;
    public const float PlayerStartX = -5.5f;
    public bool GamePaused;
    public bool Debug;
    public bool ShowLevelTooltips = true;

    public bool ReturnToLevelEditor = false;

    public List<int> TooltipCompletion = new List<int>();

    // TODO replace with sorting layers
    public readonly Dictionary<string, float> ZLayers = new Dictionary<string, float>()
    {   // Smaller values are closer to the camera
        {"MainCamera", -10f},
        {"UIOverlay", 2},            // Parented to MainCamera (-10)
        {"GameMenuOverlay", 3f},     // Parented to MainCamera (-10)
        {"Moth", -5f},
        {"Fog", -3.5f},
        {"CaveEndFront", -2f},
        {"Projectile", -1.5f},
        {"Lantern", -1.1f},
        {"Player", -1f},
        {"LanternLight", -0.8f},
        {"NPC", -0.6f},
        {"Trigger", -0.5f},
        {"Cave", 0f},
        {"Hypersonic", 1f},
        {"Spore", 3.9f},
        {"Stalactite", 4f},
        {"Mushroom", 5f},
        {"Spider", 6f},
        {"Web", 7f},
        {"Background", 20f},
    };

    public readonly Dictionary<Levels, string> LevelNames = new Dictionary<Levels, string>()
    {
        {Levels.Main1, "Darkness"},
        {Levels.Main2, "Impasse"},
        {Levels.Main3, "Blind Hope"},
        {Levels.Main4, "Shayla"},
        {Levels.Main5, "Promise"},
        {Levels.Main6, "Courage"},
        {Levels.Main7, "Echo"},
        {Levels.Main8, "Location"},
        {Levels.Main9, "9ine"},
        {Levels.Main10, "Tenpin"},
        {Levels.Main11, "Elfen"},
        {Levels.Main12, "Oceans"},
        {Levels.Main13, "Luck"},
        {Levels.Main14, "Spaceship"},
        {Levels.Main15, "Hit"},
        {Levels.Main16, "Hit2"},

        {Levels.BossS1, "A New Hope"},
        {Levels.BossS2, "Sonic"},

        {Levels.Boss1, "Rockbreath"},
        {Levels.Boss2, "Rockbreath Jr."},
        {Levels.Boss3, "Elder Rockbreath"},
        {Levels.Boss4, "King Rockbreath"},
        {Levels.Boss5, "Count Nomee"},

        {Levels.Boss6, "Rockbreath Demo"},
        {Levels.Boss7, "Rockbreath Omega"},
        {Levels.Boss8, "Rockbreath Prime"},
        {Levels.Boss9, "Nomee Prime"},
    };
    
    private static MainAudioControl mainAudio;
    private CameraFollowObject playerCamScript;
    private TooltipHandler tooltipHandler;
    private UIObjectAnimator objectAnimator;

    public static TooltipHandler Tooltips
    {
        get
        {
            if (Instance.tooltipHandler == null)
                Instance.tooltipHandler = GameObject.FindObjectOfType<TooltipHandler>();
            return Instance.tooltipHandler;
        }
    }

    public static CameraFollowObject PlayerCam
    {
        get
        {
            if (Instance.playerCamScript == null)
            {
                Camera camObject = GameStatics.Camera.LevelCamera;
                Instance.playerCamScript = camObject.GetComponent<CameraFollowObject>();
            }
            return Instance.playerCamScript;
        }
    }
    
    public enum MenuSelector
    {
        LevelSelect,
        MainMenu
    }
    public MenuSelector MenuScreen { get; set; }

    public const int CaveStartIndex = 1000;
    public const int CaveEndIndex = 1001;
    public const int CaveGnomeEndIndex = 1002;
    
    private void Awake()
    {
        if (Application.isPlaying)
        {
            HoldingArea = new Vector2(0, 100);
            MenuScreen = MenuSelector.MainMenu;
        }
        else
        {
            CheckReturnToLevelEditor();
        }
    }

    #region TooltipMemory
    // The below functions relate to session level tooltips
    // Tooltips are shown the first time a level is started, but not on restarting the level
    public bool TooltipCompleted(int tooltipId)
    {
        foreach(int id in TooltipCompletion)
        {
            if (id == tooltipId)
                return true;
        }
        return false;
    }

    public void SetTooltipComplete(int tooltipId)
    {
        TooltipCompletion.Add(tooltipId);
    }

    public void ResetTooltips()
    {
        TooltipCompletion = new List<int>();
    }
    #endregion TooltipMemory

    public static Color MothGreenColor = new Color(110 / 255f, 229 / 255f, 119 / 255f);
    public static Color MothGoldColor = new Color(212 / 255f, 195 / 255f, 126 / 255f);
    public static Color MothBlueColor = new Color(151 / 255f, 147 / 255f, 231 / 255f);

    public override void OnDestroy()
    {
        if (Application.isPlaying)
        {
            base.OnDestroy();
        }
    }

    private void CheckReturnToLevelEditor()
    {
#if UNITY_EDITOR
        if (ReturnToLevelEditor)
        {
            ReturnToLevelEditor = false;
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/LevelEditor.unity");
        }
#endif
    }
}
