﻿using UnityEngine;
using System.Collections;

public class LoadScreen : MonoBehaviour {

    private Animator MothAnimator = null;
    CanvasGroup LoadingCanvas = null;

    public bool LoadOnStartup;

    void Awake()
    {
        MothAnimator = GameObject.Find("LoadingMoth").GetComponent<Animator>();
        MothAnimator.Play("MothFlapAnim", 0, 0f);
        LoadingCanvas = GetComponent<CanvasGroup>();

        if (LoadOnStartup)
        {
            LoadingCanvas.alpha = 1f;
            LoadingCanvas.blocksRaycasts = true;
            LoadingCanvas.interactable = true;
        }
    }

    public void SetupLoadScreen()
    {
        LoadingCanvas.alpha = 1f;
        LoadingCanvas.blocksRaycasts = true;
        LoadingCanvas.interactable = true;
    }
}
