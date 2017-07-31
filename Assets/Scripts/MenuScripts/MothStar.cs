using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MothStar : MonoBehaviour {
    
    private Image activeImage;
    private Image inactiveImage;
    private Text text;

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
                activeImage = tf.GetComponent<Image>();
            }
            else if (tf.name == "Inactive")
            {
                inactiveImage = tf.GetComponent<Image>();
            }
            else if (tf.name == "Text")
            {
                text = tf.GetComponent<Text>();
            }
        }
    }

    public void HideText()
    {
        text.gameObject.SetActive(false);
    }

    public void ShowText()
    {
        text.gameObject.SetActive(true);
    }

    public void SetText(string txt)
    {
        text.text = txt;
    }

    public void SetActive()
    {
        if (inactiveImage != null) inactiveImage.enabled = false;
        if (activeImage != null) activeImage.enabled = true;
    }

    public void SetInactive()
    {
        if (inactiveImage != null) inactiveImage.enabled = true;
        if (activeImage != null) activeImage.enabled = false;
    }

    public IEnumerator AnimateToActive()
    {
        SetActive();
        yield return StartCoroutine(PulseObject(activeImage.GetComponent<RectTransform>()));
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
