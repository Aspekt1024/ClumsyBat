using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScroller : MonoBehaviour {

    private RectTransform MainPanel;
    private RectTransform LevelPanel;
    private RectTransform StatsPanel;
    private RectTransform LevelContentRect;
    
    private Transform Caves;
    private Transform MidBG;

    private const float TileSizeX = 19.2f;
    private const float AnimDuration = 0.5f;

    private float LevelSelectPosX = 1.5f*TileSizeX;
    private const float MainMenuPosX = 0f;
    private const float StatsPosX = -TileSizeX;

    private bool bLevelCaveStartStored;
    private float LevelCaveStartX;

    private float LevelPanelStartX;

    private enum MenuStates
    {
        MainMenu,
        LevelSelect,
        StatsScreen
    }
    private MenuStates MenuState = MenuStates.MainMenu;

    void Awake()
    {
        GetUIComponents();
        InitialiseMenu();
        float AspectRatio = GameObject.Find("Main Camera").GetComponent<Camera>().aspect;
        LevelSelectPosX *= (1.5f + AspectRatio) / (2 * AspectRatio);
        //LevelContentRect.sizeDelta = new Vector3(LevelContentRect.rect.x * AspectRatio / 1.5f, 0f, 0f);
        Transform LST = GameObject.Find("LevelSelectImg").GetComponent<Transform>();
        LST.localScale *= 1.5f / AspectRatio;
        LevelPanelStartX = LevelPanel.position.x;
        bLevelCaveStartStored = false;
    }

	void Update ()
    {
        MidBG.position = new Vector3(Caves.position.x * 0.5f, 0f, MidBG.position.z);
        if (MenuState == MenuStates.LevelSelect)
        {
            Caves.position = new Vector2(LevelContentRect.position.x - LevelCaveStartX - LevelSelectPosX, 0f);
        }

        if (LevelPanel.position.y != 0f)
        {
            FinaliseMenuPosition();
        }
	}

    private void InitialiseMenu()
    {
        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            MenuState = MenuStates.LevelSelect;
        }
        FinaliseMenuPosition();
    }

    private void GetUIComponents()
    {
        MainPanel = GameObject.Find("MainScreen").GetComponent<RectTransform>();
        LevelPanel = GameObject.Find("LevelScrollRect").GetComponent<RectTransform>();
        StatsPanel = GameObject.Find("StatsPanel").GetComponent<RectTransform>();
        LevelContentRect = GameObject.Find("Content").GetComponent<RectTransform>();

        GetBackgrounds();
    }

    public void MainMenu()
    {

        if (bLevelCaveStartStored && MenuState == MenuStates.LevelSelect)
        {
            LevelContentRect.position = new Vector2(LevelCaveStartX, 0f);
        }

        MenuState = MenuStates.MainMenu;
        StartCoroutine("MoveMenu");
    }

    public void LevelSelect()
    {
        MenuState = MenuStates.LevelSelect;
        StartCoroutine("MoveMenu");
    }

    public void StatsScreen()
    {
        MenuState = MenuStates.StatsScreen;
        StartCoroutine("MoveMenu");
    }

    private IEnumerator MoveMenu()
    {
        float AnimTimer = 0f;
        float StartX = Caves.position.x;
        float EndX = 0f;
        switch (MenuState)
        {
            case MenuStates.MainMenu:
                EndX = -MainMenuPosX;
                break;
            case MenuStates.LevelSelect:
                EndX = -LevelSelectPosX;
                break;
            case MenuStates.StatsScreen:
                EndX = -StatsPosX;
                break;
        }

        float XPos;
        while (AnimTimer < AnimDuration)
        {
            XPos = StartX - (StartX - EndX) * (AnimTimer / AnimDuration);
            Caves.position = new Vector2(XPos, 0f);
            MainPanel.position = new Vector2(MainMenuPosX + XPos, 0f);
            StatsPanel.position = new Vector2(StatsPosX + XPos, 0f);
            LevelPanel.position = new Vector2(LevelSelectPosX + XPos, 0f);
            AnimTimer += Time.deltaTime;
            yield return null;
        }
        FinaliseMenuPosition();
    }

    private void FinaliseMenuPosition()
    {
        float XOffset = 0f;
        switch (MenuState)
        {
            case MenuStates.MainMenu:
                XOffset = MainMenuPosX;
                break;
            case MenuStates.LevelSelect:
                XOffset = LevelSelectPosX;
                break;
            case MenuStates.StatsScreen:
                XOffset = StatsPosX;
                break;
        }

        Caves.position = new Vector2(0f - XOffset, 0f);
        MainPanel.position = new Vector2(MainMenuPosX - XOffset, 0f);
        StatsPanel.position = new Vector2(StatsPosX - XOffset, 0f);
        LevelPanel.position = new Vector2(LevelSelectPosX - XOffset, 0f);
        
        if (MenuState == MenuStates.LevelSelect && !bLevelCaveStartStored)
        {
            bLevelCaveStartStored = true;
            LevelCaveStartX = LevelContentRect.position.x;
        }

        if (MenuState == MenuStates.LevelSelect)
        {
            LevelPanel.GetComponent<ScrollRect>().horizontal = true;
        }
        else
        {
            LevelPanel.GetComponent<ScrollRect>().horizontal = false;
        }
    }

    private void GetBackgrounds()
    {
        Caves = GameObject.Find("CavePieces").GetComponent<Transform>();
        MidBG = GameObject.Find("MidBackground").GetComponent<Transform>();
    }

    public float GetAnimDuration()
    {
        return AnimDuration;
    }
}
