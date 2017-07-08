using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothStar : MonoBehaviour {
    
    private SpriteRenderer ActiveSprite;
    private SpriteRenderer InactiveSprite;

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
                ActiveSprite = tf.GetComponent<SpriteRenderer>();
            }
            else if (tf.name == "Inactive")
            {
                InactiveSprite = tf.GetComponent<SpriteRenderer>();
            }
        }
    }

    public void SetActive()
    {
        InactiveSprite.enabled = false;
        ActiveSprite.enabled = true;
    }

    public void SetInactive()
    {
        InactiveSprite.enabled = true;
        ActiveSprite.enabled = false;
    }

    public IEnumerator AnimateToActive()
    {
        SetActive();
        yield return StartCoroutine(PulseObject(ActiveSprite.GetComponent<RectTransform>()));
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
