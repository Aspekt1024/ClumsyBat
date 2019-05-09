using System.Collections;
using UnityEngine;

namespace ClumsyBat.UI
{
    public class DeathOverlay : MonoBehaviour
    {
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            HideImmediate();
        }

        public void HideImmediate()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        public void Hide()
        {
            StartCoroutine(FadeRoutine(false));
        }

        public void Show()
        {
            StartCoroutine(FadeRoutine(true));
        }

        private IEnumerator FadeRoutine(bool isFadeIn)
        {
            const float DURATION = 0.3f;
            float timer = 0f;
            float from = isFadeIn ? 0f : 1f;
            float to = isFadeIn ? 1f : 0f;

            while (timer < DURATION)
            {
                timer += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, timer / DURATION);
                yield return null;
            }

            canvasGroup.alpha = to;
            canvasGroup.blocksRaycasts = isFadeIn;
            canvasGroup.interactable = isFadeIn;
        }
    }
}
