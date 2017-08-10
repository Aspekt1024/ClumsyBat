using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NavButtonHandler : MonoBehaviour
{
    private RectTransform backButton;
    
    void Awake()
    {
        foreach (RectTransform RT in GetComponent<RectTransform>())
        {
            if (RT.name == "BackButton")
            {
                backButton = RT;
                backButton.gameObject.SetActive(false);
            }
        }
    }

    public void DisableNavButtons()
    {
        DisableBackButton();
    }
    
    public void SetNavButtons(CamPositioner.Positions camPosition)
    {
        switch (camPosition)
        {
            case CamPositioner.Positions.LevelSelect:
                EnableBackButton();
                break;
            case CamPositioner.Positions.Main:
                //EnableBackButton();
                break;
            case CamPositioner.Positions.DropdownArea:
                //EnableBackButton();
                break;
            default:
                break;
        }
    }

    private void EnableBackButton()
    {
        StartCoroutine(PopInObject(backButton));
    }

    private void DisableBackButton()
    {
        StartCoroutine(PopOutObject(backButton));
    }

    private IEnumerator PopInObject(RectTransform rt)
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

    private IEnumerator PopOutObject(RectTransform rt)
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
}
