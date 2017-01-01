using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropdownMenu : MonoBehaviour {
    
    private RectTransform MenuPanel = null;
    private CanvasGroup MainPanel = null;
    private CanvasGroup StatsPanel = null;
    private CanvasGroup OptionsPanel = null;
    private Image MenuBackPanel = null;

    public DropdownInGameMenu InGameMenu;
    public DropdownOptionsMenu OptionsMenu;
    public DropdownStatsMenu StatsMenu;

    private const float BounceDuration = 0.18f;
    private const float PanelDropAnimDuration = 0.30f;
    private bool bKeepMenuAlpha = false;

    private const float MenuTopPos = 11f;
    private const float MenuBottomPos = 0f;

    void Awake ()
    {
        GetMenuObjects();
        if (MainPanel) { SetCanvasActive(MainPanel, true); }
        if (OptionsPanel) { SetCanvasActive(OptionsPanel, false); }
        if (StatsPanel) { SetCanvasActive(StatsPanel, false); }
    }
    
    private void SetCanvasActive(CanvasGroup CanvasGrp, bool Active)
    {
        if (CanvasGrp)
        {
            CanvasGrp.alpha = (Active ? 1f : 0f);
            CanvasGrp.interactable = Active;
            CanvasGrp.blocksRaycasts = Active;
        }
    }

    public void ShowOptions()
    {
        SetCanvasActive(MainPanel, false);
        SetCanvasActive(OptionsPanel, true);
        SetCanvasActive(StatsPanel, false);
        OptionsMenu.SetToggleStates();
        StartCoroutine("PanelDropAnim", true);
    }

    public void ShowStats()
    {
        SetCanvasActive(MainPanel, false);
        SetCanvasActive(OptionsPanel, false);
        SetCanvasActive(StatsPanel, true);
        StatsMenu.Show();
        StartCoroutine("PanelDropAnim", true);
    }

    public void Hide()
    {
        MenuPanel.position = new Vector3(MenuPanel.position.x, MenuTopPos, MenuPanel.position.z);
        MenuBackPanel.color = Color.clear;
    }

    public void RaiseMenu()
    {
        StartCoroutine("PanelDropAnim", false);
    }

    private IEnumerator MenuSwitchAnim(bool bOptionsMenu)
    {
        bKeepMenuAlpha = true;
        StartCoroutine("PanelDropAnim", false);
        yield return new WaitForSeconds(PanelDropAnimDuration + 0.4f);
        SetCanvasActive(MainPanel, !bOptionsMenu);
        SetCanvasActive(OptionsPanel, bOptionsMenu);
        OptionsMenu.SetToggleStates();
        StartCoroutine("PanelDropAnim", true);
        yield return new WaitForSeconds(PanelDropAnimDuration + 2 * BounceDuration);
        bKeepMenuAlpha = false;
    }
    
    private void GetMenuObjects()
    {
        if (!gameObject.activeSelf) { gameObject.SetActive(true); }
        SetCanvasActive(gameObject.GetComponent<CanvasGroup>(), true);

        MenuPanel = GameObject.Find("GameMenuPanel").GetComponent<RectTransform>();
        MenuBackPanel = GameObject.Find("BackPanel").GetComponent<Image>();
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
                case "StatsPanel":
                    StatsPanel = RT.GetComponent<CanvasGroup>();
                    break;
            }
        }
        if (MainPanel) { InGameMenu = MainPanel.GetComponent<DropdownInGameMenu>(); }
        if (OptionsPanel) { OptionsMenu = OptionsPanel.GetComponent<DropdownOptionsMenu>(); }
        if (StatsPanel) { StatsMenu = StatsPanel.GetComponent<DropdownStatsMenu>(); }
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
        float StartPos = (bEnteringScreen ? MenuTopPos : MenuBottomPos);
        float EndPos = (bEnteringScreen ? MenuBottomPos : MenuTopPos);
        float StartAlpha = (bEnteringScreen ? 0f : 0.65f);
        float EndAlpha = (bEnteringScreen ? 0.65f : 0f);
        MenuPanel.position = new Vector3(MenuPanel.position.x, StartPos, MenuPanel.position.z);

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
            StartCoroutine("Bounce", 0.4);
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
