using System.Collections;
using UnityEngine;

using Levels = LevelProgressionHandler.Levels;
using AchievementStatus = ClumsyBat.LevelManagement.LevelAchievementHandler.AchievementStatus;
using ClumsyBat.LevelManagement;

namespace ClumsyBat.UI.DropdownMenuComponents
{
    public class LevelCompletionScreen : MonoBehaviour
    {
        private DropdownMainMenu mainMenu;

        private GameObject starsContainer;
        private MothStar[] stars;

        public LevelCompleteStats LevelCompleteStats;

        public void Init(DropdownMainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
            LevelCompleteStats = FindObjectOfType<LevelCompleteStats>();

            foreach (Transform tf in GetComponentsInChildren<Transform>())
            {
                if (tf.name == "Stats")
                {
                    starsContainer = tf.gameObject;
                }
            }

            stars = new MothStar[3];
            foreach (Transform tf in starsContainer.transform)
            {
                if (tf.name.Contains("1")) stars[0] = tf.GetComponent<MothStar>();
                if (tf.name.Contains("2")) stars[1] = tf.GetComponent<MothStar>();
                if (tf.name.Contains("3")) stars[2] = tf.GetComponent<MothStar>();
            }
            starsContainer.GetComponent<CanvasGroup>().alpha = 0f;
            starsContainer.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void ShowLevelCompletion(Levels level)
        {
            StartCoroutine(LevelCompleteRoutine());
        }

        private IEnumerator LevelCompleteRoutine()
        {
            mainMenu.HideAllButtons();

            AchievementStatus[] achievements = LevelAchievementHandler.GetLevelAchievements();

            LoadStars(achievements);
            if (GameStatics.LevelManager.Level.ToString().Contains("Boss"))
            {
                LevelCompleteStats.ShowBossLevelStats();
            }
            else
            {
                LevelCompleteStats.ShowNormalLevelStats();
            }

            yield return GameStatics.UI.DropdownMenu.DropMenuRoutine();
            StartCoroutine(mainMenu.PopInObject(mainMenu.ContinueButtonObject.GetComponent<RectTransform>()));
            yield return StartCoroutine(ShowStars(achievements));
        }

        public IEnumerator ShowMenuButtonsRoutine()
        {
            StartCoroutine(mainMenu.PopOutObject(mainMenu.ContinueButtonObject.GetComponent<RectTransform>()));
            LevelCompleteStats.PopOutAllObjects();
            yield return StartCoroutine(PopOutStars());

            mainMenu.ShowMenuButtons(new GameObject[]
            {
                mainMenu.RestartBtn,
                mainMenu.MainMenuBtn,
                mainMenu.ShareBtn,
                mainMenu.NextBtn
            });
        }

        private void LoadStars(AchievementStatus[] achievements)
        {
            starsContainer.GetComponent<CanvasGroup>().alpha = 1f;
            starsContainer.GetComponent<CanvasGroup>().blocksRaycasts = true;
            for (int i = 0; i < 3; i++)
            {
                if (achievements[i] == AchievementStatus.Achieved)
                    stars[i].SetActive();
                else
                    stars[i].SetInactive();

                stars[i].gameObject.SetActive(true);
            }

            if (GameStatics.LevelManager.Level.ToString().Contains("Boss"))
            {
                stars[0].SetText("Level Complete");
                stars[1].SetText("Under 2 Damage");
                stars[2].SetText("No Damage Taken");
            }
            else
            {
                stars[0].SetText("All Moths Collected");
                stars[1].SetText("No Damage Taken");
                stars[2].SetText("Score Over " + GameStatics.LevelManager.ScoreToBeat);
            }
        }

        private IEnumerator PopOutStars()
        {
            StartCoroutine(mainMenu.PopOutObject(stars[0].GetComponent<RectTransform>()));
            yield return new WaitForSeconds(0.05f);
            StartCoroutine(mainMenu.PopOutObject(stars[1].GetComponent<RectTransform>()));
            yield return new WaitForSeconds(0.05f);
            yield return StartCoroutine(mainMenu.PopOutObject(stars[2].GetComponent<RectTransform>()));
        }

        private IEnumerator ShowStars(AchievementStatus[] achievements)
        {
            starsContainer.SetActive(true);
            mainMenu.ContinueButtonObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < achievements.Length; i++)
            {
                if (achievements[i] == AchievementStatus.NewAchievement)
                {
                    yield return StartCoroutine(stars[i].AnimateToActive());
                    // TODO play sound
                }
            }
        }
    }
}
