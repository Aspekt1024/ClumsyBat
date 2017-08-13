using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObjectAnimator : MonoBehaviour {

    private static UIObjectAnimator objAnimator;
    public static UIObjectAnimator Instance
    {
        get
        {
            if (objAnimator == null)
            {
                objAnimator = FindObjectOfType<UIObjectAnimator>();
                if (objAnimator == null)
                {
                    Debug.LogError("Missing UIObjectAnimator in Scene");
                }
            }
            return objAnimator;
        }
    }

    public void PopInObject(RectTransform rt)
    {
        StartCoroutine(PopInObjectRoutine(rt));
    }

    public void PopOutObject(RectTransform rt)
    {
        StartCoroutine(PopOutObjectRoutine(rt));
    }

    public IEnumerator PopInObjectRoutine(RectTransform rt)
    {
        float timer = 0f;
        float duration = 0.2f;

        Vector3 originalScale = rt.localScale;
        rt.gameObject.SetActive(true);
        while (timer < duration)
        {
            timer += Time.deltaTime;
            rt.localScale = Vector3.Lerp(Vector3.one * 0.1f, originalScale * 1.1f, timer / duration);
            yield return null;
        }

        timer = 0f;
        duration = 0.08f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale * 1.1f, originalScale, timer / duration);
            yield return null;
        }
        rt.localScale = originalScale;
    }

    public IEnumerator PopOutObjectRoutine(RectTransform rt)
    {
        float timer = 0f;
        float duration = 0.08f;

        Vector3 originalScale = rt.localScale;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale, originalScale * 1.1f, timer / duration);
            yield return null;
        }

        timer = 0f;
        duration = 0.2f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale * 1.1f, Vector3.one * 0.1f, timer / duration);
            yield return null;
        }
        rt.gameObject.SetActive(false);
        rt.localScale = originalScale;
    }

    public IEnumerator Wait(float waitTime)
    {
        float timer = 0f;
        while (timer < waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }
}