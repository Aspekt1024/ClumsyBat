using UnityEngine;
using System;
using System.Collections.Generic;

using Levels = LevelProgressionHandler.Levels;

public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { }

    public Language Language = new Language();

    public Vector3 HoldingArea { get; set; }
    public float PlayerSpeed { get; set; }
    public float PlayerDashSpeed { get; set; }
    public float GravityScale { get; set; }
    public const float TileSizeX = 19.2f;
    public const float PlayerStartX = -5.5f;
    public bool GamePaused;
    public bool Debug;
    public bool ShowLevelTooltips = true;

    public List<int> TooltipCompletion = new List<int>();
    public Dictionary<string, float> ZLayers = new Dictionary<string, float>();
    public Dictionary<Levels, string> LevelNames = new Dictionary<Levels, string>();

    private static Player playerScript;
    private static MainAudioControl mainAudio;
    private CameraFollowObject playerCamScript;
    private TooltipHandler tooltipHandler;
    private UIObjectAnimator objectAnimator;

    public static TooltipHandler Tooltips
    {
        get
        {
            if (Instance.tooltipHandler == null)
                Instance.tooltipHandler = GameObject.FindGameObjectWithTag("Tooltips").GetComponent<TooltipHandler>();
            return Instance.tooltipHandler;
        }
    }

    public static UIObjectAnimator UIAnimator
    {
        get
        {
            if (Instance.objectAnimator == null)
            {
                GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
                Instance.objectAnimator = scripts.GetComponent<UIObjectAnimator>();
                if (Instance.objectAnimator == null)
                    Instance.objectAnimator = scripts.AddComponent<UIObjectAnimator>();
            }
            return Instance.objectAnimator;
        }
    }

    public static Player Player
    {
        get
        {
            if (playerScript == null)
            {
                GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject playerObj in playerObjects)
                {
                    if (playerObj.name == "Player")
                    {
                        playerScript = playerObj.GetComponent<Player>();
                        break;
                    }
                }
            }
            return playerScript;
        }
    }

    public static CameraFollowObject PlayerCam
    {
        get
        {
            if (Instance.playerCamScript == null)
            {
                GameObject[] camObjects = GameObject.FindGameObjectsWithTag("MainCamera");
                foreach (GameObject camObj in camObjects)
                {
                    if (camObj.GetComponent<CameraFollowObject>() != null)
                    {
                        Instance.playerCamScript = camObj.GetComponent<CameraFollowObject>();
                        break;
                    }
                }
            }
            return Instance.playerCamScript;
        }
    }

    public static MainAudioControl MainAudio
    {
        get
        {
            if (mainAudio == null)
                mainAudio = GameObject.FindGameObjectWithTag("AudioController").GetComponent<MainAudioControl>();

            return mainAudio;
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
        HoldingArea = new Vector2(0, 100);
        PlayerSpeed = 5.5f;
        GravityScale = 3f;
        MenuScreen = MenuSelector.MainMenu;
        
        SetupZLayers();
        SetupLevelNames();
    }

    private void SetupZLayers()
    {
        // Smaller values are closer to the camera
        ZLayers.Add("MainCamera", -10f);
        ZLayers.Add("UIOverlay", 2);            // Parented to MainCamera (-10)
        ZLayers.Add("GameMenuOverlay", 3f);     // Parented to MainCamera (-10)
        ZLayers.Add("Moth", -5f);
        ZLayers.Add("Fog", -3.5f);
        ZLayers.Add("CaveEndFront", -2f);
        ZLayers.Add("Projectile", -1.5f);
        ZLayers.Add("Lantern", -1.1f);
        ZLayers.Add("Player", -1f);
        ZLayers.Add("LanternLight", -0.8f);
        ZLayers.Add("NPC", -0.6f);
        ZLayers.Add("Trigger", -0.5f);
        ZLayers.Add("Cave", 0f);
        ZLayers.Add("Hypersonic", 1f);
        ZLayers.Add("Spore", 3.9f);
        ZLayers.Add("Stalactite", 4f);
        ZLayers.Add("Mushroom", 5f);
        ZLayers.Add("Spider", 6f);
        ZLayers.Add("Web", 7f);
        ZLayers.Add("Background", 20f);
    }

    private void SetupLevelNames()
    {
        LevelNames.Add(Levels.Main1, "Darkness");
        LevelNames.Add(Levels.Main2, "Impasse");
        LevelNames.Add(Levels.Main3, "Blind Hope");
        LevelNames.Add(Levels.Main4, "Shayla");
        LevelNames.Add(Levels.Main5, "Promise");
        LevelNames.Add(Levels.Main6, "Courage");
        LevelNames.Add(Levels.Main7, "Echo");
        LevelNames.Add(Levels.Main8, "Location");
        LevelNames.Add(Levels.Main9, "9ine");
        LevelNames.Add(Levels.Main10, "Tenpin");
        LevelNames.Add(Levels.Main11, "Elfen");
        LevelNames.Add(Levels.Main12, "Oceans");
        LevelNames.Add(Levels.Main13, "Luck");
        LevelNames.Add(Levels.Main14, "Spaceship");
        LevelNames.Add(Levels.Main15, "Hit");
        LevelNames.Add(Levels.Main16, "Hit");

        LevelNames.Add(Levels.Boss1, "A New Hope");
        LevelNames.Add(Levels.Boss2, "Rockbreath Jr.");
        LevelNames.Add(Levels.Boss3, "Sonic");
        LevelNames.Add(Levels.Boss4, "King Rockbreath");
        LevelNames.Add(Levels.Boss5, "Boss5");

        LevelNames.Add(Levels.Boss6, "Boss6");
        LevelNames.Add(Levels.Boss7, "Boss7");
        LevelNames.Add(Levels.Boss8, "Boss8");
        LevelNames.Add(Levels.Boss9, "Boss9");

        LevelNames.Add(Levels.Village1, "Village 1");
        LevelNames.Add(Levels.Village2, "Village 2");
        LevelNames.Add(Levels.Village3, "Village 3");
        LevelNames.Add(Levels.Village4, "Village 4");
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
}

[Serializable]
public class Language
{
    public string Current;
    public string LastLang;
}