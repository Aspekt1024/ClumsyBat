using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObjectAnimator : MonoBehaviour {

    private struct RTObject
    {
        public RectTransform Obj;
        public Vector3 OriginalScale;
        public Coroutine Routine;
    }
    private List<RTObject> rtObjects;

    private static UIObjectAnimator objAnimator;
    public static UIObjectAnimator Instance
    {
        get
        {
            if (objAnimator == null)
            {
                objAnimator = FindObjectOfType<UIObjectAnimator>();
                if (objAnimator == null)
                    Debug.LogError("Missing UIObjectAnimator in Scene");

                objAnimator.rtObjects = new List<RTObject>();
            }
            return objAnimator;
        }
    }

    public void PopObject(RectTransform rt)
    {
        RemoveObjIfExisting(rt);

        RTObject obj = new RTObject
        {
            Obj = rt,
            OriginalScale = rt.localScale,
            Routine = StartCoroutine(PopObjectRoutine(rt))
        };
        Instance.rtObjects.Add(obj);
    }

    public void PopInObject(RectTransform rt)
    {
        StartCoroutine(PopInObjectRoutine(rt));
    }

    public void PopOutObject(RectTransform rt)
    {
        StartCoroutine(PopOutObjectRoutine(rt));
    }

    public IEnumerator PopObjectRoutine(RectTransform rt)
    {
        float timer = 0f;
        float duration = 0.1f;

        Vector3 originalScale = rt.localScale;
        rt.gameObject.SetActive(true);
        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (timer > duration) timer = duration;
            rt.localScale = Vector3.Lerp(originalScale, originalScale * 1.1f, timer / duration);
            yield return null;
        }

        timer = 0f;
        duration = 0.08f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (timer > duration) timer = duration;
            rt.localScale = Vector3.Lerp(originalScale * 1.1f, originalScale, timer / duration);
            yield return null;
        }

        RemoveObjIfExisting(rt);
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
            if (timer > duration) timer = duration;
            rt.localScale = Vector3.Lerp(Vector3.one * 0.1f, originalScale * 1.1f, timer / duration);
            yield return null;
        }

        timer = 0f;
        duration = 0.08f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (timer > duration) timer = duration;
            rt.localScale = Vector3.Lerp(originalScale * 1.1f, originalScale, timer / duration);
            yield return null;
        }
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

    private void RemoveObjIfExisting(RectTransform rt)
    {
        foreach (RTObject rtObj in Instance.rtObjects)
        {
            if (rtObj.Obj.gameObject == rt.gameObject)
            {
                StopCoroutine(rtObj.Routine);
                rtObj.Obj.localScale = rtObj.OriginalScale;
                Instance.rtObjects.Remove(rtObj);
                break;
            }
        }
    }
}