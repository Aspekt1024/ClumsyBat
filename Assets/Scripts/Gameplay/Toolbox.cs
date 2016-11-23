using UnityEngine;

public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    public Language language = new Language();

    public Vector3 HoldingArea { get; set; }
    public float LevelSpeed { get; set; }
    public float GravityScale { get; set; }
    public int Level { get; set; }
    public const float TileSizeX = 19.2f;

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
    }



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