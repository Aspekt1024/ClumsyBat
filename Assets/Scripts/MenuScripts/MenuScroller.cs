using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScroller : MonoBehaviour {

    private RectTransform MainPanel;
    private RectTransform LevelScroller;
    private RectTransform StatsPanel;
    private RectTransform LevelContentRect;
    private NavButtonHandler NavButtons;

    private Transform Caves;
    private Transform MidBG;

    private const float TileSizeX = 19.2f;
    private const float AnimDuration = 1.0f;

    private float LevelScrollInitialPos;
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
    private bool bMenuTransition = false;

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
        if (MenuState == MenuStates.LevelSelect && !bMenuTransition)
        {
            Caves.position = new Vector2(LevelContentRect.position.x - LevelCaveStartX - LevelSelectPosX, 0f);
        }

        if (LevelScroller.position.y != 0f)
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
        // TODO setup autoscaling of content?
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
        LevelScrollInitialPos = LevelScroller.position.x;
        FinaliseMenuPosition();
    }

    private void GetUIComponents()
    {
        MainPanel = GameObject.Find("MainScreen").GetComponent<RectTransform>();
        LevelScroller = GameObject.Find("LevelScrollRect").GetComponent<RectTransform>();
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
        bMenuTransition = true;
        NavButtons.SetupNavButtons(MenuState);

        float AnimTimer = 0f;
        float StartX = Caves.position.x;
        float EndX = -GetXOffset();

        bool bLevelScrollRequired = MenuState == MenuStates.LevelSelect ? LevelScrollRequired() : false;

        while (AnimTimer <= AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float MovementRatio;
            if (bLevelScrollRequired)
            {
                MovementRatio = 1f - Mathf.Cos(Mathf.PI / 2f * (AnimTimer / AnimDuration));
            }
            else
            {
                MovementRatio = (1f - Mathf.Cos(Mathf.PI * (AnimTimer / AnimDuration))) / 2f;
            }
            float XPos = StartX - (StartX - EndX) * MovementRatio;
            Caves.position = new Vector3(XPos, 0f, 0f);
            MainPanel.position = new Vector3(MainMenuPosX + XPos, 0f, 0f);
            StatsPanel.position = new Vector3(StatsPosX + XPos, 0f, 0f);
            LevelScroller.position = new Vector3(LevelSelectPosX + XPos, 0f, 0f);

            yield return new WaitForSeconds(0.01f);
        }
        FinaliseMenuPosition();
        if (bLevelScrollRequired)
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
        LevelScroller.position = new Vector2((LevelScrollInitialPos - XOffset), 0f);

        if (MenuState == MenuStates.LevelSelect && !bLevelCaveStartStored)
        {
            bLevelCaveStartStored = true;
            LevelCaveStartX = LevelContentRect.position.x + LevelScrollInitialPos - LevelSelectPosX;
        }
        if (MenuState == MenuStates.MainMenu)
        {
            StartCoroutine("LeaveLevelAnim");
        }
        bMenuTransition = false;
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
                XOffset = LevelScrollInitialPos;
                break;
            case MenuStates.StatsScreen:
                XOffset = StatsPosX;
                break;
        }
        return XOffset;
    }

    private bool LevelScrollRequired()
    {
        const float MaxPosX = 7f;
        return (GetButtonPosX() > MaxPosX ? true : false);
    }

    private float GetButtonPosX()
    {
        RectTransform LvlButton = GameObject.Find("Lv" + CurrentLevel).GetComponent<RectTransform>();
        if (!LvlButton) { return 0; }
        float ButtonPosX = LvlButton.position.x - LevelContentRect.position.x;
        return ButtonPosX;
    }

    private void GotoCurrentLevel()
    {
        const float MaxPosX = 7f;
        float LvlButtonPosX = GetButtonPosX();
        if (LvlButtonPosX > MaxPosX)
        {
            float XShift = LvlButtonPosX - MaxPosX;
            float ContentScale = GameObject.Find("ScrollOverlay").GetComponent<RectTransform>().localScale.x;
            float NormalisedPosition = XShift / LevelContentRect.rect.width / ContentScale;
            StartCoroutine("GotoLevelAnim", NormalisedPosition);
        }
    }

    private IEnumerator GotoLevelAnim(float NormalisedPosition)
    {
        const float AnimDuration = 0.5f;
        float AnimTimer = 0f;
        float StartPos = 0f;
        float EndPos = NormalisedPosition;

        ScrollRect LevelScrollRect = LevelScroller.GetComponent<ScrollRect>();
        
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float MovementRatio = Mathf.Sin(Mathf.PI / 2f * (AnimTimer / AnimDuration));
            float Pos = StartPos - (StartPos - EndPos) * MovementRatio;
            LevelScrollRect.horizontalNormalizedPosition = Pos;

            yield return null;
        }
        LevelScrollRect.horizontalNormalizedPosition = NormalisedPosition;
    }

    private IEnumerator LeaveLevelAnim()
    {
        ScrollRect LevelScrollRect = LevelScroller.GetComponent<ScrollRect>();
        const float AnimDuration = 0.5f;
        float AnimTimer = 0f;
        float StartPos = LevelScrollRect.horizontalNormalizedPosition;
        float EndPos = 0f;
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float MovementRatio = Mathf.Sin(Mathf.PI / 2f * (AnimTimer / AnimDuration));
            float Pos = StartPos - (StartPos - EndPos) * MovementRatio;
            LevelScrollRect.horizontalNormalizedPosition = Pos;
            yield return null;
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
