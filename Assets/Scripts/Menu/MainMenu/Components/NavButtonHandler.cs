using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ClumsyBat.Menu.MainMenu
{
    public class NavButtonHandler : MonoBehaviour
    {
        private RectTransform backButton;

        void Awake()
        {
            foreach (RectTransform RT in GetComponent<RectTransform>())
            {
                if (RT.name == "BackButton")
                {
                    backButton = RT;
                    backButton.gameObject.SetActive(false);
                }
            }
        }

        public void DisableNavButtons()
        {
            DisableBackButton();
        }

        public void EnableBackButton()
        {
            UIObjectAnimator.Instance.PopInObject(backButton);
        }

        public void DisableBackButton()
        {
            UIObjectAnimator.Instance.PopOutObject(backButton);
        }

        public void HandleReturnToMainMenu()
        {
            GameStatics.GameManager.GotoMenuScene();
        }
    }
}
