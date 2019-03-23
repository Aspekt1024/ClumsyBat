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

        private enum States
        {
            Hidden, Visible
        }
        private States state;

        private void Awake()
        {
            state = States.Hidden;
            GetMenuObjects();
        }

        private void Start()
        {
            HideAll();
            HideImmediate();
            GameStatics.Camera.OnCameraChanged += CameraChanged;
        }

        private void OnDestroy()
        {
            GameStatics.Camera.OnCameraChanged -= CameraChanged;
        }

        public void ReturnButtonClicked()
        {
            if (GameStatics.GameManager.IsInMenu)
            {
                GameStatics.UI.MainMenuTransitions.GotoMainMenuArea();
            }
            else
            {
                ShowPauseMenu();
            }
        }

        public void ShowLevelCompletion(Levels level, Levels nextLevel)
        {
            HideAll();
            MainMenu.ShowScreen();
            ShowDropdownMenu();
            MainMenu.ShowLevelCompletion(level, nextLevel);
        }
        
        public void ShowOptions()
        {
            HideAll();
            OptionsMenu.ShowScreen();
            ShowDropdownMenu();
        }

        public void ShowStats()
        {
            HideAll();
            StatsMenu.ShowScreen();
            ShowDropdownMenu();
        }
        
        public void ShowPauseMenu()
        {
            HideAll();
            MainMenu.SetupPauseMenu();
            MainMenu.ShowScreen();
            ShowDropdownMenu();
        }
        
        public void ShowGameOverMenu()
        {
            HideAll();
            MainMenu.ShowScreen();
            ShowDropdownMenu();
        }
        
        public void HideImmediate()
        {
            state = States.Hidden;
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        
        public IEnumerator DropMenuRoutine()
        {
            yield return StartCoroutine(PanelDropAnim(true));
        }

        public IEnumerator RaiseMenuRoutine()
        {
            yield return StartCoroutine(PanelDropAnim(false));
        }

        private void ShowDropdownMenu()
        {
            if (state == States.Hidden)
            {
                state = States.Visible;
                StartCoroutine(DropMenuRoutine());
                CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }
        }

        private IEnumerator PanelDropAnim(bool bEnteringScreen) // todo create separate coroutine for raising and dropping
        {
            if (!bEnteringScreen)
            {
                if (state == States.Hidden) yield break;

                state = States.Hidden;
                StartCoroutine(Bounce(-0.7f));
                yield return new WaitForSecondsRealtime(BounceDuration);
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
                animTimer += Time.unscaledDeltaTime;
                _menuPanel.position = new Vector3(_menuPanel.position.x, (startPos - (animTimer / animDuration) * (startPos - endPos)), _menuPanel.position.z);
                _menuBackPanel.color = new Color(0f, 0f, 0f, startAlpha - (startAlpha - endAlpha) * (animTimer / animDuration));
                yield return null;
            }

            if (bEnteringScreen)
            {
                StartCoroutine(Bounce(-1));
                yield return new WaitForSecondsRealtime(BounceDuration);
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
                animTimer += Time.unscaledDeltaTime;
                float animRatio = -Mathf.Sin(Mathf.PI * animTimer / animDuration);
                float yPos = startY - (animRatio) * (startY - midY);
                _menuPanel.position = new Vector3(_menuPanel.position.x, yPos, _menuPanel.position.z);
                yield return null;
            }
        }
    
        private void HideAll()
        {
            MainMenu.HideScreen();
            OptionsMenu.HideScreen();
            StatsMenu.HideScreen();
        }

        private void GetMenuObjects()
        {
            if (!gameObject.activeSelf) { gameObject.SetActive(true); }

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

