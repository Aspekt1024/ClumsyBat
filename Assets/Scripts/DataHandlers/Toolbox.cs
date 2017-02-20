using UnityEngine;
using System;
using System.Collections.Generic;

using Levels = LevelProgressionHandler.Levels;

public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    public Language Language = new Language();

    public Vector3 HoldingArea { get; set; }
    public float LevelSpeed { get; set; }
    public float GravityScale { get; set; }
    public const float TileSizeX = 19.2f;
    public const float PlayerStartX = -5.5f;
    public bool GamePaused;
    public bool Debug;
    public bool TooltipCompletionPersist;
    public bool ShowLevelTooltips = true;

    public bool[] TooltipCompletion = new bool[Enum.GetNames(typeof(TooltipHandler.DialogueId)).Length];
    public Dictionary<string, float> ZLayers = new Dictionary<string, float>();
    public Dictionary<Levels, string> LevelNames = new Dictionary<Levels, string>();

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
        ZLayers.Add("NPC", -0.7f);
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

        LevelNames.Add(Levels.Boss1, "Evil Clumsy");
        LevelNames.Add(Levels.Boss2, "King Rockbreath");
        LevelNames.Add(Levels.Training1, "Rocky");
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