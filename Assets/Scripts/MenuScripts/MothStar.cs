using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MothStar : MonoBehaviour {
    
    private Image ActiveImage;
    private Image InactiveImage;

	private void Awake ()
    {
        GetSprites();
    }

    private void GetSprites()
    {
        foreach (Transform tf in GetComponentInChildren<Transform>())
        {
            if (tf.name == "Active")
            {
                ActiveImage = tf.GetComponent<Image>();
            }
            else if (tf.name == "Inactive")
            {
                InactiveImage = tf.GetComponent<Image>();
            }
        }
    }

    public void SetActive()
    {
        if (InactiveImage != null) InactiveImage.enabled = false;
        if (ActiveImage != null) ActiveImage.enabled = true;
    }

    public void SetInactive()
    {
        if (InactiveImage != null) InactiveImage.enabled = true;
        if (ActiveImage != null) ActiveImage.enabled = false;
    }

    public IEnumerator AnimateToActive()
    {
        SetActive();
        yield return StartCoroutine(PulseObject(ActiveImage.GetComponent<RectTransform>()));
    }

    private IEnumerator PulseObject(RectTransform rtObj)
    {
        float animTimer = 0f;
        const float animDuration = 0.2f;
        const float scaleMax = 0.25f;

        Vector3 startScale = rtObj.localScale;
        
        while (animTimer < animDuration)
        {
            float scale;
            if (animTimer > animDuration / 2)
            {
                scale = (1f + scaleMax) - (scaleMax * (animTimer - animDuration / 2) / (animDuration / 2));
            }
            else
            {
                scale = 1f + (scaleMax * animTimer / (animDuration / 2));
            }
            rtObj.localScale = startScale * scale;
            animTimer += Time.deltaTime;
            yield return null;
        }
    }
}
