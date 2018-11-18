using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ClumsyBat.UI.DropdownMenuComponents;

using Levels = LevelProgressionHandler.Levels;

namespace ClumsyBat.UI
{
    public class DropdownMenu : MonoBehaviour
    {
        private RectTransform _menuPanel;
        private Image _menuBackPanel;

        public DropdownMainMenu MainMenu;
        public DropdownOptionsMenu OptionsMenu;
        public DropdownStatsMenu StatsMenu;

        private const float BounceDuration = 0.18f;
        private const float PanelDropAnimDuration = 0.30f;

        private const float MenuTopOffset = 11f;
        private const float MenuBottomOffset = 0f;

        private void Awake()
        {
            GetMenuObjects();
        }

        private void Start()
        {
            Hide();
            HideAllMenus();
            GameStatics.Camera.OnCameraChanged += CameraChanged;
        }

        private void OnDestroy()
        {
            GameStatics.Camera.OnCameraChanged -= CameraChanged;
        }

        public void ShowLevelCompletion(Levels level, Levels nextLevel)
        {
            HideAllMenus();
            MainMenu.ShowScreen();
            MainMenu.ShowLevelCompletion(level, nextLevel);
        }
        
        public void ShowOptions()
        {
            HideAllMenus();
            OptionsMenu.ShowScreen();
            StartCoroutine(PanelDropAnim(true));
        }

        public void ShowStats()
        {
            HideAllMenus();
            StatsMenu.ShowScreen(); StartCoroutine(PanelDropAnim(true));
        }
        
        public void ShowPauseMenu()
        {
            MainMenu.PauseMenu();
        }

        public void ShowGameOverMenu()
        {
            StartCoroutine(PanelDropAnim(true));
            HideAllMenus();
            MainMenu.ShowScreen();
        }
        
        public void Hide()
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        private void Show()
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }
        
        public IEnumerator DropMenuRoutine()
        {
            Show();
            yield return StartCoroutine(PanelDropAnim(true));
        }

        public IEnumerator RaiseMenuRoutine()
        {
            yield return StartCoroutine(PanelDropAnim(false));
        }

        private IEnumerator PanelDropAnim(bool bEnteringScreen) // todo create separate coroutine for raising and dropping
        {
            if (!bEnteringScreen)
            {
                StartCoroutine(Bounce(-0.7f));
                yield return new WaitForSeconds(BounceDuration);
            }

            const float animDuration = PanelDropAnimDuration;
            float yPosTop = GameStatics.Camera.CurrentCamera.transform.position.y + MenuTopOffset;
            float yPosBottom = GameStatics.Camera.CurrentCamera.transform.position.y;
            float animTimer = 0f;
            float startPos = (bEnteringScreen ? yPosTop : yPosBottom);
            float endPos = (bEnteringScreen ? yPosBottom : yPosTop);
            float startAlpha = (bEnteringScreen ? 0f : 0.65f);
            float endAlpha = (bEnteringScreen ? 0.65f : 0f);
            _menuPanel.position = new Vector3(_menuPanel.position.x, startPos, _menuPanel.position.z);

            while (animTimer < animDuration)
            {
                animTimer += Time.deltaTime;
                _menuPanel.position = new Vector3(_menuPanel.position.x, (startPos - (animTimer / animDuration) * (startPos - endPos)), _menuPanel.position.z);
                _menuBackPanel.color = new Color(0f, 0f, 0f, startAlpha - (startAlpha - endAlpha) * (animTimer / animDuration));
                yield return null;
            }

            if (bEnteringScreen)
            {
                StartCoroutine(Bounce(-1));
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
    
        private void HideAllMenus()
        {
            MainMenu.HideScreen();
            OptionsMenu.HideScreen();
            StatsMenu.HideScreen();
        }

        private void GetMenuObjects()
        {
            if (!gameObject.activeSelf) { gameObject.SetActive(true); }
            Show();

            _menuPanel = GameObject.Find("GameMenuPanel").GetComponent<RectTransform>();
            _menuBackPanel = GameObject.Find("BackPanel").GetComponent<Image>();
            RectTransform contentPanel = GameObject.Find("ContentPanel").GetComponent<RectTransform>();

            foreach (RectTransform rt in contentPanel)
            {
                switch (rt.name)
                {
                    case "MainPanel":
                        MainMenu = rt.GetComponent<DropdownMainMenu>();
                        break;
                    case "OptionsPanel":
                        OptionsMenu = rt.GetComponent<DropdownOptionsMenu>();
                        break;
                    case "StatsPanel":
                        StatsMenu = rt.GetComponent<DropdownStatsMenu>();
                        break;
                }
            }
        }

        private void CameraChanged(Camera newCamera)
        {
            GetComponent<Canvas>().worldCamera = newCamera;
        }
    }
}

