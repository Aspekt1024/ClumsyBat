using System.Collections;
using UnityEngine;

namespace ClumsyBat.Menu
{
    public class MenuButtonScripts : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private RectTransform playButton;
        [SerializeField] private RectTransform optionsButton;
        [SerializeField] private RectTransform statsButton;
#pragma warning restore 649

        private float timeButtonPressed = -10f;

        private void Start()
        {
            playButton.gameObject.SetActive(false);
            optionsButton.gameObject.SetActive(false);
            statsButton.gameObject.SetActive(false);
        }

        public void ShowAll()
        {
            UIObjectAnimator.Instance.PopInObject(playButton);
            UIObjectAnimator.Instance.PopInObject(optionsButton);
            UIObjectAnimator.Instance.PopInObject(statsButton);
        }

        public void PlayButtonPressed()
        {
            if (ButtonRecentlyPressed()) return;
            timeButtonPressed = Time.time;
            GameStatics.UI.MainMenuTransitions.AnimateToLevelSelect();
            StartCoroutine(ButtonPressedRoutine(playButton));
        }

        public void OptionsButtonPressed()
        {
            if (ButtonRecentlyPressed()) return;
            timeButtonPressed = Time.time;
            StartCoroutine(ShowDropdownRoutine(GameStatics.UI.DropdownMenu.ShowOptions));
        }

        public void StatsButtonPressed()
        {
            if (ButtonRecentlyPressed()) return;
            timeButtonPressed = Time.time;
            StartCoroutine(ShowDropdownRoutine(GameStatics.UI.DropdownMenu.ShowStats));
        }

        private IEnumerator ShowDropdownRoutine(System.Action action)
        {
            GameStatics.UI.MainMenuTransitions.GotoDropdownArea();
            yield return new WaitForSeconds(0.7f);
            action.Invoke();
        }
        
        private IEnumerator ButtonPressedRoutine(RectTransform button)
        {
            if (button != playButton)
            {
                UIObjectAnimator.Instance.PopOutObject(playButton);
            }

            if (button != statsButton)
            {
                UIObjectAnimator.Instance.PopOutObject(statsButton);
            }

            if (button != optionsButton)
            {
                UIObjectAnimator.Instance.PopOutObject(optionsButton);
            }
            
            yield return new WaitForSeconds(0.6f);
            
            UIObjectAnimator.Instance.PopOutObject(button);
        }

        private bool ButtonRecentlyPressed()
        {
            return Time.time < timeButtonPressed + 2f;
        }
    }
}
