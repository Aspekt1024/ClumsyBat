using UnityEngine;
using System.Collections;

public class LoadScreen : MonoBehaviour {

    private Animator MothAnimator = null;
    CanvasGroup LoadingCanvas = null;

    void Awake()
    {
        MothAnimator = GameObject.Find("LoadingMoth").GetComponent<Animator>();
        MothAnimator.Play("MothGreenFlap", 0, 0f);
        LoadingCanvas = GetComponent<CanvasGroup>();
        LoadingCanvas.alpha = 1f;
        LoadingCanvas.blocksRaycasts = true;
        LoadingCanvas.interactable = true;
    }
}
