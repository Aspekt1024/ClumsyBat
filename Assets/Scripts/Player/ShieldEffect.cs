using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : MonoBehaviour {

    private CircleCollider2D shieldCollider;
    private SpriteRenderer shieldRenderer;

    private float effectDuration;

    public void BeginShieldEffect(float duration)
    {
        StartCoroutine(PlayShieldEffect(duration));
    }

    private void Start ()
    {
        shieldCollider = GetComponent<CircleCollider2D>();
        shieldRenderer = GetComponent<SpriteRenderer>();
        DisableShield();
    }

    private void DisableShield()
    {
        shieldCollider.enabled = false;
        shieldRenderer.enabled = false;
    }

    private void EnableShield()
    {
        shieldCollider.enabled = true;
        shieldRenderer.enabled = true;
    }

    private IEnumerator PlayShieldEffect(float duration)
    {
        EnableShield();

        float timer = 0f;
        float flashTimer = 0f;
        const float flashInterval = 0.045f;
        bool flashOn = false;

        while (timer < duration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
                flashTimer += Time.deltaTime;
                if (flashTimer >= flashInterval)
                {
                    flashTimer = 0f;
                    flashOn = !flashOn;
                    if (flashOn)
                        shieldRenderer.color = new Color(1f, 1f, 1f, 0.6f);
                    else
                        shieldRenderer.color = new Color(1f, 1f, 1f, 0.2f);
                }
            }
            yield return null;
        }
        DisableShield();
    }
}
