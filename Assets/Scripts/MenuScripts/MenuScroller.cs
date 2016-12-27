using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScroller : MonoBehaviour {

    private RectTransform MainPanel;
    private RectTransform LevelPanel;
    private RectTransform StatsPanel;
    private RectTransform LevelContentRect;
    private NavButtonHandler NavButtons;

    private Transform Caves;
    private Transform MidBG;

    private const float TileSizeX = 19.2f;
    private const float AnimDuration = 0.5f;

    private float LevelSelectPosX = 1.5f*TileSizeX;
    private const float MainMenuPosX = 0f;
    private const float StatsPosX = -TileSizeX;

    private bool bLevelCaveStartStored;
    private float LevelCaveStartX;

    private float CurrentLevel;

    public enum MenuStates
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
        SetupScreenSizing();
        SetupNavButtonHandler();
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

    public void SetCurrentLevel(int Level)
    {
        CurrentLevel = Level;
    }

    private void SetupScreenSizing()
    {
        float AspectRatio = GameObject.Find("Main Camera").GetComponent<Camera>().aspect;
        LevelSelectPosX *= (1.5f + AspectRatio) / (2 * AspectRatio);
        Transform LST = GameObject.Find("LevelSelectImg").GetComponent<Transform>();
        LST.localScale *= 1.5f / AspectRatio;
        bLevelCaveStartStored = false;
    }

    private void SetupNavButtonHandler()
    {
        GameObject RuntimeScripts = GameObject.Find("Runtime Scripts");
        NavButtons = RuntimeScripts.AddComponent<NavButtonHandler>();
        NavButtons.SetupNavButtons(MenuState);
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
        NavButtons.SetupNavButtons(MenuState);

        float AnimTimer = 0f;
        float StartX = Caves.position.x;
        float EndX = -GetXOffset();

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

        if (MenuState == MenuStates.LevelSelect)
        {
            GotoCurrentLevel();
        }
    }

    private void FinaliseMenuPosition()
    {
        float XOffset = GetXOffset();

        Caves.position = new Vector2(0f - XOffset, 0f);
        MainPanel.position = new Vector2(MainMenuPosX - XOffset, 0f);
        StatsPanel.position = new Vector2(StatsPosX - XOffset, 0f);
        LevelPanel.position = new Vector2(LevelSelectPosX - XOffset, 0f);
        
        if (MenuState == MenuStates.LevelSelect && !bLevelCaveStartStored)
        {
            bLevelCaveStartStored = true;
            LevelCaveStartX = LevelContentRect.position.x;
        }
    }

    private float GetXOffset()
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
        return XOffset;
    }

    private void GotoCurrentLevel()
    {
        float MaxPosX = 7f;
        RectTransform LvlButton = null;
        LvlButton = GameObject.Find("Lv" + CurrentLevel).GetComponent<RectTransform>();
        if (!LvlButton) { return; }

        float LvlButtonPosX = LvlButton.position.x - LevelContentRect.position.x;
        if (LvlButtonPosX > MaxPosX)
        {
            float XShift = LvlButtonPosX - MaxPosX;
            float NormalisedPosition = XShift / LevelContentRect.rect.width * 100f; // TODO convert 100 to scalevalue
            StartCoroutine("GotoLevelAnim", NormalisedPosition);
        }
    }

    private IEnumerator GotoLevelAnim(float NormalisedPosition)
    {
        const float AnimDuration = 0.5f;
        float AnimTimer = 0f;
        float StartPos = 0f;
        float EndPos = NormalisedPosition;

        ScrollRect LevelScrollRect = LevelPanel.GetComponent<ScrollRect>();

        float Pos;
        while (AnimTimer < AnimDuration)
        {
            Pos = StartPos - (StartPos - EndPos) * (AnimTimer / AnimDuration);
            LevelScrollRect.horizontalNormalizedPosition = Pos;

            AnimTimer += Time.deltaTime;
            yield return null;
        }
        LevelScrollRect.horizontalNormalizedPosition = NormalisedPosition;
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
