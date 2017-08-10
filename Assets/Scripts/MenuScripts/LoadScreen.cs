using UnityEngine;
using System.Collections;

public class LoadScreen : MonoBehaviour {

    private Animator _mothAnimator;
    CanvasGroup _loadingCanvas;

    public bool LoadOnStartup;

    private void Awake()
    {
        _mothAnimator = GameObject.Find("LoadingMoth").GetComponent<Animator>();
        _mothAnimator.Play("MothFlapAnim", 0, 0f);
        _loadingCanvas = GetComponent<CanvasGroup>();

        if (LoadOnStartup)
        {
            _loadingCanvas.alpha = 1f;
            _loadingCanvas.blocksRaycasts = true;
            _loadingCanvas.interactable = true;
        }
    }

    public void ShowLoadScreen()
    {
        _mothAnimator.Play("MothFlapAnim", 0, 0f);
        _loadingCanvas.alpha = 1f;
        _loadingCanvas.blocksRaycasts = true;
        _loadingCanvas.interactable = true;
    }

    public void HideLoadScreen()
    {
        _loadingCanvas.alpha = 0f;
        _loadingCanvas.blocksRaycasts = false;
        _loadingCanvas.interactable = false;
    }

    public void HideLoadScreen(float delay)
    {
        StartCoroutine(HideLoadAfterDelay(delay));
    }

    private IEnumerator HideLoadAfterDelay(float delay)
    {
        yield return StartCoroutine(Toolbox.UIAnimator.Wait(delay));
        HideLoadScreen();
    }
}
