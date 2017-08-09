using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScroller : MonoBehaviour {

    private RectTransform scrollOverlay;

    private RectTransform mainScreen;
    private RectTransform levelScroller;
    private RectTransform levelContentRect;
    private NavButtonHandler navButtons;
    private ScrollRect levelScrollRect;
    private KeyPointsHandler keyPoints;

    private Transform Caves;
    private Transform MidBG;

    private const float TileSizeX = 19.2f;
    private const float TransitionDuration = 1.0f;

    private float LevelScrollInitialPos;
    private const float LevelSelectPosX = 1.5f * TileSizeX;
    private const float MainMenuPosX = 0f;
    private const float StatsPosX = -TileSizeX;
    private const float UpgradesPosX = -2f * TileSizeX;

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
    private bool bMenuAtFullSpeed = false;

    void Awake()
    {
        GetUIComponents();
        InitialiseMenu();
        SetupScreenSizing();
        SetupNavButtonHandler();
        keyPoints = GameObject.FindGameObjectWithTag("Scripts").GetComponent<KeyPointsHandler>();
        levelScroller.position = keyPoints.LevelMapStart.transform.position;
    }

	void Update ()
    {
        float xDiff = keyPoints.MainMenuCamPoint.transform.position.x - mainScreen.position.x;
        mainScreen.position += Vector3.right * xDiff;
        levelScroller.position += Vector3.right * xDiff;

        return;

        if (MenuState == MenuStates.LevelSelect && !bMenuTransition)
        {
            Caves.position = new Vector2(levelContentRect.position.x - LevelCaveStartX - LevelSelectPosX, 0f);
        }
        MidBG.position = new Vector3(Caves.position.x * 0.5f, 0f, MidBG.position.z);
	}

    public void SetCurrentLevel(int Level)
    {
        CurrentLevel = Level;
        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            JumpToCurrentLevel();
        }
    }

    private void SetupScreenSizing()
    {
        bLevelCaveStartStored = false;
    }

    private void SetupNavButtonHandler()
    {
        GameObject RuntimeScripts = GameObject.Find("Runtime Scripts");
        navButtons = RuntimeScripts.AddComponent<NavButtonHandler>();
        navButtons.SetupNavButtons(MenuState);
    }

    private void InitialiseMenu()
    {
        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            MenuState = MenuStates.LevelSelect;
        }

        LevelScrollInitialPos = levelScroller.position.x;
        FinaliseMenuPosition();
    }

    private void JumpToCurrentLevel()
    {
        GotoCurrentLevel(Instantly: true);
    }

    private void GetUIComponents()
    {
        mainScreen = GameObject.Find("MainScreen").GetComponent<RectTransform>();
        levelScroller = GameObject.Find("LevelScrollRect").GetComponent<RectTransform>();
        levelContentRect = GameObject.Find("Content").GetComponent<RectTransform>();
        levelScrollRect = levelScroller.GetComponent<ScrollRect>();

        GetBackgrounds();
    }

    public void MainMenu()
    {
    //    if (MenuState == MenuStates.LevelSelect)
    //    {
    //        StartCoroutine("LeaveLevelAnim");
    //    }
    //    else
    //    {
    //        MenuState = MenuStates.MainMenu;
    //        StartCoroutine("MoveMenu");
    //    }
    }

    public void LevelSelect()
    {
        //MenuState = MenuStates.LevelSelect;
        
        //StartCoroutine(MoveMenu(keyPoints.LevelCamPoint));
    }

    public float StatsScreen()
    {
        MenuState = MenuStates.StatsScreen;
        StartCoroutine("MoveMenu");
        return TransitionDuration;
    }

    private IEnumerator MoveMenu(GameObject pointObj)
    {
        bMenuTransition = true;
        navButtons.SetupNavButtons(MenuState);

        float AnimTimer = 0f;
        float StartX = Caves.position.x;
        float EndX = -GetXOffset();

        bool bLevelScrollRequired = MenuState == MenuStates.LevelSelect ? LevelScrollRequired() : false;

        while (AnimTimer <= TransitionDuration)
        {
            AnimTimer += Time.deltaTime;
            float MovementRatio;
            if (bLevelScrollRequired)
            {
                MovementRatio = 1f - Mathf.Cos(Mathf.PI / 2f * (AnimTimer / TransitionDuration));
            }
            else if (bMenuAtFullSpeed)
            {
                MovementRatio = Mathf.Sin(Mathf.PI / 2f * (AnimTimer / TransitionDuration));
            }
            else
            {
                MovementRatio = (1f - Mathf.Cos(Mathf.PI * (AnimTimer / TransitionDuration))) / 2f;
            }
            float XPos = StartX - (StartX - EndX) * MovementRatio;
            Caves.position = new Vector3(XPos, 0f, 0f);
            mainScreen.position = new Vector3(MainMenuPosX + XPos, 0f, 0f);
            levelScroller.position = new Vector3(LevelScrollInitialPos + XPos, 0f, 0f);

            yield return new WaitForSeconds(0.01f);
        }
        bMenuAtFullSpeed = false;
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
        mainScreen.position = new Vector2(MainMenuPosX - XOffset, 0f);
        levelScroller.position = new Vector2((LevelScrollInitialPos - XOffset), 0f);
        if (MenuState == MenuStates.LevelSelect && !bLevelCaveStartStored)
        {
            bLevelCaveStartStored = true;
            LevelCaveStartX = levelContentRect.position.x + LevelScrollInitialPos - LevelSelectPosX;
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
        GameObject lvlButton = GameObject.Find("Lv" + CurrentLevel);
        if (!lvlButton) { return 0; }
        RectTransform lvlButtonRt = lvlButton.GetComponent<RectTransform>();
        float ButtonPosX = lvlButtonRt.position.x - levelContentRect.position.x;
        return ButtonPosX;
    }

    private void GotoCurrentLevel(bool Instantly = false)
    {
        const float MaxPosX = 7f;
        float LvlButtonPosX = GetButtonPosX();
        if (LvlButtonPosX > MaxPosX)
        {
            float XShift = LvlButtonPosX - MaxPosX;
            float ContentScale = GameObject.Find("ScrollOverlay").GetComponent<RectTransform>().localScale.x;
            float NormalisedPosition = XShift / levelContentRect.rect.width / ContentScale;
            if (!Instantly)
            {
                StartCoroutine("GotoLevelAnim", NormalisedPosition);
            }
            else
            {
                levelScrollRect.horizontalNormalizedPosition = NormalisedPosition;
            }
        }
    }

    private IEnumerator GotoLevelAnim(float NormalisedPosition)
    {
        const float AnimDuration = 0.5f;
        float AnimTimer = 0f;
        float StartPos = 0f;
        float EndPos = NormalisedPosition;
        
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float MovementRatio = Mathf.Sin(Mathf.PI / 2f * (AnimTimer / AnimDuration));
            float Pos = StartPos - (StartPos - EndPos) * MovementRatio;
            levelScrollRect.horizontalNormalizedPosition = Pos;

            yield return null;
        }
        levelScrollRect.horizontalNormalizedPosition = NormalisedPosition;
    }

    private IEnumerator LeaveLevelAnim()
    {
        ScrollRect LevelScrollRect = levelScroller.GetComponent<ScrollRect>();

        const float AnimDuration = 0.5f;
        float AnimTimer = 0f;
        float StartPos = LevelScrollRect.horizontalNormalizedPosition;
        float EndPos = 0f;
        
        if (LevelScrollRect.horizontalNormalizedPosition == 0f)
        {
            AnimTimer = AnimDuration;
        }

        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float MovementRatio = 1f - Mathf.Cos(Mathf.PI / 2f * (AnimTimer / AnimDuration));
            float Pos = StartPos - (StartPos - EndPos) * MovementRatio;
            LevelScrollRect.horizontalNormalizedPosition = Pos;
            yield return null;
        }
        bMenuAtFullSpeed = true;
        MenuState = MenuStates.MainMenu;
        StartCoroutine("MoveMenu");
    }

    private void GetBackgrounds()
    {
        Caves = GameObject.Find("CavePieces").GetComponent<RectTransform>();
        MidBG = GameObject.Find("MidBackground").GetComponent<Transform>();
    }

    public float GetAnimDuration()
    {
        return TransitionDuration;
    }
}
