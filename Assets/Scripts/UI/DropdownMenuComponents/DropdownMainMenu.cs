using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Levels = LevelProgressionHandler.Levels;

namespace ClumsyBat.UI.DropdownMenuComponents
{
    public class DropdownMainMenu : MenuScreenBase
    {
        public GameObject ContinueButtonObject;

        public GameObject NextBtn;
        public GameObject ShareBtn;
        public GameObject OptionsBtn;
        public GameObject MainMenuBtn;
        public GameObject RestartBtn;
        public GameObject ResumeBtn;
        public GameObject LevelSelectBtn;
        
        private LevelCompletionScreen completionScreen;

        private Levels nextLevel;

        private struct TextType
        {
            public RectTransform RectTransform;
            public Text Text;
        }
        private TextType _menuHeader;
        private TextType _subText;

        private void Start()
        {
            GetMenuObjects();
            CreateScreenComponents();
        }

        public void NextButtonPressed()
        {
            if (nextLevel == Levels.Unassigned)
            {
                LevelSelectPressed();
            }
            else
            {
                GameStatics.GameManager.LoadLevel(nextLevel);
            }
        }

        public void MenuButtonPressed()
        {
            GameStatics.GameManager.GotoMenuScene();
        }

        public void RestartButtonPressed()
        {
            GameStatics.Data.SaveData();
            GameStatics.GameManager.LoadLevel(GameStatics.LevelManager.Level);
        }

        public void OptionsButtonPressed()
        {
            GameStatics.UI.DropdownMenu.ShowOptions();
        }

        public void ResumeButtonPressed()
        {
            GameStatics.GameManager.ResumeGameFromMenu();
        }

        public void BackToMainPressed()
        {
            GameStatics.Data.SaveData();
            GameStatics.GameManager.GotoMenuScene();
        }

        public void LevelSelectPressed()
        {
            GameStatics.Data.SaveData();
            GameStatics.GameManager.GotoLevelSelect();
        }

        public void ShareButtonPressed()
        {
            // TODO this
        }

        public void HideAllButtons()
        {
            ContinueButtonObject.SetActive(false);
            NextBtn.SetActive(false);
            ShareBtn.SetActive(false);
            MainMenuBtn.SetActive(false);
            RestartBtn.SetActive(false);
            OptionsBtn.SetActive(false);
            ResumeBtn.SetActive(false);
            LevelSelectBtn.SetActive(false);
        }

        public void ShowLevelCompletion(Levels level, Levels nextLevel)
        {
            this.nextLevel = nextLevel;
            _menuHeader.Text.text = "LEVEL COMPLETE!";
            _subText.Text.text = Toolbox.Instance.LevelNames[GameStatics.LevelManager.Level];
            completionScreen.ShowLevelCompletion(level);
        }
        
        public void ContinueButton()
        {
            StartCoroutine(completionScreen.ShowMenuButtonsRoutine());
        }
        
        public void SetupPauseMenu()
        {
            NextBtn.SetActive(false);
            ShareBtn.SetActive(false);
            MainMenuBtn.SetActive(true);
            RestartBtn.SetActive(true);
            OptionsBtn.SetActive(true);
            ResumeBtn.SetActive(true);
            LevelSelectBtn.SetActive(false);

            PositionMenuBtn(RestartBtn, GetButtonPosX(1, 4));
            PositionMenuBtn(MainMenuBtn, GetButtonPosX(2, 4));
            PositionMenuBtn(OptionsBtn, GetButtonPosX(3, 4));
            PositionMenuBtn(ResumeBtn, GetButtonPosX(4, 4));

            _menuHeader.Text.text = "GAME PAUSED";
            _subText.Text.text = "Clumsy will wait for you...";
        }

        public void GameOverMenu()
        {
            NextBtn.SetActive(false);
            ShareBtn.SetActive(false);
            MainMenuBtn.SetActive(true);
            RestartBtn.SetActive(true);
            OptionsBtn.SetActive(false);
            ResumeBtn.SetActive(false);
            LevelSelectBtn.SetActive(true);

            PositionMenuBtn(RestartBtn, GetButtonPosX(1, 3));
            PositionMenuBtn(MainMenuBtn, GetButtonPosX(2, 3));
            PositionMenuBtn(LevelSelectBtn, GetButtonPosX(3, 3));

            _menuHeader.Text.text = "GAME OVER";
            _subText.Text.text = "Clumsy didn't make it...";
        }

        private void GetMenuObjects()
        {
            _menuHeader.RectTransform = GameObject.Find("MenuHeader").GetComponent<RectTransform>();
            _menuHeader.Text = _menuHeader.RectTransform.GetComponent<Text>();

            _subText.RectTransform = GameObject.Find("SubText").GetComponent<RectTransform>();
            _subText.Text = _subText.RectTransform.GetComponent<Text>();

            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            ContinueButtonObject.SetActive(false);
        }

        private void PositionMenuBtn(GameObject btn, float buttonXPos)
        {
            RectTransform btnRt = btn.GetComponent<RectTransform>();
            btnRt.position = new Vector3(buttonXPos, btnRt.position.y, btnRt.position.z);
        }

        private float GetButtonPosX(int btnNum, int numButtons)
        {
            float buttonPosX = 0;
            if (numButtons == 3)
            {
                switch (btnNum)
                {
                    case 1:
                        buttonPosX = -2;
                        break;
                    case 2:
                        buttonPosX = 0;
                        break;
                    case 3:
                        buttonPosX = 2;
                        break;
                }
            }
            else if (numButtons == 4)
            {
                switch (btnNum)
                {
                    case 1:
                        buttonPosX = -3f;
                        break;
                    case 2:
                        buttonPosX = -1f;
                        break;
                    case 3:
                        buttonPosX = 1f;
                        break;
                    case 4:
                        buttonPosX = 3f;
                        break;
                }
            }

            return buttonPosX + _menuHeader.RectTransform.position.x;
        }

        // TODO move helper functions or create animation
        public IEnumerator PopInObject(RectTransform rt)
        {
            float timer = 0f;
            float duration = 0.2f;

            Vector3 originalScale = rt.localScale;
            rt.gameObject.SetActive(true);
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                rt.localScale = Vector3.Lerp(Vector3.one * 0.1f, originalScale * 1.1f, timer / duration);
                yield return null;
            }

            timer = 0f;
            duration = 0.08f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                rt.localScale = Vector3.Lerp(originalScale * 1.1f, originalScale, timer / duration);
                yield return null;
            }
            rt.localScale = originalScale;
        }

        public IEnumerator PopOutObject(RectTransform rt)
        {
            float timer = 0f;
            float duration = 0.08f;

            Vector3 originalScale = rt.localScale;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                rt.localScale = Vector3.Lerp(originalScale, originalScale * 1.1f, timer / duration);
                yield return null;
            }

            timer = 0f;
            duration = 0.2f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                rt.localScale = Vector3.Lerp(originalScale * 1.1f, Vector3.one * 0.1f, timer / duration);
                yield return null;
            }
            rt.gameObject.SetActive(false);
            rt.localScale = originalScale;
        }

        public void ShowMenuButtons(GameObject[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                PositionMenuBtn(buttons[i], GetButtonPosX(i + 1, buttons.Length));
                StartCoroutine(PopInObject(buttons[i].GetComponent<RectTransform>()));
            }
        }
        
        private void CreateScreenComponents()
        {
            completionScreen = gameObject.AddComponent<LevelCompletionScreen>();

            completionScreen.Init(this);
        }
    }
}