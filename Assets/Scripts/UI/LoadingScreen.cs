using UnityEngine;
using System.Collections;

namespace ClumsyBat.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        public bool LoadOnStartup;
        public float BaseHideDelay = 0f;

        public CanvasGroup LoadingCanvas;
        public RectTransform LoadTextRt;

        private void Awake()
        {
            LoadingCanvas = GetComponent<CanvasGroup>();

            if (LoadOnStartup)
            {
                LoadingCanvas.alpha = 1f;
                LoadingCanvas.blocksRaycasts = true;
                LoadingCanvas.interactable = true;
            }
        }

        private void Start()
        {
            GameStatics.Camera.OnCameraChanged += CameraChanged;
        }

        public IEnumerator ShowLoadScreen()
        {
            LoadingCanvas.blocksRaycasts = true;
            LoadingCanvas.interactable = true;

            const float duration = 0.2f;
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                LoadingCanvas.alpha = Mathf.Lerp(0f, 1f, timer / duration);
                yield return null;
            }

            yield return StartCoroutine(UIObjectAnimator.Instance.PopInObjectRoutine(LoadTextRt));
        }

        public IEnumerator HideLoadScreen(float delay = 0f)
        {
            yield return new WaitForSecondsRealtime(delay + BaseHideDelay);

            UIObjectAnimator.Instance.PopOutObject(LoadTextRt);

            const float duration = 0.2f;
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                LoadingCanvas.alpha = Mathf.Lerp(1f, 0f, timer / duration);
                yield return null;
            }
            LoadingCanvas.alpha = 0f;
            LoadingCanvas.blocksRaycasts = false;
            LoadingCanvas.interactable = false;
        }
        
        private void CameraChanged(Camera newCamera)
        {
            GetComponent<Canvas>().worldCamera = newCamera;
        }
    }
}