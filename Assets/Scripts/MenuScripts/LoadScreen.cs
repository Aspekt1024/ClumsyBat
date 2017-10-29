using UnityEngine;
using System.Collections;

public class LoadScreen : MonoBehaviour {

    public bool LoadOnStartup;

    private Animator mothAnimator;
    private CanvasGroup loadingCanvas;
    private RectTransform loadTextRt;

    private void Awake()
    {
        GetLoadScreenComponents();
        mothAnimator.Play("MothFlapAnim", 0, 0f);
        loadingCanvas = GetComponent<CanvasGroup>();

        if (LoadOnStartup)
        {
            loadingCanvas.alpha = 1f;
            loadingCanvas.blocksRaycasts = true;
            loadingCanvas.interactable = true;
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
        yield return StartCoroutine(Toolbox.UIAnimator.Wait(delay));
        yield return StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {
        loadingCanvas.blocksRaycasts = true;
        loadingCanvas.interactable = true;
        mothAnimator.gameObject.SetActive(false);

        const float duration = 0.2f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            loadingCanvas.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        Toolbox.UIAnimator.PopInObject(mothAnimator.GetComponent<RectTransform>());
        mothAnimator.Play("MothOpenWings", 0, 0f);
        yield return StartCoroutine(Toolbox.UIAnimator.PopInObjectRoutine(loadTextRt));
    }

    private IEnumerator FadeOut()
    {
        Toolbox.UIAnimator.PopOutObject(loadTextRt);
        yield return StartCoroutine(Toolbox.UIAnimator.PopOutObjectRoutine(mothAnimator.GetComponent<RectTransform>()));

        const float duration = 0.2f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            loadingCanvas.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }
        loadingCanvas.alpha = 0f;
        loadingCanvas.blocksRaycasts = false;
        loadingCanvas.interactable = false;
    }

    private void GetLoadScreenComponents()
    {
        foreach(RectTransform rt in GetComponent<RectTransform>())
        {
            if (rt.name == "Darkness")
            {
                foreach(RectTransform r in rt)
                {
                    if (r.name == "LoadingMoth")
                        mothAnimator = r.GetComponent<Animator>();
                    else if (r.name == "Text")
                        loadTextRt = r;
                }
            }
        }
    }
}
