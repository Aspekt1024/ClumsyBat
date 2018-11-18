using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashIndicator : MonoBehaviour {

    private RectTransform lightRt;
    private RectTransform arrow1;
    private RectTransform arrow2;
    private Image radialShadow;

    private float cooldownDuration;
    private float cooldownTimer;
    private bool isReady;
    private bool isEnabled;
    
    public void StartCooldown(float duration)
    {
        cooldownDuration = duration;
        cooldownTimer = duration;
        if (!isEnabled || !isReady) return;
        
        isReady = false;
        UIObjectAnimator.Instance.PopOutObject(lightRt);
        UIObjectAnimator.Instance.PopOutObject(arrow1);
        UIObjectAnimator.Instance.PopOutObject(arrow2);
    }

    public void Enable()
    {
        isEnabled = true;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        isEnabled = false;
        gameObject.SetActive(false);
    }

    private void Start ()
    {
        GetIndicatorComponents();

        radialShadow.fillAmount = 1f;
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
        lightRt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isReady || Toolbox.Instance.GamePaused) return;
        cooldownTimer -= Time.deltaTime;
        SetCooldownPercentRemaining(cooldownTimer / cooldownDuration);
    }

    private void SetCooldownPercentRemaining(float percent)
    {
        if (!isEnabled) return;

        radialShadow.fillAmount = percent;

        if (Mathf.Abs(percent) < 0.01f)
            SetReadyGraphics();
    }

    private void SetReadyGraphics()
    {
        if (isReady) return;

        isReady = true;
        UIObjectAnimator.Instance.PopInObject(lightRt);
        UIObjectAnimator.Instance.PopInObject(arrow1);
        UIObjectAnimator.Instance.PopInObject(arrow2);
        radialShadow.fillAmount = 0f;
    }
    
    private void GetIndicatorComponents()
    {
        foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
        {
            if (rt.name == "Light")
                lightRt = rt;
            else if (rt.name == "DashArrow1")
                arrow1 = rt;
            else if (rt.name == "DashArrow2")
                arrow2 = rt;
            else if (rt.name == "RadialProgressBar")
                radialShadow = rt.GetComponent<Image>();
        }
    }
}
