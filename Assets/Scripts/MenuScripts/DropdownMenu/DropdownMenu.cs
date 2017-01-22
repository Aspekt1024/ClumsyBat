using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropdownMenu : MonoBehaviour {
    
    private RectTransform _menuPanel;
    private CanvasGroup _mainPanel;
    private CanvasGroup _statsPanel;
    private CanvasGroup _optionsPanel;
    private Image _menuBackPanel;

    public DropdownInGameMenu InGameMenu;
    public DropdownOptionsMenu OptionsMenu;
    public DropdownStatsMenu StatsMenu;

    private const float BounceDuration = 0.18f;
    private const float PanelDropAnimDuration = 0.30f;
    private bool _bKeepMenuAlpha;

    private const float MenuTopPos = 11f;
    private const float MenuBottomPos = 0f;

    void Awake ()
    {
        GetMenuObjects();
        if (_mainPanel) { SetCanvasActive(_mainPanel, true); }
        if (_optionsPanel) { SetCanvasActive(_optionsPanel, false); }
        if (_statsPanel) { SetCanvasActive(_statsPanel, false); }
    }
    
    private void SetCanvasActive(CanvasGroup canvasGrp, bool active)
    {
        if (canvasGrp)
        {
            canvasGrp.alpha = (active ? 1f : 0f);
            canvasGrp.interactable = active;
            canvasGrp.blocksRaycasts = active;
        }
    }

    public void ShowOptions()
    {
        OptionsMenu.InitialiseOptionsView();
        SetCanvasActive(_mainPanel, false);
        SetCanvasActive(_optionsPanel, true);
        SetCanvasActive(_statsPanel, false);
        OptionsMenu.SetToggleStates();
        StartCoroutine("PanelDropAnim", true);
    }

    public void ShowStats()
    {
        SetCanvasActive(_mainPanel, false);
        SetCanvasActive(_optionsPanel, false);
        SetCanvasActive(_statsPanel, true);
        StatsMenu.Show();
        StartCoroutine("PanelDropAnim", true);
    }

    public void Hide()
    {
        _menuPanel.position = new Vector3(_menuPanel.position.x, MenuTopPos, _menuPanel.position.z);
        _menuBackPanel.color = Color.clear;
    }

    public float RaiseMenu()
    {
        StartCoroutine("PanelDropAnim", false);
        return PanelDropAnimDuration;
    }

    private IEnumerator MenuSwitchAnim(bool bOptionsMenu)
    {
        _bKeepMenuAlpha = true;
        StartCoroutine("PanelDropAnim", false);
        yield return new WaitForSeconds(PanelDropAnimDuration + 0.4f);
        SetCanvasActive(_mainPanel, !bOptionsMenu);
        SetCanvasActive(_optionsPanel, bOptionsMenu);
        OptionsMenu.SetToggleStates();
        StartCoroutine("PanelDropAnim", true);
        yield return new WaitForSeconds(PanelDropAnimDuration + 2 * BounceDuration);
        _bKeepMenuAlpha = false;
    }
    
    private void GetMenuObjects()
    {
        if (!gameObject.activeSelf) { gameObject.SetActive(true); }
        SetCanvasActive(gameObject.GetComponent<CanvasGroup>(), true);

        _menuPanel = GameObject.Find("GameMenuPanel").GetComponent<RectTransform>();
        _menuBackPanel = GameObject.Find("BackPanel").GetComponent<Image>();
        RectTransform contentPanel = GameObject.Find("ContentPanel").GetComponent<RectTransform>();
        foreach (RectTransform rt in contentPanel)
        {
            switch (rt.name)
            {
                case "MainPanel":
                    _mainPanel = rt.GetComponent<CanvasGroup>();
                    break;
                case "OptionsPanel":
                    _optionsPanel = rt.GetComponent<CanvasGroup>();
                    break;
                case "StatsPanel":
                    _statsPanel = rt.GetComponent<CanvasGroup>();
                    break;
            }
        }
        if (_mainPanel) { InGameMenu = _mainPanel.GetComponent<DropdownInGameMenu>(); }
        if (_optionsPanel) { OptionsMenu = _optionsPanel.GetComponent<DropdownOptionsMenu>(); }
        if (_statsPanel) { StatsMenu = _statsPanel.GetComponent<DropdownStatsMenu>(); }
    }

    private IEnumerator PanelDropAnim(bool bEnteringScreen)
    {
        if (!bEnteringScreen)
        {
            StartCoroutine("Bounce", -0.7f);
            yield return new WaitForSeconds(BounceDuration);
        }

        const float animDuration = PanelDropAnimDuration;
        float animTimer = 0f;
        float startPos = (bEnteringScreen ? MenuTopPos : MenuBottomPos);
        float endPos = (bEnteringScreen ? MenuBottomPos : MenuTopPos);
        float startAlpha = (bEnteringScreen ? 0f : 0.65f);
        float endAlpha = (bEnteringScreen ? 0.65f : 0f);
        _menuPanel.position = new Vector3(_menuPanel.position.x, startPos, _menuPanel.position.z);

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            _menuPanel.position = new Vector3(_menuPanel.position.x, (startPos - (animTimer / animDuration) * (startPos - endPos)), _menuPanel.position.z);
            if (!_bKeepMenuAlpha)
            {
                _menuBackPanel.color = new Color(0f, 0f, 0f, startAlpha - (startAlpha - endAlpha) * (animTimer / animDuration));
            }
            yield return null;
        }

        if (bEnteringScreen)
        {
            StartCoroutine("Bounce", -1);
            yield return new WaitForSeconds(BounceDuration);
            StartCoroutine("Bounce", 0.3);
            yield return new WaitForSeconds(BounceDuration);
        }
        _menuPanel.position = new Vector3(_menuPanel.position.x, endPos, _menuPanel.position.z);
    }

    private IEnumerator Bounce(float yDist)
    {
        float animTimer = 0;
        const float animDuration = BounceDuration;

        float startY = _menuPanel.position.y;
        float midY = _menuPanel.position.y - yDist;

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            float animRatio = -Mathf.Sin(Mathf.PI * animTimer / animDuration);
            float yPos = startY - (animRatio) * (startY - midY);
            _menuPanel.position = new Vector3(_menuPanel.position.x, yPos, _menuPanel.position.z);
            yield return null;
        }
    }
}
