using UnityEngine;
using System.Collections;

namespace ClumsyBat.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        public bool LoadOnStartup;

        public Animator MothAnimator;
        public CanvasGroup LoadingCanvas;
        public RectTransform LoadTextRt;

        private void Awake()
        {
            MothAnimator.Play("MothFlapAnim", 0, 0f);
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
            MothAnimator.gameObject.SetActive(false);

            const float duration = 0.2f;
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                LoadingCanvas.alpha = Mathf.Lerp(0f, 1f, timer / duration);
                yield return null;
            }

            UIObjectAnimator.Instance.PopInObject(MothAnimator.GetComponent<RectTransform>());
            MothAnimator.Play("MothOpenWings", 0, 0f);
            yield return StartCoroutine(UIObjectAnimator.Instance.PopInObjectRoutine(LoadTextRt));
        }

        public IEnumerator HideLoadScreen(float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            UIObjectAnimator.Instance.PopOutObject(LoadTextRt);
            yield return StartCoroutine(UIObjectAnimator.Instance.PopOutObjectRoutine(MothAnimator.GetComponent<RectTransform>()));

            const float duration = 0.2f;
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
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