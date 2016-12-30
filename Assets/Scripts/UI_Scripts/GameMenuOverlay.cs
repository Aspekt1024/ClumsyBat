using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameMenuOverlay : MonoBehaviour {

    private StatsHandler Stats;

    private RectTransform MenuPanel = null;
    private TextType MenuHeader;
    private TextType SubText;
    
    private GameObject NextBtn = null;
    private GameObject ShareBtn = null;
    private GameObject OptionsBtn = null;
    private GameObject MainMenuBtn = null;
    private GameObject RestartBtn = null;
    private GameObject ResumeBtn = null;

    private Animator ToggleMusic = null;
    private Animator ToggleSFX = null;
    private Animator ToggleToolTips = null;

    private CanvasGroup MainPanel = null;
    private CanvasGroup OptionsPanel = null;
    private CanvasGroup LoadingOverlay = null;

    private struct TextType
    {
        public RectTransform RectTransform;
        public Text Text;
    }

    enum MenuState
    {
        Pause,
        GameOver,
        Win
    }

    private const float BounceDuration = 0.18f;
    private const float PanelDropAnimDuration = 0.28f;
    private bool bKeepMenuAlpha = false;

    void Awake()
    {
        LoadingOverlay = GameObject.Find("LoadScreen").GetComponent<CanvasGroup>();
        SetCanvasActive(LoadingOverlay, true);
    }

    // Use this for initialization
    void Start ()
    {
        Stats = FindObjectOfType<StatsHandler>();
        GetMenuObjects();

        SetCanvasActive(MainPanel, true);
        SetCanvasActive(OptionsPanel, false);
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}


    private void SetCanvasActive(CanvasGroup CanvasGrp, bool Active)
    {
        CanvasGrp.alpha = (Active ? 1f : 0f);
        CanvasGrp.interactable = Active;
        CanvasGrp.blocksRaycasts = Active;
    }

    /// <summary>
    /// Button behaviour described below
    /// </summary>

    public void MenuButtonPressed()
    {
        Stats.SaveStats();
        SceneManager.LoadScene("Play");
    }

    public void RestartButtonPressed()
    {
        Stats.SaveStats();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OptionsButtonPressed()
    {
        Stats.SaveStats(); // Just because
        StartCoroutine("MenuSwitchAnim", true);
    }

    public void BackToMainPressed()
    {
        Stats.SaveStats(); // Just because
        StartCoroutine("MenuSwitchAnim", false);
    }

    public void ToggleMusicPressed()
    {
        Stats.Settings.Music = !Stats.Settings.Music;
        ToggleMusic.Play(Stats.Settings.Music ? "MusicON" : "MusicOFF");
    }

    public void ToggleSFXPressed()
    {
        Stats.Settings.SFX = !Stats.Settings.SFX;
        ToggleSFX.Play(Stats.Settings.SFX ? "SFXON" : "SFXOFF");
    }

    private void SetToggleStates()
    {
        string MusicState = (Stats.Settings.Music ? "MusicON" : "MusicOFF");
        string SFXState = (Stats.Settings.SFX ? "SFXON" : "SFXOFF");
        string TooltipsState = (Stats.Settings.Tooltips ? "SFXON" : "SFXOFF");

        ToggleMusic.Play(MusicState);
        ToggleSFX.Play(SFXState);
        //ToggleTooltips.Play(TooltipsState);
    }

    private IEnumerator MenuSwitchAnim(bool bOptionsMenu)
    {
        bKeepMenuAlpha = true;
        StartCoroutine("PanelDropAnim", false);
        yield return new WaitForSeconds(PanelDropAnimDuration + 0.4f);
        SetCanvasActive(MainPanel, !bOptionsMenu);
        SetCanvasActive(OptionsPanel, bOptionsMenu);
        SetToggleStates();
        StartCoroutine("PanelDropAnim", true);
        yield return new WaitForSeconds(PanelDropAnimDuration + 2 * BounceDuration);
        bKeepMenuAlpha = false;
    }

    private void GetMenuObjects()
    {
        if (!gameObject.activeSelf) { gameObject.SetActive(true); }

        RectTransform ContentPanel = GameObject.Find("ContentPanel").GetComponent<RectTransform>();
        foreach (RectTransform RT in ContentPanel)
        {
            switch (RT.name)
            {
                case "MainPanel":
                    MainPanel = RT.GetComponent<CanvasGroup>();
                    break;
                case "OptionsPanel":
                    OptionsPanel = RT.GetComponent<CanvasGroup>();
                    break;
            }
        }
        GetMenuButtons();

        MenuPanel = GameObject.Find("GameMenuPanel").GetComponent<RectTransform>();

        MenuHeader.RectTransform = GameObject.Find("MenuHeader").GetComponent<RectTransform>();
        MenuHeader.Text = MenuHeader.RectTransform.GetComponent<Text>();

        SubText.RectTransform = GameObject.Find("SubText").GetComponent<RectTransform>();
        SubText.Text = SubText.RectTransform.GetComponent<Text>();
        
        ToggleMusic = GameObject.Find("ToggleMusic").GetComponent<Animator>();
        ToggleSFX = GameObject.Find("ToggleSFX").GetComponent<Animator>();
        //ToggleToolTips = GameObject.Find("ToggleToolTips").GetComponent<RectTransform>();
        //TODO NextLevelButton
    }

    private void GetMenuButtons()
    {
        foreach (RectTransform RT in MainPanel.GetComponent<RectTransform>())
        {
            switch (RT.name)
            {
                case "ShareButton":
                    ShareBtn = RT.GetComponent<GameObject>();
                    break;
                case "NextButton":
                    NextBtn = RT.GetComponent<GameObject>();
                    break;
                case "OptionButton":
                    OptionsBtn = RT.GetComponent<GameObject>();
                    break;
                case "MainMenuButton":
                    MainMenuBtn = RT.GetComponent<GameObject>();
                    break;
                case "RestartButton":
                    RestartBtn = RT.GetComponent<GameObject>();
                    break;
                case "ResumeButton":
                    ResumeBtn = RT.GetComponent<GameObject>();
                    break;
            }
        }
    }

    public void GameOver()
    {
        gameObject.SetActive(true);
        if (MenuHeader.RectTransform == null)
        {
            GetMenuObjects();
        }
        MenuHeader.Text.text = "GAME OVER";
        SubText.Text.text = "Clumsy didn't make it...";
        StartCoroutine("PanelDropAnim", true);
    }

    public void RemoveLoadingOverlay()
    {
        SetCanvasActive(LoadingOverlay, false);
    }

    public void PauseGame()
    {
        gameObject.SetActive(true);
        MenuHeader.Text.text = "GAME PAUSED";
        SubText.Text.text = "Clumsy will wait for you...";
        StartCoroutine("PanelDropAnim", true);
    }

    public void WinGame()
    {
        gameObject.SetActive(true);
        MenuHeader.Text.text = "LEVEL COMPLETE!";
        SubText.Text.text = "Clumsy made it!";
        StartCoroutine("PanelDropAnim", true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator PanelDropAnim(bool bEnteringScreen)
    {
        if (!bEnteringScreen)
        {
            StartCoroutine("Bounce", -0.7f);
            yield return new WaitForSeconds(BounceDuration);
        }

        const float AnimDuration = PanelDropAnimDuration;
        float AnimTimer = 0f;
        float StartPos = (bEnteringScreen ? 10f : 0f);
        float EndPos = (bEnteringScreen ? 0f : 10f);
        float StartAlpha = (bEnteringScreen ? 0f : 0.65f);
        float EndAlpha = (bEnteringScreen ? 0.65f : 0f);
        MenuPanel.position = new Vector3(MenuPanel.position.x, StartPos, MenuPanel.position.z);
        Image MenuBackPanel = GameObject.Find("BackPanel").GetComponent<Image>();

        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            MenuPanel.position = new Vector3(MenuPanel.position.x, (StartPos - (AnimTimer / AnimDuration) * (StartPos - EndPos)), MenuPanel.position.z);
            if (!bKeepMenuAlpha)
            {
                MenuBackPanel.color = new Color(0f, 0f, 0f, StartAlpha - (StartAlpha - EndAlpha) * (AnimTimer / AnimDuration));
            }
            yield return null;
        }

        if (bEnteringScreen)
        {
            StartCoroutine("Bounce", -1);
            yield return new WaitForSeconds(BounceDuration);
            StartCoroutine("Bounce", 0.5);
            yield return new WaitForSeconds(BounceDuration);
            StartCoroutine("Bounce", -0.2);
            yield return new WaitForSeconds(BounceDuration);
        }
        MenuPanel.position = new Vector3(MenuPanel.position.x, EndPos, MenuPanel.position.z);
    }

    private IEnumerator Bounce(float YDist)
    {
        float AnimTimer = 0;
        const float AnimDuration = BounceDuration;

        float StartY = MenuPanel.position.y;
        float MidY = MenuPanel.position.y - YDist;

        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float AnimRatio = -Mathf.Sin(Mathf.PI * AnimTimer / AnimDuration);
            float YPos = StartY - (AnimRatio) * (StartY - MidY);
            MenuPanel.position = new Vector3(MenuPanel.position.x, YPos, MenuPanel.position.z);
            yield return null;
        }
    }
}
