﻿using UnityEngine;
using System.Collections.Generic;

public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    public Language language = new Language();

    public Vector3 HoldingArea { get; set; }
    public float LevelSpeed { get; set; }
    public float GravityScale { get; set; }
    public int Level { get; set; }
    public const float TileSizeX = 19.2f;

    public Dictionary<string, float> ZLayers = new Dictionary<string, float>();
    public Dictionary<int, string> LevelNames = new Dictionary<int, string>();

    public enum MenuSelector
    {
        LevelSelect,
        MainMenu
    }
    public MenuSelector MenuScreen { get; set; }

    void Awake()
    {
        HoldingArea = new Vector2(100, 0);
        LevelSpeed = 4f;
        GravityScale = 4f;
        MenuScreen = MenuSelector.MainMenu;
        
        SetupZLayers();
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
        ZLayers.Add("Hypersonic", 1f);
        ZLayers.Add("Cave", 0f);
        ZLayers.Add("Spore", 3.9f);
        ZLayers.Add("Stalactite", 4f);
        ZLayers.Add("Mushroom", 5f);
        ZLayers.Add("Background", 20f);
        ZLayers.Add("NearBackground", 0f);      // Child of Background (20)
        ZLayers.Add("MidBackground", 1f);       // Child of Background (20)
        ZLayers.Add("FarBackground", 2f);       // Child of Background (20)
    }

    // TODO Level names?


    /*// (optional) allow runtime registration of global objects
    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
    Note: to run this, need to create MonoBehaviourExtended.cs
    See http://wiki.unity3d.com/index.php/Toolbox
     */
}

[System.Serializable]
public class Language
{
    public string current;
    public string lastLang;
}