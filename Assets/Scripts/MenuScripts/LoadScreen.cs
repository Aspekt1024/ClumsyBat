using UnityEngine;
using System.Collections;
using ClumsyBat.Managers;

public class LoadScreen : MonoBehaviour {

    public bool LoadOnStartup;

    public Animator MothAnimator;
    public CanvasGroup LoadingCanvas;
    public RectTransform LoadTextRt;

    private void Awake()
    {
        CameraManager.OnCameraChanged += CameraChanged;

        MothAnimator.Play("MothFlapAnim", 0, 0f);
        LoadingCanvas = GetComponent<CanvasGroup>();

        if (LoadOnStartup)
        {
            LoadingCanvas.alpha = 1f;
            LoadingCanvas.blocksRaycasts = true;
            LoadingCanvas.interactable = true;
        }
    }

    public void ShowLoadScreen()
    {
        StartCoroutine(FadeIn());
    }

    public void HideLoadScreen()
    {
        StartCoroutine(FadeOut());
    }

    public void HideLoadScreen(float delay)
    {
        StartCoroutine(HideLoadAfterDelay(delay));
    }

    private IEnumerator HideLoadAfterDelay(float delay)
    {
        yield return StartCoroutine(UIObjectAnimator.Instance.Wait(delay));
        yield return StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
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

    public IEnumerator FadeOut()
    {
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
