using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace ClumsyBat.UI
{
    public class GameHud : MonoBehaviour
    {
        private Text scoreText;
        private Text currencyText;
        private Text bigScoreText;
        private Text levelText;
        private Text resumeTimerText;
        private Text timeText;
        private DashIndicator dashIndicator;
        private CanvasGroup gameUICanvas;
        private Image mothImage;
        private GameObject pauseButton;

        private Vector3 currencyScale;
        private Vector3 collectedCurrencyScale;
        private RectTransform currencyRt;
        private RectTransform bigScoreTextRt;

        private RectTransform _resumeTimerRt;
        private int _resumeTime;
        private bool _bCooldownReady;

        private bool _bGamePaused;

        private Coroutine levelNameRoutine;
        private RectTransform levelTextRt;
        private Vector3 levelNameOriginalPosition;

        private void Awake()
        {
            GetTextObjects();
            levelNameOriginalPosition = levelTextRt.anchoredPosition;
        }

        private void Start()
        {
            scoreText.text = "0";
            UpdateTimer(0f);
            SetupUI();
        }

        public void Hide()
        {
            gameUICanvas.alpha = 0f;
            gameUICanvas.blocksRaycasts = false;
        }

        public void Show()
        {
            gameUICanvas.alpha = 1f;
            gameUICanvas.blocksRaycasts = true;
        }

        public void PauseButtonPressed()
        {
            GameStatics.GameManager.PauseGame();
            GameStatics.UI.DropdownMenu.ShowPauseMenu();
        }
        
        private void SetupUI()
        {
            bigScoreText.text = string.Empty;
            gameUICanvas.alpha = 0f;
            gameUICanvas.blocksRaycasts = false;
        }

        public void UpdateTimer(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60);
            if (minutes > 0)
            {
                seconds -= 60 * minutes;
                timeText.text = $"{minutes}:{seconds:n2}";
            }
            else
            {
                timeText.text = seconds.ToString("n2");
            }
        }

        public void SetCurrencyText(string text)
        {
            if (currencyText == null) return;
            if (currencyText.text == text) return;

            currencyText.gameObject.SetActive(true);
            UIObjectAnimator.Instance.PopObject(currencyRt);
            currencyText.text = text;
        }

        private void GetTextObjects()
        {
            foreach (RectTransform rt in GetComponent<RectTransform>())
            {
                if (rt.name == "UIElements")
                {
                    gameUICanvas = rt.GetComponent<CanvasGroup>();
                    foreach (RectTransform r in rt)
                    {
                        switch (r.name)
                        {
                            case "LevelText":
                                levelText = r.GetComponent<Text>();
                                levelTextRt = levelText.GetComponent<RectTransform>();
                                break;
                            case "ScoreText":
                                scoreText = r.GetComponent<Text>();
                                break;
                            case "CurrencyText":
                                currencyText = r.GetComponent<Text>();
                                currencyRt = r;
                                break;
                            case "BigPointsText":
                                bigScoreText = r.GetComponent<Text>();
                                bigScoreTextRt = r;
                                break;
                            case "TimeText":
                                timeText = r.GetComponent<Text>();
                                break;
                            case "MothImage":
                                mothImage = r.GetComponent<Image>();
                                break;
                            case "DashIndicator1":
                                dashIndicator = r.GetComponent<DashIndicator>();
                                break;
                            case "PauseButton":
                                pauseButton = r.gameObject;
                                break;
                        }
                    }
                }
                else if (rt.name == "ResumeTimerText")
                {
                    resumeTimerText = rt.GetComponent<Text>();
                    _resumeTimerRt = rt;
                }
            }
            // TODO remove these if not needed
            bigScoreTextRt.gameObject.SetActive(false);
            currencyRt.gameObject.SetActive(false);
        }

        public void SetResumeTimer(float timeRemaining)
        {
            if (resumeTimerText.enabled == false)
            {
                resumeTimerText.enabled = true;
            }
            if (_resumeTime != Mathf.CeilToInt(timeRemaining))
            {
                _resumeTime = Mathf.CeilToInt(timeRemaining);
                if (_resumeTime == 0)
                {
                    return;
                }
                UIObjectAnimator.Instance.PopObject(_resumeTimerRt);
                resumeTimerText.text = _resumeTime.ToString();
            }
        }

        public void HideResumeTimer()
        {
            resumeTimerText.enabled = false;
        }

        public void HidePauseButton() => pauseButton.SetActive(false);
        public void ShowPauseButton() => pauseButton.SetActive(true);
        
        public void SetStartText(string startText)
        {
            UIObjectAnimator.Instance.PopObject(_resumeTimerRt);
            resumeTimerText.text = startText;
        }

        public void SetLevelText(LevelProgressionHandler.Levels levelId)
        {
            // Note: this must be called by the LevelScript once the level has been set in GameData
            var level = (int)levelId;
            if (level == -1)
            {
                levelText.text = "Level: Endless";
            }
            else
            {
                levelText.text = "Level: " + Toolbox.Instance.LevelNames[levelId];
            }
        }

        public void SetCustomText(string text)
        {
            currencyText.text = text;
        }

        public void GamePaused(bool paused)
        {
            _bGamePaused = paused;
            gameUICanvas.alpha = paused ? 0f : 1f;
            gameUICanvas.blocksRaycasts = !paused;
        }

        public void SetupGame()
        {
            Hide();
        }

        public void StartGame()
        {
            Show();
            if (levelNameRoutine != null) StopCoroutine(levelNameRoutine);
            levelNameRoutine = StartCoroutine(ShowLevelNameRoutine());
            
            if (GameStatics.LevelManager.Level.ToString().Contains("Boss"))
            {
                HideStandardLevelElements();
            }
            else
            {
                ShowStandardLevelElements();
                CancelInvoke(nameof(SetScore));
                InvokeRepeating(nameof(SetScore), 0f, 0.1f);
            }

            gameUICanvas.alpha = 1f;
            gameUICanvas.blocksRaycasts = true;
        }

        public void SetCooldownTimer(float duration)
        {
            dashIndicator.StartCooldown(duration);
        }

        public void ShowCooldown(bool bShow)
        {
            if (bShow)
                dashIndicator.Enable();
            else
                dashIndicator.Disable();
        }

        private void SetScore()
        {
            if (_bGamePaused || !GameStatics.LevelManager.IsInPlayMode) return;
            scoreText.text = GameStatics.Data.GameState.Score.ToString();
        }

        private void HideStandardLevelElements()
        {
            scoreText.GetComponent<Text>().enabled = false;
            mothImage.enabled = false;
            currencyRt.GetComponent<Text>().enabled = false;
        }
        
        private void ShowStandardLevelElements()
        {
            scoreText.GetComponent<Text>().enabled = true;
            mothImage.enabled = true;
            currencyRt.GetComponent<Text>().enabled = true;
        }

        private IEnumerator ShowLevelNameRoutine()
        {
            var pos = levelNameOriginalPosition;
            var endPosX = pos.x;
            float startPos = Screen.width;
            const float duration = 1f;
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                pos.x = Mathf.Lerp(startPos, endPosX, timer / duration);
                levelTextRt.anchoredPosition = pos;
                yield return null;
            }
            
            yield return new WaitForSeconds(3f);

            timer = 0f; 
            endPosX = -Screen.width / 2f;
            var startPosX = pos.x;
            while (timer < duration)
            {
                yield return null;
                timer += Time.deltaTime;
                pos.x = Mathf.Lerp(startPosX, endPosX, timer / duration);
                levelTextRt.anchoredPosition = pos;
            }
        }
    }
}
