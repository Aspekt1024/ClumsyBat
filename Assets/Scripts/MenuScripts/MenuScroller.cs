using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScroller : MonoBehaviour {

    private RectTransform MainPanel;
    private RectTransform LevelPanel;
    private RectTransform MenuScrollRect;
    
    private Transform Caves;
    private Transform MidBG;

    private const float TileSizeX = 19.2f;
    private const float AnimDuration = 0.5f;

    private enum MenuStates
    {
        MainMenu,
        LevelSelect
    }
    private MenuStates MenuState = MenuStates.MainMenu;

    void Awake()
    {
        GetUIComponents();
        InitialiseMenu();
    }

	void Update ()
    {
        MidBG.position = new Vector3(Caves.position.x * 0.5f, 0f, MidBG.position.z);
	}

    private void InitialiseMenu()
    {
        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            MenuState = MenuStates.LevelSelect;
        }

        switch (MenuState)
        {
            case MenuStates.MainMenu:
                //LockToMainMenu();
                MenuScrollRect.GetComponent<ScrollRect>().horizontal = false;
                break;
            case MenuStates.LevelSelect:
                //LockToLevelSelect();
                MenuScrollRect.GetComponent<ScrollRect>().horizontal = true;
                break;
            default:
                //LockToMainMenu();
                MenuScrollRect.GetComponent<ScrollRect>().horizontal = false;
                break;
        }
    }

    private void GetUIComponents()
    {
        MainPanel = GameObject.Find("MainScreen").GetComponent<RectTransform>();
        LevelPanel = GameObject.Find("LevelSelect").GetComponent<RectTransform>();
        MenuScrollRect = GameObject.Find("MenuScrollRect").GetComponent<RectTransform>();

        GetBackgrounds();
    }

    public void MainMenu()
    {
        StartCoroutine("MoveToMainMenu");
    }
    
    private IEnumerator MoveToMainMenu()
    {
        float AnimTime = 0f;
        float StartX = Caves.position.x;
        const float EndX = 0f;
        float XPos = StartX;

        while (AnimTime < AnimDuration)
        {
            XPos = StartX - (StartX - EndX) * (AnimTime / AnimDuration);
            Caves.position = new Vector2(XPos, 0f);
            MainPanel.position = new Vector2(XPos, 0f);
            LevelPanel.position = new Vector2(XPos + TileSizeX, 0f);
            AnimTime += Time.deltaTime;
            yield return null;
        }
        LockToMainMenu();
    }

    private void LockToMainMenu()
    {
        Caves.position = new Vector2(0f, 0f);
        MainPanel.position = new Vector2(0f, 0f);
        LevelPanel.position = new Vector2(TileSizeX, 0f);
        MenuScrollRect.GetComponent<ScrollRect>().horizontal = false;
    }

    public void LevelSelect()
    {
        StartCoroutine("MoveToLevelSelect");
    }

    private IEnumerator MoveToLevelSelect()
    {
        float AnimTime = 0f;
        float XPos = 0f;
        while (AnimTime < AnimDuration)
        {
            XPos = -TileSizeX * (AnimTime / AnimDuration);
            Caves.position = new Vector2(XPos, 0f);
            MainPanel.position = new Vector2(XPos, 0f);
            LevelPanel.position = new Vector2(XPos + TileSizeX,0f);

            AnimTime += Time.deltaTime;
            yield return null;
        }
        LockToLevelSelect();
    }

    private void LockToLevelSelect()
    {
        Caves.position = new Vector2(-TileSizeX, 0f);
        MainPanel.position = new Vector2(-TileSizeX, 0f);
        LevelPanel.position = new Vector2(0f, 0f);
        MenuScrollRect.GetComponent<ScrollRect>().horizontal = true;
    }

    private void GetBackgrounds()
    {
        Caves = GameObject.Find("CavePieces").GetComponent<Transform>();
        MidBG = GameObject.Find("MidBackground").GetComponent<Transform>();
    }
}
