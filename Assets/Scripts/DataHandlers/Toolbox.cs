using UnityEngine;
using System;
using System.Collections.Generic;

public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    public Language Language = new Language();

    public Vector3 HoldingArea { get; set; }
    public float LevelSpeed { get; set; }
    public float GravityScale { get; set; }
    public int Level { get; set; }
    public const float TileSizeX = 19.2f;
    public const float PlayerStartX = -5.5f;
    public bool Debug;
    public bool TooltipCompletionPersist;
    public bool ShowLevelTooltips = true;

    public bool[] TooltipCompletion = new bool[Enum.GetNames(typeof(TooltipHandler.DialogueId)).Length];
    public Dictionary<string, float> ZLayers = new Dictionary<string, float>();
    public Dictionary<int, string> LevelNames = new Dictionary<int, string>();

    public enum MenuSelector
    {
        LevelSelect,
        MainMenu
    }
    public MenuSelector MenuScreen { get; set; }

    public enum LevelPrefix
    {
        LevelPrefix = 0,
        BossPrefix = 1000,
        TrainingPrefix = 2000
    }

    public const int CaveStartIndex = 1000;
    public const int CaveEndIndex = 1001;
    public const int CaveGnomeEndIndex = 1002;
    
    private void Awake()
    {
        HoldingArea = new Vector2(100, 0);
        LevelSpeed = 4f;
        GravityScale = 4f;
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
        ZLayers.Add("Moth", -4f);
        ZLayers.Add("Fog", -3.5f);
        ZLayers.Add("CaveEndFront", -2f);
        ZLayers.Add("Lantern", -1.1f);
        ZLayers.Add("Player", -1f);
        ZLayers.Add("LanternLight", -0.8f);
        ZLayers.Add("Trigger", -0.5f);
        // TODO WebFront 0.1f?
        ZLayers.Add("Cave", 0f);
        ZLayers.Add("Hypersonic", 1f);
        ZLayers.Add("Spore", 3.9f);
        ZLayers.Add("Stalactite", 4f);
        ZLayers.Add("Mushroom", 5f);
        ZLayers.Add("Spider", 6f);
        ZLayers.Add("Web", 7f);
        ZLayers.Add("Background", 20f);
        ZLayers.Add("NearBackground", 0f);      // Child of Background (20)
        ZLayers.Add("MidBackground", 1f);       // Child of Background (20)
        ZLayers.Add("FarBackground", 2f);       // Child of Background (20)
    }

    private void SetupLevelNames()
    {
        LevelNames.Add(1, "Darkness");
        LevelNames.Add(2, "Impasse");
        LevelNames.Add(3, "Blind Hope");
        LevelNames.Add(4, "Shayla");
        LevelNames.Add(5, "Promise");
        LevelNames.Add(6, "Courage");
        LevelNames.Add(7, "Echo");
        LevelNames.Add(8, "Location");
        LevelNames.Add(9, "9ine");
        LevelNames.Add(10, "Tenpin");
        LevelNames.Add(11, "Elfen");
        LevelNames.Add(12, "Oceans");
        LevelNames.Add(13, "Luck");
        LevelNames.Add(14, "Spaceship");
        LevelNames.Add(15, "Hit");

        LevelNames.Add((int)LevelPrefix.BossPrefix + 1, "Evil Clumsy");

        LevelNames.Add((int)LevelPrefix.TrainingPrefix + 1, "Rocky");
    }

    // The below functions relate to session level tooltips
    // Tooltips are shown the first time a level is started, but not on restarting the level
    public bool TooltipCompleted(TooltipHandler.DialogueId tooltipId)
    {
        return TooltipCompletion[(int)tooltipId];
    }
    public void SetTooltipComplete(TooltipHandler.DialogueId tooltipId)
    {
        TooltipCompletion[(int)tooltipId] = true;
    }
    public void ResetTooltips()
    {
        TooltipCompletion = new bool[Enum.GetNames(typeof(TooltipHandler.DialogueId)).Length];
    }
}

[Serializable]
public class Language
{
    public string Current;
    public string LastLang;
}